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
            Console.WriteLine("Enter a number");
            var a = int.Parse(Console.ReadLine());
            Console.WriteLine("You entered: " + a.ToString());
            Console.ReadLine();
        }
    }
    enum TileVals
    {
        X = 1,
        O = 10,
        Blank = 0
    }
}
