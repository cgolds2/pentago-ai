using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;


namespace PentagoAICrossP
{
    class Program
    {
        static int[] lastTurn = new int[85];
        static GameMove lastMove;
        static List<int[]> turns = new List<int[]>();
        static List<int[]> GetTurns()
        {
            return turns;
        }
        static void AddTurn(int[] t)
        {
            if (t.Length != 85)
            {
                throw new ArgumentException("Array did not have 85 elements");
            }
            turns.Add(t);
        }
        static void TrackPlacement(int x, int y)
        {
            lastTurn[x * 6 + y] = 1;
        }
        static void TrackRotation(int x, String y)
        {
            if (y == "right" || y == "Right" || y == "r" || y == "R")
                TrackRotation(x, false);
            else
                TrackRotation(x, true);
        }
        static void TrackRotation(int x, bool rotLeft)
        {
            if (rotLeft)
            {
                lastTurn[84 - ((x * 2) + 1)] = 1;

            }
            else
            {
                lastTurn[84 - (x * 2)] = 1;

            }
        }
        static void UpdateTurn()
        {
            AddTurn(lastTurn);
            lastTurn = new int[85];
        }



        //rotational index of values in squares
        static readonly int[] rotIndex = {
                                        0, 1, 2,
                                        6, 7, 8,
                                        12, 13, 14
                                };
        //swuare that has been left rotated
        static readonly int[] leftRotIndex = {
                                        12, 6, 0,
                                        13, 7, 1,
                                        14, 8, 2
                                };
        //square that has been right rotated
        static readonly int[] rightRotIndex = {
                                        2, 8, 14,
                                        1, 7, 13,
                                        0, 6, 12
                                };

        //keeps track of which player is allowed to move
        static bool isXTurn = false;

        static TupleList<int, int> PointsFromWinCondition(int index)
        {
            TupleList<int, int> returnValues = new TupleList<int, int>();
            if (index <= 11)
            {
                //horizontal
                int additive = index % 2;
                for (int i = 0; i < 5; i++)
                {
                    returnValues.Add(new MyTuple<int, int>(i + additive, index / 2));
                }
            }
            else if (index <= 23)
            {
                //verticle
                int additive = index % 2;
                for (int i = 0; i < 5; i++)
                {
                    returnValues.Add(new MyTuple<int, int>((index - 12) / 2, i + additive));
                }
            }
            else if (index <= 27)
            {
                //diag 1
                int x = -1;
                int y = -1;
                switch (index)
                {
                    case 24:
                        x = 0;
                        y = 1;
                        break;
                    case 25:
                        x = 0;
                        y = 0;
                        break;
                    case 26:
                        x = 1;
                        y = 1;
                        break;
                    case 27:
                        x = 1;
                        y = 0;
                        break;
                    default:
                        break;
                }
                return DiagFromPoint(x, y, true);

            }
            else if (index <= 31)
            {
                //diag 2        
                int x = -1;
                int y = -1;
                switch (index)
                {
                    case 28:
                        x = 4;
                        y = 0;
                        break;
                    case 29:
                        x = 5;
                        y = 0;
                        break;
                    case 30:
                        x = 4;
                        y = 1;
                        break;
                    case 31:
                        x = 5;
                        y = 1;
                        break;
                    default:
                        break;
                }
                return DiagFromPoint(x, y, false);

            }
            else
            {
                throw new Exception("Out of bounds of win array");
            }
            return returnValues;
        }

