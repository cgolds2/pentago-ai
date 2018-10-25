using System;

namespace Pentago_console
{
    class Program
    {
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
        X = -1,
        O = 1,
        Blank = 0
    }
}
