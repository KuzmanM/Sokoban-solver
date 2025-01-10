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
                (BoardDrawResult board, string commandParam) input = BoardParser.ShowLegendAndReadBordDraw();

                // Validate user input
                BoardParser.Vlidate(input.board.BoardDefinition, input.board.PlayerPositionX, input.board.PlayerPositionY);

                // Found the solution
                Board board = new Board(input.board.BoardDefinition, input.board.PlayerPositionX, input.board.PlayerPositionY);
                Board end;
                if (input.commandParam.Equals("bf", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("breadth-first search");
                    Console.WriteLine();
                    end = Board.BuildBFSolutionTree(board);
                }
                else
                {
                    Console.WriteLine("depth-first search");
                    Console.WriteLine();
                    end = Board.BuildDFSolutionTree(board);
                }

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
            
            double totalSeconds = (DateTime.Now - start).TotalSeconds;
            uint allSeconds = Convert.ToUInt32(totalSeconds);
            uint allMinutes = allSeconds / 60;
            uint secondsPart = allSeconds % 60;
            uint hoursPart = allMinutes / 60;
            uint minutesPart = allMinutes % 60;
            Console.WriteLine($"Time (h:min:sec): {hoursPart:D2}:{minutesPart:D2}:{secondsPart:D2}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