        static int[] winValues = new int[32];
        /*
         * 00-11: horizontal
         * 12-23: verticle
         * 24-27: top left bot right diag
         * 28-31: top right bot left diag
         */
        static void UpdatePoint(TileVals[,] board, int x, int y)
        {

            #region Update Horizontal
            if (x != 0)
            {
                //update leftmost to right
                TileVals[] xTiles = CustomArray<TileVals>.GetRowMinusFirst(board, y);
                var xSum = Array.ConvertAll(xTiles, value => (int)value).Sum();
                winValues[y * 2 + 1] = xSum;
            }
            if (x != 5)
            {
                //update left to rightmost
                TileVals[] xTiles = CustomArray<TileVals>.GetRowMinusLast(board, y);
                var xSum = Array.ConvertAll(xTiles, value => (int)value).Sum();
                winValues[y * 2] = xSum;
            }
            #endregion

            #region Update Verticle
            if (y != 0)
            {
                //update topmost to bot
                TileVals[] yTiles = CustomArray<TileVals>.GetColumnMinusFirst(board, x);
                var ySum = Array.ConvertAll(yTiles, value => (int)value).Sum();
                winValues[12 + x * 2] = ySum;
            }
            if (y != 5)
            {
                //update top to botmost
                TileVals[] yTiles = CustomArray<TileVals>.GetColumnMinusLast(board, x);
                var ySum = Array.ConvertAll(yTiles, value => (int)value).Sum();
                winValues[12 + x * 2 + 1] = ySum;
            }
            #endregion

            //update diag (2)
            int tmpX;
            int tmpY;

            #region Update Top left to Bot right diag
            tmpX = x;
            tmpY = y;
            while (tmpX != 0 && tmpY != 0)
            {
                tmpX--;
                tmpY--;
            }
            if (tmpX == 0 && tmpY == 0)
            {
                //top left point
                var diagOne = (DiagFromPoint(0, 0, true));
                var diagTwo = (DiagFromPoint(1, 1, true));
                winValues[24 + 1] = SumDiag(board, diagOne);
                winValues[24 + 2] = SumDiag(board, diagTwo);

            }
            if ((tmpX + tmpY) == 1)
            {
                //other 2 left to right
                var diag = DiagFromPoint(tmpX, tmpY, true);
                TupleList<int, int>[] diags = new TupleList<int, int>[4];
                var diagOne = (DiagFromPoint(0, 1, true));
                var diagTwo = (DiagFromPoint(1, 0, true));
                winValues[24 + 0] = SumDiag(board, diagOne);
                winValues[24 + 3] = SumDiag(board, diagTwo);

            }
            #endregion

            #region Update Top right to Bot left diag
            tmpX = x;
            tmpY = y;
            while (tmpX != 5 && tmpY != 0)
            {
                tmpX++;
                tmpY--;
            }
            if (tmpX == 5 && tmpY == 0)
            {
                //top right point
                var diagOne = (DiagFromPoint(5, 0, false));
                var diagTwo = (DiagFromPoint(4, 1, false));
                winValues[28 + 1] = SumDiag(board, diagOne);
                winValues[28 + 2] = SumDiag(board, diagTwo);
            }
            else if (tmpX == 4 && tmpY == 0)
            {
                //diags[0] = (DiagFromPoint(4, 0, false));
                //diags[1] = (DiagFromPoint(5, 0, false));
                //diags[2] = (DiagFromPoint(4, 1, false));
                //diags[3] = (DiagFromPoint(5, 1, false));
                //winValues[24 + i] = sumDiag(board, diags[i]);

                var diagOne = (DiagFromPoint(4, 0, false));
                winValues[28 + 0] = SumDiag(board, diagOne);
            }
            else if (tmpX == 5 && tmpY == 1)
            {
                var diagOne = (DiagFromPoint(5, 1, false));
                winValues[28 + 3] = SumDiag(board, diagOne);
            }
            #endregion


            PrintBoard(board);



        }

        #region Update wins on square rotation
        static void UpdateRotation(TileVals[,] board, int quad)
        {
            //update horizontal (6)
            //update verticle (6)
            //update diag (4)
            //update one othe diag (1)
            PrintBoard(board);
            UpdateHorizontal(board, quad);
            UpdateVerticle(board, quad);
            UpdateDiagonal(board, quad);
            UpdateOther(board, quad);


        }
        static void UpdateHorizontal(TileVals[,] board, int quad)
        {

            int additive = (quad < 2) ? 0 : 3;

            for (int i = additive; i < 3 + additive; i++)
            {
                // Iterate through the second dimension
                TileVals[] x = CustomArray<TileVals>.GetRowMinusLast(board, i);
                TileVals[] y = CustomArray<TileVals>.GetRowMinusFirst(board, i);
                var xSum = Array.ConvertAll(x, value => (int)value).Sum();
                var ySum = Array.ConvertAll(y, value => (int)value).Sum();

                winValues[i * 2] = xSum;
                winValues[i * 2 + 1] = ySum;

                Console.WriteLine("X1: " + xSum);
                Console.WriteLine("X2: " + ySum);

            }

        }
        static void UpdateVerticle(TileVals[,] board, int quad)
        {
            int additive = (quad == 0 || quad == 2) ? 0 : 3;

            for (int i = additive; i < 3 + additive; i++)
            {
                TileVals[] x = CustomArray<TileVals>.GetColumnMinusLast(board, i);
                TileVals[] y = CustomArray<TileVals>.GetColumnMinusFirst(board, i);
                var xSum = Array.ConvertAll(x, value => (int)value).Sum();
                var ySum = Array.ConvertAll(y, value => (int)value).Sum();

                winValues[12 + i * 2] = xSum;
                winValues[12 + i * 2 + 1] = ySum;

                Console.WriteLine("Y1: " + xSum);
                Console.WriteLine("Y2: " + ySum);
            }

        }
        static void UpdateDiagonal(TileVals[,] board, int quad)
        {
            if (quad == 0 || quad == 3)
            {
                TupleList<int, int>[] diags = new TupleList<int, int>[4];
                diags[0] = (DiagFromPoint(0, 1, true));
                diags[1] = (DiagFromPoint(0, 0, true));
                diags[2] = (DiagFromPoint(1, 1, true));
                diags[3] = (DiagFromPoint(1, 0, true));
                for (int i = 0; i < 4; i++)
                {
                    winValues[24 + i] = SumDiag(board, diags[i]);
                }


            }
            else
            {
                TupleList<int, int>[] diags = new TupleList<int, int>[4];
                diags[0] = (DiagFromPoint(4, 0, false));
                diags[1] = (DiagFromPoint(5, 0, false));
                diags[2] = (DiagFromPoint(4, 1, false));
                diags[3] = (DiagFromPoint(5, 1, false));
                for (int i = 0; i < 4; i++)
                {
                    winValues[28 + i] = SumDiag(board, diags[i]);
                }
            }

        }

