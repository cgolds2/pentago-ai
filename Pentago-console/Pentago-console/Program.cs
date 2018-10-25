using System;

namespace Pentago_console
{
    class Program
    {
        static bool isOver(TileVals[,] board)
        {
            for (int i = 0; i < 6; i++)
            {
                int val = 0;
                for (int j = 0; j < 6; j++)
                {
                    val += board[i, j];
                }
                if (val == 5) { return true; }
            }
            for (int i = 1; i < 7; i++)
            {
                int val = 0;
                for (int j = 1; j < 7; j++)
                {
                    val += board[i, j];
                }
                if (val == 5) { return true; }
            }
            for (int i = 0; i < 6; i++)
            {
                int val = 0;
                for (int j = 0; j < 6; j++)
                {
                    val += board[j, i];
                }
                if (val == 5) { return true; }
            }
            for (int i = 1; i < 7; i++)
            {
                int val = 0;
                for (int j = 1; j < 7; j++)
                {
                    val += board[j, i];
                }
                if (val == 5) { return true; }
            }
            
        }
        static void Main(string[] args)
        {
            TileVals[,] gameBoard = new TileVals[6, 6];
            bool isXTurn = false;
            while (true)
            {
                isXTurn = !isXTurn;
                var xVal = -1;
                var yVal = -1;
                while (true)
                {
                    Console.WriteLine("Enter x val of square");
                   
                    int.TryParse(Console.ReadLine(), out xVal);
                    while (xVal < 0 || xVal > 5)
                    {
                        Console.WriteLine("x not valid, reenter x");
                        int.TryParse(Console.ReadLine(), out xVal);
                    }
                    Console.WriteLine("You entered " + xVal);


                    Console.WriteLine("Enter y val of square");
                   
                    int.TryParse(Console.ReadLine(), out yVal);
                    while (yVal < 0 || yVal > 5)
                    {
                        Console.WriteLine("y not valid, reenter y");
                        int.TryParse(Console.ReadLine(), out xVal);
                    }
                    Console.WriteLine("You entered " + yVal);
                    if(gameBoard[xVal, yVal] != TileVals.Blank)
                    {
                        Console.WriteLine("Square already taken\n");
                    }
                    else
                    {
                        break;
                    }
                }
                gameBoard[xVal, yVal] = isXTurn ? TileVals.X : TileVals.O;
                printBoard(gameBoard);
                Console.WriteLine("---------------");
                int square = -1;
                Console.WriteLine("Enter index of square to rotate");

                int.TryParse(Console.ReadLine(), out square);
                while (square < 0 || square > 3)
                {
                    Console.WriteLine("square not valid, reenter square");
                    int.TryParse(Console.ReadLine(), out square);
                }
                Console.WriteLine("You entered " + xVal);


                string rot = "";
                Console.WriteLine("Enter left or right for rotation");
                rot = Console.ReadLine();
                while (!(rot == "left" || rot == "right"))
                {
                    Console.WriteLine("rotate not valid");
                    rot = Console.ReadLine();
                }
                Console.WriteLine("You entered " + rot);

                //rotation
                int baseForIndex = 0;
                if(square > 1)
                {
                    baseForIndex += 18;
                }
                if(square == 1 || square == 3)
                {
                    baseForIndex += 3;
                }

                int[] rotIndex = {
                    0, 1, 2,
                    6, 7, 8,
                    12, 13, 14
                };

                int[] leftRotIndex = {
                    12, 6, 0,
                    13, 7, 1,
                    14, 8, 2
                };

                int[] rightRotIndex = {
                    2, 8, 14,
                    1, 7, 13,
                    0, 6, 12
                };
             
                TileVals[] tempTiles = new TileVals[9];

                for (int i = 0; i < 9; i++)
                {
                    int fromSpot = rotIndex[i] + baseForIndex;
                    int fromX = fromSpot % 6;
                    int fromY = fromSpot / 6;
                    tempTiles[i] = gameBoard[fromX, fromY];
                }
                if (rot == "left")
                {
                    for (int i = 0; i < 9; i++)
                    {
                        int x = (leftRotIndex[i] + baseForIndex) % 6;
                        int y = (leftRotIndex[i] + baseForIndex) / 6;
                        gameBoard[x, y] = tempTiles[i];
                    }
                }
                else
                {
                    for (int i = 0; i < 9; i++)
                    {
                        int x = (rightRotIndex[i] + baseForIndex) % 6;
                        int y = (rightRotIndex[i] + baseForIndex) / 6;
                        gameBoard[x, y] = tempTiles[i];

                    }
                }
           


                printBoard(gameBoard);
                if (didItWin(gameBoard))
                {
                    break;
                }

              
            }
            
           
           
        }
        static void printBoard(TileVals[,] board)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
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
        static bool didItWin(TileVals[,] board)
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
