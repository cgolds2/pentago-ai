﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PentagoAICrossP {
	class Program {
		static int[] lastTurn = new int[85];
		static List<int[]> turns = new List<int[]>();
		static readonly int[] rotIndex = {
										0, 1, 2,
										6, 7, 8,
										12, 13, 14
								};
		static readonly int[] leftRotIndex = {
										12, 6, 0,
										13, 7, 1,
										14, 8, 2
								};
		static readonly int[] rightRotIndex = {
										2, 8, 14,
										1, 7, 13,
										0, 6, 12
								};
		static bool isXTurn = false;
        static int turnCounter = 0;
	
        static TileVals[,] rotateBoard(TileVals[,] board, int n) {
			TileVals[,] ret = new TileVals[n, n];
			for (int i = 0; i < n; i++) {
				for (int j = 0; j < n; j++) {
					ret[i, j] = board[n - j - 1, i];
				}
			}
			return ret;
		}
		static List<int[]> getTurns() {
			return turns;
		}

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

        static void addTurn(int[] t) {
			if (t.Length != 85) {
				throw new ArgumentException("Array did not have 85 elements");
			}
			turns.Add(t);
		}
		static void trackPlacement(int x, int y) {
			lastTurn[x * 6 + y] = 1;
		}
		static void trackRotation(int x, String y) {
			if (y == "right" || y == "Right" || y == "r" || y == "R")
				lastTurn[84 - (x * 2)] = 1;
			else
				lastTurn[84 - ((x * 2) + 1)] = 1;
		}
		static void updateTurn() {
			addTurn(lastTurn);
			lastTurn = new int[85];
		}
		static bool IsOver(TileVals[,] board) {
			for (int i = 0; i < 5; i++) {
				int val = 0;
				for (int j = 0; j < 5; j++) {
					val += (int)board[i, j];
				}
				if (val == 5) { return true; }
			}
			for (int i = 1; i < 6; i++) {
				int val = 0;
				for (int j = 1; j < 6; j++) {
					val += (int)board[i, j];
				}
				if (val == 5) { return true; }
			}
			for (int i = 0; i < 5; i++) {
				int val = 0;
				for (int j = 0; j < 5; j++) {
					val += (int)board[j, i];
				}
				if (val == 5) { return true; }
			}
			for (int i = 1; i < 6; i++) {
				int val = 0;
				for (int j = 1; j < 6; j++) {
					val += (int)board[j, i];
				}
				if (val == 5) { return true; }
			}
			int x = 0;
			for (int k = 0; k < 4; k++) {
				x = (int)board[0, 0] + (int)board[1, 1] + (int)board[2, 2] + (int)board[3, 3] + (int)board[4, 4];
				if (x == 5 || x == 50) { return true; }
				x = (int)board[1, 1] + (int)board[2, 2] + (int)board[3, 3] + (int)board[4, 4] + (int)board[5, 5];
				if (x == 5 || x == 50) { return true; }
				x = (int)board[0, 1] + (int)board[1, 2] + (int)board[2, 3] + (int)board[3, 4] + (int)board[4, 5];
				if (x == 5 || x == 50) { return true; }
				x = (int)board[1, 0] + (int)board[2, 1] + (int)board[3, 2] + (int)board[4, 3] + (int)board[5, 4];
				board = rotateBoard(board, 6);
			}
			return false;
		}
		static void Main(string[] args) {
			TileVals[,] gameBoard = new TileVals[6, 6];
			printBoard(gameBoard);
			while (true) {
				isXTurn = !isXTurn;
                if(isXTurn){
                    turnCounter++;
                }
				printBoard(gameBoard);
				var xVal = -1;
				var yVal = -1;
				while (true) {
					xVal = TryGetInt("x value", 0, 5);
					yVal = TryGetInt("y value", 0, 5);
					if (gameBoard[xVal, yVal] != TileVals.Blank) {
						printBoard(gameBoard);
						Console.WriteLine("Square already taken\n");
					} else {
						break;
					}
				}

				gameBoard[xVal, yVal] = isXTurn ? TileVals.X : TileVals.O;
				trackPlacement(xVal, yVal);
				printBoard(gameBoard);
				int square = TryGetInt("index of square to rotate:\n0 1\n2 3", 0, 3);
				string rot = "";
				Console.WriteLine("Enter (L)eft or (R)ight for rotation");
				rot = Console.ReadLine();
				while (!(rot == "right" ||
								 rot == "left" ||
								 rot == "Right" ||
								 rot == "Left" ||
								 rot == "r" ||
								 rot == "l" ||
								 rot == "R" ||
								 rot == "L")) {
					Console.WriteLine("rotate not valid");
					rot = Console.ReadLine();
				}

				Console.WriteLine("You entered " + rot);
				trackRotation(square, rot);
				//Console.WriteLine("Last turn:");
				//foreach (var item in lastTurn) {
				//	Console.Write(item.ToString());
				//}
				//Console.WriteLine();
				updateTurn();
				//rotation
                RotateSquare(gameBoard, square, rot.ToLower() == "left" || rot.ToLower() == "l");
			
				if (IsOver(gameBoard)) {
					printBoard(gameBoard);
					Console.Write("Game Over.\n");
					break;
				}
			}
		}
		static void printBoard(TileVals[,] board) {
			Console.Clear();
			Console.WriteLine("   0 1 2     3 4 5");
			for (int i = 0; i < 6; i++) {
				for (int j = 0; j < 6; j++) {
					if (j == 0) {
						Console.Write(i + " ");
					}
					if (j == 3) {
						Console.Write("|   ");

        //keeps track of which player is allowed to move
        static bool isXTurn = false;

        static int[] winValues = new int[32];
        /*
         * 00-11: horizontal
         * 12-23: verticle
         * 24-27: top left bot right diag
         * 28-31: top right bot left diag
         */

        static void UpdateBoard(TileVals[,] board)
        {
            //update horizontal (6)
            //update verticle (6)
            //update diag (4)
            //update one othe diag (1)


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

                Console.WriteLine("Y1: " + xSum);
                Console.WriteLine("Y2: " + ySum);
            }

        }
        static void UpdateDiagonal(TileVals[,] board, int quad)
        {
            if (quad == 0 || quad == 4)
            {
                List<TupleList<int, int>> diags = new List<TupleList<int, int>>();

                var oneDiag = new TupleList<int, int>
                    {
                      { 1,1},{ 5, 1 },{ 3, 1 },{ 1,1 },{ 1,1 }
                    };
                var twoDiag = new TupleList<int, int>
                    {
                      { 1,1},{ 5, 1 },{ 3, 1 },{ 1,1 },{ 1,1 }
                    };
                var threeDiag = new TupleList<int, int>
                    {
                      { 1,1},{ 5, 1 },{ 3, 1 },{ 1,1 },{ 1,1 }
                    };
                var fourDiag = new TupleList<int, int>
                    {
                      { 1,1},{ 5, 1 },{ 3, 1 },{ 1,1 },{ 1,1 }
                    };


                diags.Add(oneDiag);
                diags.Add(twoDiag);
                diags.Add(threeDiag);
                diags.Add(fourDiag);

            }
            else
            {

            }
        }
        static void UpdateOther(TileVals[,] board, int quad)
        {
            switch (quad)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
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
        return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public static T[] GetRow(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

    public static T[] GetColumnMinusLast(T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0) - 1)
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public static T[] GetRowMinusLast(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1) - 1)
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

    public static T[] GetColumnMinusFirst(T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(1, matrix.GetLength(0) - 1)
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public static T[] GetRowMinusFirst(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(1, matrix.GetLength(1) - 1)
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }
}

public class TupleList<T1, T2> : List<Tuple<T1, T2>>
{
    public void Add(T1 item, T2 item2)
    {
        Add(new Tuple<T1, T2>(item, item2));
    }
}