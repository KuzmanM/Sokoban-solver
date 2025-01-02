using SokobanSolver.DataModel;
using System;

namespace SokobanSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Show description and read user input
                BoardDrawResult def = BoardParser.ShowLegendAndReadBordDraw();

                // Validate user input
                BoardParser.Vlidate(def.BoardDfinition, def.PlayerPositionX, def.PlayerPositionY);

                // Found the solution
                Board board = new Board(def.BoardDfinition, def.PlayerPositionX, def.PlayerPositionY);
                Board end = board.SetNext();

                // Print the result
                BoardParser.PrintResult(end);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
