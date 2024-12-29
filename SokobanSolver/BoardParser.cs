using SokobanSolver.DataModel;
using System;
using System.Collections.Generic;

namespace SokobanSolver
{
    public static class BoardParser
    {
        public static BoardDrawResult ShowLegendAndReadBordDraw()
        {
            Console.WriteLine("  - Empty space");
            Console.WriteLine("# - Wall");
            Console.WriteLine("* - Target");
            Console.WriteLine("b - Box");
            Console.WriteLine("@ - Box on target");
            Console.WriteLine("& - Player");
            Console.WriteLine();
            Console.WriteLine("\"Draw\" the board and write 'go' to complete!");
            Console.WriteLine("\"Draw\" will be converted to coordinates with 1 based index. Zero point is upper left point of the Draw.");
            Console.WriteLine("Result movments are also 1 based indexes!");
            Console.WriteLine();

            bool exit;
            ushort charCounter = 1;//X
            ushort lineCounter = 1;//Y
            List<BoardPositionContent> boardDefinition = new List<BoardPositionContent>();
            ushort playerX = 0;
            ushort playerY = 0;
            do
            {
                string line = Console.ReadLine().TrimEnd();
                exit = line.Trim().Equals("go", StringComparison.OrdinalIgnoreCase);
                if (!exit)
                {
                    //NOTE: Leading spaces (before first character), have to be replaced with Not In Board.
                    //replace leading spaces with dots.
                    int lineLength = line.Length;
                    line = line.TrimStart();
                    line = line.PadLeft(lineLength, '.');

                    foreach (char boardPart in line)
                    {
                        if (boardPart == '&')
                        {
                            playerX = charCounter;
                            playerY = lineCounter;
                        }

                        PositionType positionType;
                        switch (boardPart)
                        {
                            case ' ': positionType = PositionType.Empty; break;
                            case '&': positionType = PositionType.Empty; break;
                            case '#': positionType = PositionType.Wall; break;
                            case '*': positionType = PositionType.Target; break;
                            case 'b': positionType = PositionType.Box; break;
                            case '@': positionType = PositionType.BoxOnTarget; break;
                            default: positionType = PositionType.NotInBoard; break;
                        }
                        if (positionType != PositionType.NotInBoard)
                        {
                            boardDefinition.Add(new BoardPositionContent(charCounter, lineCounter, positionType));
                        }

                        charCounter++;
                    }
                }

                lineCounter++;
                charCounter = 1;
            } while (!exit);

            return new BoardDrawResult { BoardDfinition = boardDefinition, PlayerPositionX = playerX, PlayerPositionY = playerY };
        }

        public static void PrintResult(Board end)
        {
            if (end.BoardState == BoardState.Solved)
                Console.WriteLine($"***** Solved *****");
            else
                Console.WriteLine($"***** Solution not found *****");

            // Print movements
            List<BoxMovement> movements = new List<BoxMovement>();
            if (end.BoardState == BoardState.Solved)
            {
                Board currentBoard = end;
                do
                {
                    if (currentBoard.BoxMovement != null)
                        movements.Add(currentBoard.BoxMovement);

                    currentBoard = currentBoard.PreviousBoardCondition;
                } while (currentBoard != null);
                movements.Reverse();

                for (int i = 0; i < movements.Count; i++)
                    Console.WriteLine($"{i+1}. {movements[i]}");
            }
        }
    }
}
