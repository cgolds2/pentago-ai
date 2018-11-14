using System;
using System.Collections.Generic;
using System.Linq;

namespace PentagoAICrossP
{
    class Program
    {
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

        static void UpdateBoard(TileVals[,] board, int quad)
        {
            //update horizontal (6)
            //update verticle (6)
            //update diag (4)
            //update one othe diag (1)
            printBoard(board);
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

                winValues[i*2] = xSum;
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
                    winValues[24 + i] = sumDiag(board, diags[i]);
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
                    winValues[28 + i] = sumDiag(board, diags[i]);
                }
            }

        }

        static int sumDiag(TileVals[,] board, TupleList<int, int> diags)
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
                x = leftToRight ? x++ : x--;
                y--;
                diag.Add(x, y);
            }
            return diag;

        }
        static void UpdateOther(TileVals[,] board, int quad)
        {
            switch (quad)
            {
                case 0:
                    winValues[28] = sumDiag(board, DiagFromPoint(4, 0, false));
                    break;
                case 1:
                    winValues[24+3] = sumDiag(board, DiagFromPoint(1, 0, true));

                    break;
                case 2:
                    winValues[24+0] = sumDiag(board, DiagFromPoint(0, 1, true));

                    break;
                case 3:
                    winValues[28+3] = sumDiag(board, DiagFromPoint(5, 1, false));
                    break;
                default:
                    throw new ArgumentException("quad greater than 3");
            }
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



        //screams
        static bool IsOver(TileVals[,] board)
        {
            for (int i = 0; i < 5; i++)
            {
                int val = 0;
                for (int j = 0; j < 5; j++)
                {
                    val += (int)board[i, j];
                }
                if (val == 5) { return true; }
            }
            for (int i = 1; i < 6; i++)
            {
                int val = 0;
                for (int j = 1; j < 6; j++)
                {
                    val += (int)board[i, j];
                }
                if (val == 5) { return true; }
            }
            for (int i = 0; i < 5; i++)
            {
                int val = 0;
                for (int j = 0; j < 5; j++)
                {
                    val += (int)board[j, i];
                }
                if (val == 5) { return true; }
            }
            for (int i = 1; i < 6; i++)
            {
                int val = 0;
                for (int j = 1; j < 6; j++)
                {
                    val += (int)board[j, i];
                }
                if (val == 5) { return true; }
            }
            int x = 0;
            for (int k = 0; k < 4; k++)
            {
                x = (int)board[0, 0] + (int)board[1, 1] + (int)board[2, 2] + (int)board[3, 3] + (int)board[4, 4];
                if (x == 5 || x == 50) { return true; }
                x = (int)board[1, 1] + (int)board[2, 2] + (int)board[3, 3] + (int)board[4, 4] + (int)board[5, 5];
                if (x == 5 || x == 50) { return true; }
                x = (int)board[0, 1] + (int)board[1, 2] + (int)board[2, 3] + (int)board[3, 4] + (int)board[4, 5];
                if (x == 5 || x == 50) { return true; }
                x = (int)board[1, 0] + (int)board[2, 1] + (int)board[3, 2] + (int)board[4, 3] + (int)board[5, 4];
                board = RotateBoard(board, 6);
            }
            return false;
        }


        static void Main(string[] args)
        {
            TileVals[,] gameBoard = new TileVals[6, 6];
            printBoard(gameBoard);

            //main game loop
            while (true)
            {
                //change turn
                isXTurn = !isXTurn;
                printBoard(gameBoard);

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
                        printBoard(gameBoard);
                        Console.WriteLine("Square already taken\n");
                    }
                    else
                    {
                        break;
                    }
                }
                //place an X or O depending on whos turn it is
                gameBoard[xVal, yVal] = isXTurn ? TileVals.X : TileVals.O;
                printBoard(gameBoard);
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

                for (int i = 0; i < 3; i++)
                {
                    UpdateHorizontal(gameBoard, i);

                }
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
                UpdateBoard(gameBoard, square);
                if (IsOver(gameBoard))
                {
                    printBoard(gameBoard);
                    Console.Write("Game Over.\n");
                    break;
                }
            }
        }
        static void printBoard(TileVals[,] board)
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
        static bool isOver(TileVals[,] board)
        {
            return false;
        }
    }
    enum TileVals
    {
        X = 1,
        O = 10,
        Blank = 0
    }


}

public class CustomArray<T>
{
    public static T[] GetColumn(T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0)-1)
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public static T[] GetRow(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1)-1)
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

    public static T[] GetColumnMinusLast(T[,] matrix, int columnNumber)
    {
        //return Enumerable.Range(0, matrix.GetLength(0) - 1)
        //.Select(x => matrix[x, columnNumber])
        //.ToArray();
        var colLength = matrix.GetLength(0)-1;
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
        var rowLength = matrix.GetLength(1)-1;
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
        var colLength = matrix.GetLength(0)-1;
        var colVector = new T[colLength];

        for (var i = 0; i < colLength; i++)
            colVector[i] = matrix[columnNumber, i+1];

        return colVector;
    }

    public static T[] GetRowMinusFirst(T[,] matrix, int rowNumber)
    {
        //return Enumerable.Range(1, matrix.GetLength(1) - 1)
        //.Select(x => matrix[rowNumber, x])
        //.ToArray();
        var rowLength = matrix.GetLength(1)-1;
        var rowVector = new T[rowLength];

        for (var i = 0; i < rowLength; i++)
            rowVector[i] = matrix[i+1, rowNumber];

        return rowVector;
    }
}

public class TupleList<T1, T2> : List<Tuple<T1, T2>>
{
    public void Add(T1 item, T2 item2)
    {
        Add(new Tuple<T1, T2>(item, item2));
    }
}