using SokobanSolver.DataModel;
using System;
using System.Collections.Generic;

namespace SokobanSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            BoardDrawResult def = BoardParser.ShowLegendAndReadBordDraw();
            Board board = new Board(def.BoardDfinition, def.PlayerPositionX, def.PlayerPositionY);
            Board end = board.SetNext();
            BoardParser.PrintResult(end);
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
