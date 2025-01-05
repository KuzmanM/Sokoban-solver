using SokobanSolver.DataModel;
using System;

namespace SokobanSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            
            try
            {
                // Show description and read user input
                BoardDrawResult def = BoardParser.ShowLegendAndReadBordDraw();

                // Validate user input
                BoardParser.Vlidate(def.BoardDefinition, def.PlayerPositionX, def.PlayerPositionY);

                // Found the solution
                Board board = new Board(def.BoardDefinition, def.PlayerPositionX, def.PlayerPositionY);
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
            
            TimeSpan t = (DateTime.Now - start);
            if (t.TotalSeconds < 1000)
                Console.WriteLine($"Time: {Math.Round(t.TotalSeconds, 1)} seconds");
            else if (t.TotalMinutes < 500)
                Console.WriteLine($"Time: {Math.Round(t.TotalMinutes, 2)} minutes");
            else
                Console.WriteLine($"Time: {Math.Round(t.TotalHours, 3)} hours");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