        static int GetQuadFromPoint(int x, int y)
        {
            if (x <= 2 && y <= 2)
            {
                return 0;
            }
            else if (x <= 2)
            {
                return 2;
            }
            else if (y <= 2)
            {
                return 1;
            }
            else
            {
                return 3;
            }
        }

        static void UpdateOther(TileVals[,] board, int quad)
        {
            switch (quad)
            {
                case 0:
                    winValues[28] = SumDiag(board, DiagFromPoint(4, 0, false));
                    break;
                case 1:
                    winValues[24 + 3] = SumDiag(board, DiagFromPoint(1, 0, true));

                    break;
                case 2:
                    winValues[24 + 0] = SumDiag(board, DiagFromPoint(0, 1, true));

                    break;
                case 3:
                    winValues[28 + 3] = SumDiag(board, DiagFromPoint(5, 1, false));
                    break;
                default:
                    throw new ArgumentException("quad greater than 3");
            }
        }
        #endregion

        static TileVals[,] RotateSquare(TileVals[,] board, int squareToRotate, bool rotLeft)
        {
            int baseForIndex = 0;
            if (squareToRotate > 1)
            {
                baseForIndex += 18;
            }
            if (squareToRotate == 1 || squareToRotate == 3)
            {
                baseForIndex += 3;
            }
            TileVals[] tempTiles = new TileVals[9];
            for (int i = 0; i < 9; i++)
            {
                int fromSpot = rotIndex[i] + baseForIndex;
                int fromX = fromSpot % 6;
                int fromY = fromSpot / 6;
                tempTiles[i] = board[fromX, fromY];
            }
            if (rotLeft)
            {
                for (int i = 0; i < 9; i++)
                {
                    int x = (leftRotIndex[i] + baseForIndex) % 6;
                    int y = (leftRotIndex[i] + baseForIndex) / 6;
                    board[x, y] = tempTiles[i];
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    int x = (rightRotIndex[i] + baseForIndex) % 6;
                    int y = (rightRotIndex[i] + baseForIndex) / 6;
                    board[x, y] = tempTiles[i];

                }
            }
            return board;

        }

        static void Main(string[] args)
        {
            var x = RunCmd.RunPythonThing();
            TileVals[,] gameBoard = new TileVals[6, 6];
            PrintBoard(gameBoard);
            bool useHeu;
            bool useNN;

            int choice = TryGetInt("\n1: Play against Heuristic\n" +
                                   "2: Play against another player\n" +
                                   "3: Play against NN\n" +
                                   "4: Play NN against Heuristic", 1, 4);

            useHeu = (choice == 1 || choice == 4);
            useNN = (choice == 3 || choice == 4);


            //main game loop
            while (true)
            {
                //change turn
                isXTurn = !isXTurn;

                PrintBoard(gameBoard);
                GameMove g;
                if (isXTurn && useHeu)
                {

                    g = PentagoHeuristic(gameBoard);
                }
                else if (!isXTurn && useNN)
                {
                    g = NNTurn(gameBoard);
                }
                else
                {
                    g = PlayerTurn(gameBoard);
                }
                lastMove = g;


                gameBoard[g.xCord, g.yCord] = isXTurn ? TileVals.X : TileVals.O;
                UpdatePoint(gameBoard, g.xCord, g.yCord);
                TrackPlacement(g.xCord, g.yCord);

                TrackRotation(g.rotIndex, g.rotLeft);

                //rotation
                gameBoard = RotateSquare(gameBoard, g.rotIndex, g.rotLeft);


                UpdateRotation(gameBoard, g.rotIndex);
                UpdateTurn();
                if (IsGameWon(gameBoard))
                {
                    PrintBoard(gameBoard);
                    Console.Write("Game Over On Turn " + GetTurns().Count + ".\n");
                    break;
                }
            }
        }






