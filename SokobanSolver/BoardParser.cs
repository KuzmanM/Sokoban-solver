using SokobanSolver.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SokobanSolver
{
    public static class BoardParser
    {
        /// <summary>
        /// Show instructions and read board.
        /// </summary>
        /// <returns>Board positions and player coordinates.</returns>
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
                            case 'B':
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

        /// <summary>
        /// Print solving moves.
        /// </summary>
        /// <param name="end">Last node of solving sequence.</param>
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

        /// <summary>
        /// Throw exception if board have invalid configuration.
        /// </summary>
        /// <param name="boardDefinition">Deffintion of sokoban board - all positions.</param>
        /// <param name="playerXPosition">Begin X position of sokoban player.</param>
        /// <param name="playerYPosition">Begin Y position of sokoban player.</param>
        public static void Vlidate(IEnumerable<BoardPositionContent> boardDefinition, ushort playerXPosition, ushort playerYPosition)
        {
            // Existance of player position
            if (playerXPosition == 0 && playerYPosition == 0)
                throw new ArgumentException("Missing the player's position!");

            // If player position is empty
            foreach (BoardPositionContent position in boardDefinition)
                if (position.X == playerXPosition && position.Y == playerYPosition && position.PositionType != PositionType.Empty)
                    throw new AggregateException("Initial player's position is expected to be empty!");

            // Boxes and targets are equal
            ushort targetsCount = 0;
            ushort boxesCount = 0;
            foreach (BoardPositionContent position in boardDefinition)
            {
                if (position.PositionType == PositionType.Target)
                    targetsCount++;
                if (position.PositionType == PositionType.Box)
                    boxesCount++;
            }
            if (targetsCount != boxesCount)
                throw new ArgumentException("Targets not match the boxes count!");

            if(boxesCount == 0)
                throw new ArgumentException("At least one box is required!");

            //--- Validate if bord contour is closed ---
            // Split (group) to lines and set them in jagged array.
            IEnumerable<IGrouping<ushort, BoardPositionContent>> lineGroups = boardDefinition.GroupBy(i => i.Y);
            BoardPositionContent[][] boardLines = new BoardPositionContent[lineGroups.Count()][];
            for (int i = 0; i < lineGroups.Count(); i++)
                boardLines[i] = lineGroups.ElementAt(i).OrderBy(j => j.X).ToArray();

            // Board size - number of lines
            int boardLinesCount = boardLines.Length;
            if (boardLinesCount < 3)
                throw new ArgumentException("Invalid board definition!");

            // First and last board line contain only walls
            if (boardLines.First().Count() != boardLines.First().Where(i => i.PositionType == PositionType.Wall).Count()
                || boardLines.Last().Count() != boardLines.Last().Where(i => i.PositionType == PositionType.Wall).Count())
                throw new ArgumentException("First and last line of row definition is expected to contains anly walls!");

            // Begin and end walls in each line intersect with begin and en walls in the next line - contour is closed
            var prevBeginEndWalls = GetXOfBeginAndEndWalls(boardLines.First());
            for (int i = 1; i < lineGroups.Count(); i++)
            {
                var currBeginEndWalls = GetXOfBeginAndEndWalls(boardLines[i]);
                
                // Begin
                if(currBeginEndWalls.begin.Intersect(prevBeginEndWalls.begin).Count() == 0)
                    throw new ArgumentException($"Not closed bord contour at begin of line {i+1}!");

                // End
                if (currBeginEndWalls.end.Intersect(prevBeginEndWalls.end).Count() == 0)
                    throw new ArgumentException($"Not closed bord contour at end of line {i + 1}!");

                prevBeginEndWalls = currBeginEndWalls;
            }
        }

        /// <summary>
        /// Gets X coordinate of line begin and end wall positions.
        /// NOTE: Each bord line begins and ends with wall elements.
        /// </summary>
        /// <param name="boardLine">Line of board definition.</param>
        /// <returns>X coordinate of line begin and end wall positions.</returns>
        private static (List<ushort> begin, List<ushort> end) GetXOfBeginAndEndWalls(BoardPositionContent[] boardLine)
        {
            List<ushort> begin = new List<ushort>();
            List<ushort> end = new List<ushort>();
            
            // Begin
            for (int i = 0; i < boardLine.Length; i++)
            {
                BoardPositionContent currentPosition = boardLine[i];
                if (currentPosition.PositionType == PositionType.Wall)
                    begin.Add(currentPosition.X);
                else
                    break;
            }
            
            // End
            for (int i = boardLine.Length - 1; i >= 0; i--)
            {
                BoardPositionContent currentPosition = boardLine[i];
                if (currentPosition.PositionType == PositionType.Wall)
                    end.Add(currentPosition.X);
                else
                    break;
            }

            // NOTE: If boardLine contains only walls, begin and end results will contain the X coordinates of the entire line and that is very useful!

            return (begin, end);
        }
    }
}
