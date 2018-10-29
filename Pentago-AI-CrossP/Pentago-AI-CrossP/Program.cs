using System;
namespace PentagoAICrossP {
	class Program {
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
		static TileVals[,] rotateBoard(TileVals[,] board, int n) {
			TileVals[,] ret = new TileVals[n, n];
			for (int i = 0; i < n; i++) {
				for (int j = 0; j < n; j++) {
					ret[i, j] = board[n - j - 1, i];
				}
			}
			return ret;
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
				//rotation
				int baseForIndex = 0;
				if (square > 1) {
					baseForIndex += 18;
				}
				if (square == 1 || square == 3) {
					baseForIndex += 3;
				}
				TileVals[] tempTiles = new TileVals[9];
				for (int i = 0; i < 9; i++) {
					int fromSpot = rotIndex[i] + baseForIndex;
					int fromX = fromSpot % 6;
					int fromY = fromSpot / 6;
					tempTiles[i] = gameBoard[fromX, fromY];
				}
				if (rot == "left" || rot == "Left" || rot == "l" || rot == "L") {
					for (int i = 0; i < 9; i++) {
						int x = (leftRotIndex[i] + baseForIndex) % 6;
						int y = (leftRotIndex[i] + baseForIndex) / 6;
						gameBoard[x, y] = tempTiles[i];
					}
				} else {
					for (int i = 0; i < 9; i++) {
						int x = (rightRotIndex[i] + baseForIndex) % 6;
						int y = (rightRotIndex[i] + baseForIndex) / 6;
						gameBoard[x, y] = tempTiles[i];

					}
				}
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
	}
	enum TileVals {
		X = 1,
		O = 10,
		Blank = 0
	}
}