        static GameMove PlayerTurn(TileVals[,] gameBoard)
        {
            GameMove playerMove = new GameMove();
            //set vals to illegal by default
            var xVal = -1;
            var yVal = -1;
            while (true)
            {
                xVal = TryGetInt("x value", 0, 5);
                yVal = TryGetInt("y value", 0, 5);
                //make sure the value selected is actually open
                if (gameBoard[xVal, yVal] != TileVals.Blank)
                {
                    PrintBoard(gameBoard);
                    Console.WriteLine("Square already taken\n");
                }
                else
                {
                    break;
                }
            }
            //place an X or O depending on whos turn it is
            playerMove.xCord = xVal;
            playerMove.yCord = yVal;

            int square = TryGetInt("index of square to rotate:\n0 1\n2 3", 0, 3);
            string rot = "";
            Console.WriteLine("Enter (L)eft or (R)ight for rotation");
            rot = Console.ReadLine();

            //list of valid values for rotation
            var rotationInput = new List<string> { "right", "left", "r", "l" };

            while (!rotationInput.Contains(rot.ToLower()))
            {
                Console.WriteLine("rotate not valid");
                rot = Console.ReadLine();
            }

            Console.WriteLine("You entered " + rot);
            playerMove.rotIndex = square;
            playerMove.rotLeft = (rot.ToLower() == "left" || rot.ToLower() == "l");
            return playerMove;
        }

        static GameMove NNTurn(TileVals[,] gameBoard)
        {
            throw new Exception("fill this out");
        }


        static int SumDiag(TileVals[,] board, TupleList<int, int> diags)
        {
            int retVal = 0;

            foreach (var item in diags)
            {
                retVal += (int)board[item.Item1, item.Item2];
            }
            return retVal;
        }
        static TupleList<int, int> DiagFromPoint(int x, int y, bool leftToRight)
        {
            var diag = new TupleList<int, int>();
            diag.Add(x, y);
            for (int i = 0; i < 4; i++)
            {
                x = leftToRight ? x + 1 : x - 1;
                y++;
                diag.Add(x, y);
            }
            return diag;

        }

