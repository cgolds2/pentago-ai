using System;
using System.Collections.Generic;

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

					}
					Console.Write("|" + TileToString(board[j, i]));
				}
				Console.WriteLine("|");
				if (i == 2) {
					Console.WriteLine("");
				}
			}
			Console.WriteLine("\n--------------");
			Console.WriteLine((isXTurn ? "Player 1" : "Player 2") + "'s turn");
			Console.WriteLine("--------------\n");
		}
		static int TryGetInt(string prompt, int min, int max) {
			int ret;
			Console.WriteLine("Enter " + prompt);
			while (true) {
				bool successfullyParsed = int.TryParse(Console.ReadLine(), out ret);
				if (successfullyParsed) {
					if (ret >= min && ret <= max) {
						break;
					}
				}
				Console.WriteLine("Enter valid " + prompt);
			}
			Console.WriteLine("You entered " + ret);
			return ret;
		}
		static string TileToString(TileVals t) {
			switch (t) {
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
		static bool isOver(TileVals[,] board) {
			return false;
		}
		static int[,] pentagoHeuristic(board)
		{
			int[,] ret;
			//first turn
			if (turnCounter == 1) {
				board[xVal, yVal] == TileVals.X;
				Random startQuadrant = new Random()
				for (int i = 0; i < 1; i++)
				{
					startQuadrant.Next(0,4) = startQuadrant;
				}
				if (startQuadrant == 0) {
					xVal = 2;
					yVal = 2;
					//rotateBoard(board, 3);
				}
				else if (startQuadrant == 1) {
					xVal = 3;
					yVal = 2;
					//rotateBoard(board, 2);
				}
				else if (startQuadrant == 2) {
					xVal = 2;
					yVal = 3;
					//rotateBoard(board, 1);
				}
				else if (startQuadrant == 3) {
					xVal = 3;
					yVal = 3;
					//rotateBoard(board, 0);
				}
			}
			//second turn minus rotations
			if (turnCounter == 2) {
				if (startQuadrant == 0) {
					if ((int)board[2,0] == 0) {
						xVal = 2;
						yVal = 0;

					}
					else {
						xVal = 0;
						yVal = 2;
					}
				}
				if (startQuadrant == 1) {
					if ((int)board[3,0] == 0) {
						xVal = 3;
						yVal = 0;
					}
					else {
						xVal = 5;
						yVal = 2;
					}
				}
				if (startQuadrant == 2) {
					if ((int)board[2,5] == 0) {
						xVal = 2;
						yVal = 5;
					}
					else {
						xVal = 0;
						yVal = 3;
					}
				}
				if (startQuadrant == 3) {
					if ((int)board[3,5] == 0) {
						xVal = 3;
						yVal = 5;
					}
					else {
						xVal = 5;
						yVal = 3;
					}
				}
			}
			// /*
			if (turnCounter == 3) {

			}
			if (turnCounter == 4) {
				if (startQuadrant == 0 || startQuadrant == 3) {
					//half of diagonal edge cases
					if ((int)board[3,2] + (int)board[2,3] == 20) {
						if ((int)board[4,1] == 10) {
							xVal = 1;
							yVal = 4;
						}
						else if ((int)board[1,4] == 10) {
							xVal = 4;
							yVal = 1;
						}
					}
					else {

					}
				}
				else {
					//second half of diagonal edge cases
					if ((int)board[2,2] + (int)board[3,3] == 20) {
						if ((int)board[4,4] == 10) {
							xVal = 1;
							yVal = 1;
						}
						else if ((int)board[1,1] == 10) {
							xVal = 4;
							yVal = 4;
						}
					}
				}
			}
			// */
		}
		
	}
	enum TileVals {
		X = 1,
		O = 10,
		Blank = 0
	}
}