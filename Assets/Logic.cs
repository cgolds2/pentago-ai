using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PentagoAICrossP
{
    public class Logic : MonoBehaviour
    {
        static int[] lastTurn = new int[85];
        static List<int[]> turns = new List<int[]>();
        static int turnCounter = 0;
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
                lastTurn[84 - (x * 2)] = 1;
            else
                lastTurn[84 - ((x * 2) + 1)] = 1;
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
                TileVals[] yTiles = CustomArray<TileVals>.GetColumnMinusFirst(board, y);
                var ySum = Array.ConvertAll(yTiles, value => (int)value).Sum();
                winValues[12 + x * 2] = ySum;
            }
            if (y != 5)
            {
                //update top to botmost
                TileVals[] yTiles = CustomArray<TileVals>.GetColumnMinusLast(board, y);
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
            if (quad == 0 || quad == 4)
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
        public static void callUpdate()
        {
            TileVals[,] gameBoard = new TileVals[6, 6];
            PrintBoard(gameBoard);

            //main game loop
            //while (true)
            //{
                //change turn
                isXTurn = !isXTurn;
                PrintBoard(gameBoard);

                //set vals to illegal by default
                var xVal = GameController.xVal;
                var yVal = GameController.yVal;

                while (true)
                {
                    //make sure the value selected is actually open
                    if (gameBoard[xVal, yVal] != TileVals.Blank)
                    {
                        //PrintBoard(gameBoard);
                        //Console.WriteLine("Square already taken\n");
                        return;
                    }
                    else
                    {
                        break;
                    }
                }
                
                //place an X or O depending on whos turn it is
                gameBoard[xVal, yVal] = isXTurn ? TileVals.X : TileVals.O;
                UpdatePoint(gameBoard, xVal, yVal);
                TrackPlacement(xVal, yVal);
                int square = GameController.lastBlockNumber;
                string rot = "";
                //Console.WriteLine("Enter (L)eft or (R)ight for rotation");
                //rot = Console.ReadLine();

                rot = GameController.lastRotation;
                //list of valid values for rotation
                var rotationInput = new List<string> { "right", "left", "r", "l" };
                
                //break;
                //while (!rotationInput.Contains(rot.ToLower()))
                //{
                //    Console.WriteLine("rotate not valid");
                //    rot = Console.ReadLine();
                //}
                //Debug.Log("You entered " + rot + " at block: " + GameController.lastBlockNumber);
                TrackRotation(square, rot);
                //rotation
                int baseForIndex = 0;

                //if its the lower 2 squares
                if (square > 1)
                {
                    baseForIndex += 18;
                }

                //if its the right 2 squeares
                if (square == 1 || square == 3)
                {
                    baseForIndex += 3;
                }

                //initilize a temp square to map to and 
                //dup the values to it
                TileVals[] tempTiles = new TileVals[9];
                for (int i = 0; i < 9; i++)
                {
                    int fromSpot = rotIndex[i] + baseForIndex;
                    int fromX = fromSpot % 6;
                    int fromY = fromSpot / 6;
                    tempTiles[i] = gameBoard[fromX, fromY];
                }
                if (rot == "left" || rot == "Left" || rot == "l" || rot == "L")
                {
                    //rotate board left
                    for (int i = 0; i < 9; i++)
                    {
                        int x = (leftRotIndex[i] + baseForIndex) % 6;
                        int y = (leftRotIndex[i] + baseForIndex) / 6;
                        gameBoard[x, y] = tempTiles[i];

                    }
                }
                else
                {
                    //rotate board right
                    for (int i = 0; i < 9; i++)
                    {
                        int x = (rightRotIndex[i] + baseForIndex) % 6;
                        int y = (rightRotIndex[i] + baseForIndex) / 6;
                        gameBoard[x, y] = tempTiles[i];

                    }
                }
                UpdateRotation(gameBoard, square);
                UpdateTurn();
                if (IsGameWon(gameBoard))
                {
                    PrintBoard(gameBoard);
                    Debug.Log("Game Over.");
                    //break;
                }
            //}
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
                Debug.Log(item);
                if (item == 50)
                {
                    didOWin = true;
                    Debug.Log("O won");
                }
                if (item == 5)
                {
                    didXWin = true;
                    Debug.Log("X won");
                }
            }

            return didXWin || didOWin;
        }
        static void PrintBoard(TileVals[,] board)
        {
            //Console.Clear();
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
    }


    enum TileVals
    {
        X = 1,
        O = 10,
        Blank = 0
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

public class TupleList<T1, T2> : List<MyTuple<int, int>>
{
    public void Add(int item, int item2)
    {
        Add(new MyTuple<int, int>(item, item2));
    }
}

public class MyTuple<T1, T2>
{
    public int Item1;
    public int Item2;
    public MyTuple(int item1, int item2)
    {
        this.Item1 = item1;
        this.Item2 = item2;
    }


    public void Add(int item1, int item2)
    {
        this.Item1 += item1;
        this.Item2 += item2;
    }
}
#endregion