        static int MatchArray(int[] basArr, List<int[]> matches)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (basArr.SequenceEqual(matches[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        static int[] EnumArrToNNArr(TileVals[,] enumArr)
        {
            var x = enumArr.Cast<TileVals>().ToList().ToArray();
            return Array.ConvertAll(x, value => (int)value);
        }

        static TileVals[,] RotateBoard(TileVals[,] board, int n)
        {
            TileVals[,] ret = new TileVals[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    ret[i, j] = board[n - j - 1, i];
                }
            }
            return ret;
        }
        static bool IsGameWon(TileVals[,] board)
        {
            bool didXWin = false;
            bool didOWin = false;
            foreach (var item in winValues)
            {
                if (item == 50)
                {
                    didOWin = true;
                    Console.WriteLine("O won");
                }
                if (item == 5)
                {
                    didXWin = true;
                    Console.WriteLine("X won");
                }
            }

            return didXWin || didOWin;
        }
        static void PrintBoard(TileVals[,] board)
        {
            Console.Clear();
            Console.WriteLine("   0 1 2     3 4 5");
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (j == 0)
                    {
                        Console.Write(i + " ");
                    }
                    if (j == 3)
                    {
                        Console.Write("|   ");

                    }
                    Console.Write("|" + TileToString(board[j, i]));
                }
                Console.WriteLine("|");
                if (i == 2)
                {
                    Console.WriteLine("");
                }
            }
            if (lastMove != null)
            {
                Console.WriteLine("\n--------------");
                Console.Write("Previous move = ({0},{1}),\n" +
                              "Rotated square {2} {3}clockwise",
                                  lastMove.xCord, lastMove.yCord, lastMove.rotIndex, lastMove.rotLeft ? "counter" : "");
            }
            Console.WriteLine("\n--------------");
            Console.WriteLine((isXTurn ? "Player 1" : "Player 2") + "'s turn");
            Console.WriteLine("--------------\n");

        }
        static int TryGetInt(string prompt, int min, int max)
        {
            int ret;
            Console.WriteLine("Enter " + prompt);
            while (true)
            {
                bool successfullyParsed = int.TryParse(Console.ReadLine(), out ret);
                if (successfullyParsed)
                {
                    if (ret >= min && ret <= max)
                    {
                        break;
                    }
                }
                Console.WriteLine("Enter valid " + prompt);
            }
            Console.WriteLine("You entered " + ret);
            return ret;
        }
        static string TileToString(TileVals t)
        {
            switch (t)
            {
                case TileVals.X:
                    return "X";
                case TileVals.O:
                    return "O";
                case TileVals.Blank:
                    return " ";
                default:
                    return "error";
            }
        }
        static Random r = new Random();
        static int startQuadrant = -1;
        static GameMove PentagoHeuristic(TileVals[,] board)
        {

            for (int i = 0; i < winValues.Length; i++)
            {
                if (winValues[i] == 4 || winValues[i] == 40)
                {
                    var points = PointsFromWinCondition(i);
                    foreach (var item in points)
                    {
                        if (board[item.Item1, item.Item2] == TileVals.Blank)
                        {
                            int quad = GetQuadFromPoint(item.Item1, item.Item2);
                            return new GameMove(item.Item1, item.Item2, quad, false);
                        }

                    }
                }
            }
            int turnCounter = GetTurns().Count / 2;
            turnCounter++;
            GameMove ret = new GameMove();
            ret.rotIndex = -1;
            //EARLY GAME
            //first turn
            if (turnCounter == 1)
            {
                startQuadrant = r.Next(4);
                if (startQuadrant == 0)
                {
                    /*
                    xVal = 2;
                    yVal = 2;
                    RotateSquare(board, 3, true);
                    */
                    ret = new GameMove(2, 2, 3, true);
                }
                else if (startQuadrant == 1)
                {
                    /*
                    xVal = 3;
                    yVal = 2;
                    RotateSquare(board, 2, true);
                    */
                    ret = new GameMove(3, 2, 2, true);
                }
                else if (startQuadrant == 2)
                {
                    /*
                    xVal = 2;
                    yVal = 3;
                    RotateSquare(board, 1, true);
                    */
                    ret = new GameMove(2, 3, 1, true);
                }
                else if (startQuadrant == 3)
                {
                    /*
                    xVal = 3;
                    yVal = 3;
                    RotateSquare(board, 0, true);
                    */
                    ret = new GameMove(3, 3, 0, true);
                }
            }
            //second turn
            else if (turnCounter == 2)
            {
                if (startQuadrant == 0)
                {
                    if ((int)board[2, 0] == 0)
                    {
                        ret = new GameMove(2, 0, 3, true);
                    }
                    else if ((int)board[0, 2] == 0)
                    {
                        ret = new GameMove(0, 2, 3, true);
                    }
                    else
                    {
                        ret = new GameMove(2, 2, 3, true);
                    }
                }
                if (startQuadrant == 1)
                {
                    if ((int)board[3, 0] == 0)
                    {
                        ret = new GameMove(3, 0, 0, true);
                    }
                    else if ((int)board[5, 2] == 0)
                    {
                        ret = new GameMove(5, 2, 2, true);
                    }
                    else
                    {
                        ret = new GameMove(3, 2, 2, true);
                    }
                }
                if (startQuadrant == 2)
                {
                    if ((int)board[2, 5] == 0)
                    {
                        ret = new GameMove(2, 5, 1, true);
                    }
                    else if ((int)board[0, 3] == 0)
                    {
                        ret = new GameMove(0, 3, 1, true);
                    }
                    else
                    {
                        ret = new GameMove(2, 3, 1, true);
                    }
                }
                if (startQuadrant == 3)
                {
                    if ((int)board[3, 5] == 0)
                    {
                        ret = new GameMove(3, 5, 0, true);
                    }
                    else if ((int)board[5, 3] == 0)
                    {
                        ret = new GameMove(5, 3, 0, true);
                    }
                    else
                    {
                        ret = new GameMove(3, 3, 0, true);
                    }
                }
            }
            //third turn
            else if (turnCounter == 3)
            {
                if (startQuadrant == 0)
                {
                    if ((int)board[0, 0] == 0)
                    {
                        /*
                        xVal = 0;
                        yVal = 0;
                        RotateSquare(board, 1, true);
                        */
                        ret = new GameMove(0, 0, 1, true);
                    }
                    else if ((int)board[0, 2] == 0)
                    {
                        /*
                        xVal = 0;
                        yVal = 2;
                        RotateSquare(board, 1, true);
                        */
                        ret = new GameMove(0, 2, 1, true);
                    }
                    else
                    {
                        /*
                        xVal = 0;
                        yVal = 5;
                        RotateSquare(board, 2, true);
                        */
                        ret = new GameMove(0, 5, 2, true);
                    }
                }
                if (startQuadrant == 1)
                {
                    if ((int)board[5, 0] == 0)
                    {
                        /*
                        xVal = 5;
                        yVal = 0;
                        RotateSquare(board, 2, true);
                        */
                        ret = new GameMove(5, 0, 2, true);
                    }
                    else if ((int)board[5, 2] == 0)
                    {
                        /*
                        xVal = 5;
                        yVal = 2;
                        RotateSquare(board, 2, true);
                        */
                        ret = new GameMove(5, 2, 2, true);
                    }
                    else
                    {
                        /*
                        xVal = 5;
                        yVal = 5;
                        RotateSquare(board, 3, false);
                        */
                        ret = new GameMove(5, 5, 3, false);
                    }
                }
                if (startQuadrant == 2)
                {
                    if ((int)board[0, 5] == 0)
                    {
                        /*
                        xVal = 0;
                        yVal = 5;
                        RotateSquare(board, 1, true);
                        */
                        ret = new GameMove(0, 5, 1, true);
                    }
                    else if ((int)board[0, 3] == 0)
                    {
                        /*
                        xVal = 5;
                        yVal = 2;
                        RotateSquare(board, 1, true);
                        */
                        ret = new GameMove(5, 2, 1, true);
                    }
                    else
                    {
                        /*
                        xVal = 0;
                        yVal = 0;
                        RotateSquare(board, 0, false);
                        */
                        ret = new GameMove(0, 0, 0, false);
                    }
                }
                if (startQuadrant == 3)
                {
                    if ((int)board[5, 5] == 0)
                    {
                        /*
                        xVal = 5;
                        yVal = 5;
                        RotateSquare(board, 2, true);
                        */
                        ret = new GameMove(5, 5, 2, true);
                    }
                    else if ((int)board[5, 3] == 0)
                    {
                        /*
                        xVal = 5;
                        yVal = 2;
                        RotateSquare(board, 2, true);
                        */
                        ret = new GameMove(5, 2, 2, true);
                    }
                    else
                    {
                        /*
                        xVal = 5;
                        yVal = 0;
                        RotateSquare(board, 3, true);
                        */
                        ret = new GameMove(5, 0, 3, true);
                    }
                }
            }
            //fourth turn
            else if (turnCounter == 4)
            {
                if (startQuadrant == 0 || startQuadrant == 3)
                {
                    //half of diagonal edge cases
                    if ((int)board[3, 2] + (int)board[2, 3] == 20)
                    {
                        if ((int)board[4, 1] == 10)
                        {
                            /*
                            xVal = 1;
                            yVal = 4;
                            RotateSquare(board, 2, true);
                            */
                            ret = new GameMove(1, 4, 2, true);
                        }
                        else if ((int)board[1, 4] == 10)
                        {
                            /*
                            xVal = 4;
                            yVal = 1;
                            RotateSquare(board, 1, true);
                            */
                            ret = new GameMove(4, 1, 1, true);
                        }
                    }
                    else if (startQuadrant == 0)
                    {
                        if ((int)board[2, 1] == 0)
                        {
                            /*
                            xVal = 2;
                            yval = 1;
                            RotateSquare(board, 3, true);
                            */
                            ret = new GameMove(2, 1, 3, true);
                        }
                        else if ((int)board[1, 0] == 0)
                        {
                            /*
                            xVal = 1;
                            yval = 0;
                            RotateSquare(board, 0, false);
                            */
                            ret = new GameMove(1, 0, 0, false);
                        }
                        else
                        {
                            /*
                            xVal = 2;
                            yval = 4;
                            RotateSquare(board, 0, false);
                            */
                            ret = new GameMove(2, 4, 0, false);
                        }
                    }
                    else
                    {
                        if ((int)board[3, 4] == 0)
                        {
                            /*
                            xVal = 3;
                            yval = 4;
                            RotateSquare(board, 0, true);
                            */
                            ret = new GameMove(3, 4, 0, true);
                        }
                        else if ((int)board[4, 5] == 0)
                        {
                            /*
                            xVal = 4;
                            yval = 5;
                            RotateSquare(board, 3, false);
                            */
                            ret = new GameMove(4, 5, 3, false);
                        }
                        else
                        {
                            /*
                            xVal = 3;
                            yval = 1;
                            RotateSquare(board, 0, true);
                            */
                            ret = new GameMove(3, 1, 0, true);
                        }
                    }
                }
                else
                {
                    //second half of diagonal edge cases
                    if ((int)board[2, 2] + (int)board[3, 3] == 20)
                    {
                        if ((int)board[4, 4] == 10)
                        {
                            /*
                            xVal = 1;
                            yVal = 1;
                            RotateSquare(board, 0, true);
                            */
                            ret = new GameMove(1, 1, 0, true);
                        }
                        else if ((int)board[1, 1] == 10)
                        {
                            /*
                            xVal = 4;
                            yVal = 4;
                            RotateSquare(board, 3, true);
                            */
                            ret = new GameMove(4, 4, 3, true);
                        }
                    }
                    else if (startQuadrant == 1)
                    {
                        if ((int)board[2, 1] == 0)
                        {
                            /*
                            xVal = 2;
                            yval = 1;
                            RotateSquare(board, 3, true);
                            */
                            ret = new GameMove(2, 1, 3, true);
                        }
                        else if ((int)board[1, 0] == 0)
                        {
                            /*
                            xVal = 1;
                            yval = 0;
                            RotateSquare(board, 0, false);
                            */
                            ret = new GameMove(1, 0, 0, false);
                        }
                        else
                        {
                            /*
                            xVal = 2;
                            yval = 4;
                            RotateSquare(board, 0, false);
                            */
                            ret = new GameMove(2, 4, 0, false);
                        }
                    }
                    else
                    {
                        if ((int)board[3, 4] == 0)
                        {
                            /*
                            xVal = 3;
                            yval = 4;
                            RotateSquare(board, 0, true);
                            */
                            ret = new GameMove(3, 4, 0, true);
                        }
                        else if ((int)board[4, 5] == 0)
                        {
                            /*
                            xVal = 4;
                            yval = 5;
                            RotateSquare(board, 3, false);
                            */
                            ret = new GameMove(4, 5, 3, false);
                        }
                        else
                        {
                            /*
                            xVal = 3;
                            yval = 1;
                            RotateSquare(board, 0, true);
                            */
                            ret = new GameMove(3, 1, 0, true);
                        }
                    }
                }
            }
            else
            {




                int[,] zerothQuad = { { 2, 0 }, { 2, 1 }, { 2, 2 }, { 1, 2 }, { 0, 2 } };
                int zerothInt = GetSumFromPoints(board, zerothQuad);
                int[,] firstQuad = { { 3, 0 }, { 3, 1 }, { 3, 2 }, { 4, 2 }, { 5, 2 } };
                int firstInt = GetSumFromPoints(board, firstQuad);
                int[,] secondQuad = { { 0, 3 }, { 1, 3 }, { 2, 3 }, { 2, 4 }, { 2, 5 } };
                int secondInt = GetSumFromPoints(board, secondQuad);
                int[,] thirdQuad = { { 5, 5 }, { 4, 5 }, { 3, 5 }, { 3, 4 }, { 3, 3 } };
                int thirdInt = GetSumFromPoints(board, thirdQuad);
                if (turnCounter < 11)
                {
                    if (4 < turnCounter && turnCounter < 11)
                    {
                        List<TupleList<int, int>> possibleWinPoints = new List<TupleList<int, int>>();
                        for (int i = 0; i < winValues.Length; i++)
                        {
                            if (winValues[i] >= 2 && winValues[i] <= 23)
                            {
                                possibleWinPoints.Add(PointsFromWinCondition(i));
                            }
                        }
                        foreach (var pointArr in possibleWinPoints)
                        {
                            foreach (var point in pointArr)
                            {
                                if (board[point.Item1, point.Item2] != TileVals.Blank)
                                {
                                    continue;
                                }
                                if (((-1 < point.Item1 && point.Item1 < 3) && (-1 < point.Item2 && point.Item2 < 3)) || ((2 < point.Item1 && point.Item1 < 6) && (2 < point.Item2 && point.Item2 < 6)))
                                {
                                    if (firstInt < secondInt)
                                    {
                                        ret = new GameMove(point.Item1, point.Item2, 1, false);
                                    }
                                    else
                                    {
                                        ret = new GameMove(point.Item1, point.Item2, 2, false);
                                    }
                                }
                                else if (((2 < point.Item1 && point.Item1 < 6) && (-1 < point.Item2 && point.Item2 < 3)) || ((-1 < point.Item1 && point.Item1 < 3) && (2 < point.Item2 && point.Item2 < 6)))
                                {
                                    if (zerothInt < thirdInt)
                                    {
                                        ret = new GameMove(point.Item1, point.Item2, 0, false);
                                    }
                                    else
                                    {
                                        ret = new GameMove(point.Item1, point.Item2, 3, false);
                                    }
                                }
                            }
                        }
                    }
                }
                //LATE GAME

                else
                {
                    List<TupleList<int, int>> possibleWinPoints = new List<TupleList<int, int>>();
                    for (int i = 0; i < winValues.Length; i++)
                    {
                        if (winValues[i] >= 23)
                        {
                            possibleWinPoints.Add(PointsFromWinCondition(i));
                        }
                    }
                    foreach (var pointArr in possibleWinPoints)
                    {
                        foreach (var point in pointArr)
                        {

                            if (((-1 < point.Item1 && point.Item1 < 3) && (-1 < point.Item2 && point.Item2 < 3)) || ((2 < point.Item1 && point.Item1 < 6) && (2 < point.Item2 && point.Item2 < 6)))
                            {
                                if (firstInt > secondInt)
                                {
                                    ret = new GameMove(point.Item1, point.Item2, 1, true);
                                }
                                else
                                {
                                    ret = new GameMove(point.Item1, point.Item2, 2, false);
                                }
                            }
                            else if (((2 < point.Item1 && point.Item1 < 6) && (-1 < point.Item2 && point.Item2 < 3)) || ((-1 < point.Item1 && point.Item1 < 3) && (2 < point.Item2 && point.Item2 < 6)))
                            {

                                if (zerothInt > thirdInt)
                                {
                                    ret = new GameMove(point.Item1, point.Item2, 0, false);
                                }
                                else
                                {
                                    ret = new GameMove(point.Item1, point.Item2, 3, false);
                                }
                            }
                        }
                    }
                }
                if (ret.rotIndex == -1)
                {
                    throw new Exception("Ret was never initalized in the heuristic");
                }

            }
            if (board[ret.xCord, ret.yCord] != TileVals.Blank)
            {
                throw new Exception("The AI overwrote a user tile...");
            }
            return ret;
        }


        enum TileVals
        {
            X = 1,
            O = 10,
            Blank = 0
        }

        static int GetSumFromPoints(TileVals[,] gameboard, int[,] pointsToSum)
        {
            int ret = 0;
            for (int i = 0; i < pointsToSum.GetLength(0); i++)
            {
                for (int j = 0; j < pointsToSum.GetLength(1); j++)
                {
                    ret += (int)gameboard[i, j];
                }
            }
            return ret;
        }

    }


    #region Classes for helping with win conditions
    public class CustomArray<T>
    {
        public static T[] GetColumn(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0) - 1)
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public static T[] GetRow(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1) - 1)
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        public static T[] GetColumnMinusLast(T[,] matrix, int columnNumber)
        {
            //return Enumerable.Range(0, matrix.GetLength(0) - 1)
            //.Select(x => matrix[x, columnNumber])
            //.ToArray();
            var colLength = matrix.GetLength(0) - 1;
            var colVector = new T[colLength];

            for (var i = 0; i < colLength; i++)
                colVector[i] = matrix[columnNumber, i];

            return colVector;
        }

        public static T[] GetRowMinusLast(T[,] matrix, int rowNumber)
        {
            //return Enumerable.Range(0, matrix.GetLength(1) - 1)
            //.Select(x => matrix[rowNumber, x])
            //.ToArray();
            var rowLength = matrix.GetLength(1) - 1;
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[i, rowNumber];

            return rowVector;
        }


        public static T[] GetColumnMinusFirst(T[,] matrix, int columnNumber)
        {
            //return Enumerable.Range(1, matrix.GetLength(0) - 1)
            //.Select(x => matrix[x, columnNumber])
            //.ToArray();
            var colLength = matrix.GetLength(0) - 1;
            var colVector = new T[colLength];

            for (var i = 0; i < colLength; i++)
                colVector[i] = matrix[columnNumber, i + 1];

            return colVector;
        }

        public static T[] GetRowMinusFirst(T[,] matrix, int rowNumber)
        {
            //return Enumerable.Range(1, matrix.GetLength(1) - 1)
            //.Select(x => matrix[rowNumber, x])
            //.ToArray();
            var rowLength = matrix.GetLength(1) - 1;
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[i + 1, rowNumber];

            return rowVector;
        }
    }

    public class TupleList<T1, T2> : List<MyTuple<T1, T2>>
    {
        public TupleList()
        {
        }
        public TupleList(T1 one, T2 two)
        {
            Add(one, two);
        }
        public void Add(T1 item, T2 item2)
        {
            Add(new MyTuple<T1, T2>(item, item2));
        }
    }

    public class MyTuple<T1, T2>
    {
        private T1 _item1;
        private T2 _item2;
        public MyTuple(T1 item1, T2 item2)
        {
            _item1 = item1;
            _item2 = item2;
        }

        public T1 Item1 { get => _item1; set => _item1 = value; }
        public T2 Item2 { get => _item2; set => _item2 = value; }

        public void Add(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
    }

    public class GameMove
    {
        public int xCord;
        public int yCord;
        public int rotIndex;
        public bool rotLeft;

        public GameMove()
        {
        }

        public GameMove(int xCord, int yCord, int rotIndex, bool rotLeft)
        {
            this.xCord = xCord;
            this.yCord = yCord;
            this.rotIndex = rotIndex;
            this.rotLeft = rotLeft;
        }
    }


    public class RunCmd
    {
        public static string Run(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    return result;
                }
            }
        }

        public static GameMove RunPythonThing()
        {
            var res = Run("/Users/connorgoldsmith/Desktop/python-test/pytest.py", "hello there");
            Console.WriteLine(res);
            try
            {
                string[] rets = new string[4];
                using (System.IO.StringReader reader = new System.IO.StringReader(res))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string line = reader.ReadLine();
                        rets[i] = line;
                    }

                }
                GameMove g = new GameMove
                (
                    Int32.Parse(rets[0]),
                    Int32.Parse(rets[1]),
                    Int32.Parse(rets[2]),
                    rets[3] == "1"
                    );
                return g;
            }
            catch (Exception ex)
            {
                throw new Exception("NN did not return correct format");
            }

        }
    }
}
#endregion