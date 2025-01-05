using SokobanSolver.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SokobanSolver
{
    /// <summary>
    /// Sokoban board.
    /// </summary>
    public class Board : IEquatable<Board>
    {
        #region Tree properties

        /// <summary>
        /// Movement that leads to current board condition.
        /// NOTE: In initial board condition, this property is null.
        /// </summary>
        public BoxMovement BoxMovement { get; set; }

        /// <summary>
        /// Tree parent node.
        /// Previous board condition.
        /// NOTE: In initial board condition, this property is null.
        /// </summary>
        public Board PreviousBoardCondition { get; set; }

        /// <summary>
        /// Tree children nodes.
        /// All possible conditions of the board after single box movement.
        /// NOTE: In initial board condition, this property is null until SetNext is called.
        /// </summary>
        public List<Board> PossibleNextConditions { get; set; }

        #endregion

        #region Initialisation

        /// <summary>
        /// Deffintion of sokoban board - all positions.
        /// NOTE: Expected to be valid - close contour populatet with boxes, targets, wall and empty positions.
        /// </summary>
        private Dictionary<BoardPosition, PositionType> _board;

        /// <summary>
        /// Begin X position of sokoban player.
        /// NOTE: Expected to be empty position.
        /// </summary>
        private ushort _playerXPosition;

        /// <summary>
        /// Begin Y position of sokoban player.
        /// NOTE: Expected to be empty position.
        /// </summary>
        private ushort _playerYPosition;

        /// <summary>
        /// Current state of the board:
        /// - Solved,
        /// - Blocked,
        /// - Have Potential to be solved.
        /// NOTE: In initial board condition, this property is not accurate until SetNext is called.
        /// </summary>
        public BoardState BoardState { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="boardDefinition">
        /// Deffintion of sokoban board - all positions.
        /// NOTE: Expected to be valid - close contour populatet with boxes, targets, wall and empty positions.
        /// </param>
        /// <param name="playerXPosition">
        /// Begin X position of sokoban player.
        /// NOTE: Expected to be empty position.
        /// </param>
        /// <param name="playerYPosition">
        /// Begin Y position of sokoban player.
        /// NOTE: Expected to be empty position.
        /// </param>
        /// <param name="boxMovement">
        /// Movement that leads to current board condition.
        /// NOTE: In initial board condition, this property is null.
        /// </param>
        /// <param name="previousBoardCondition">
        /// Tree parent node.
        /// Previous board condition.
        /// NOTE: In initial board condition, this property is null.
        /// </param>
        /// <param name="possibleNextConditions">
        /// Tree children nodes.
        /// All possible conditions of the board after single box movement.
        /// NOTE: In initial board condition, this property is null.
        /// </param>
        public Board(IEnumerable<BoardPositionContent> boardDefinition, ushort playerXPosition, ushort playerYPosition
            ,BoxMovement boxMovement = null, Board previousBoardCondition = null)
        {
            _board = boardDefinition.ToDictionary(i => new BoardPosition(i.X, i.Y), i => i.PositionType);
            _playerXPosition = playerXPosition;
            _playerYPosition = playerYPosition;

            // Tree properties
            BoxMovement = boxMovement;
            PreviousBoardCondition = previousBoardCondition;
            PossibleNextConditions = new List<Board>(0);
        }

        #endregion

        /// <summary>
        /// Get all boxes rechable from the player.
        /// </summary>
        /// <returns>All rechable from the player box, box on target, empty and target.</returns>
        public ReachableBoxesResult FindReachableBoxes()
        {
            HashSet<BoardPosition> reachablePositionsBag = new HashSet<BoardPosition>();
            Queue<BoardPosition> reachablePositionsQueue = new Queue<BoardPosition>();
            HashSet<BoardPosition> reachableBoxes = new HashSet<BoardPosition>();
            ushort x = _playerXPosition;
            ushort y = _playerYPosition;
            bool processNext = false;
            do
            {
                // Next positions
                BoardPosition upPosition = new BoardPosition(x, (ushort)(y - 1));
                BoardPosition downPosition = new BoardPosition(x, (ushort)(y + 1));
                BoardPosition leftPosition = new BoardPosition((ushort)(x - 1), y);
                BoardPosition rightPosition = new BoardPosition((ushort)(x + 1), y);

                // Next positions type
                _board.TryGetValue(upPosition, out PositionType upType);
                _board.TryGetValue(downPosition, out PositionType downType);
                _board.TryGetValue(leftPosition, out PositionType leftType);
                _board.TryGetValue(rightPosition, out PositionType rightType);

                // Add next positions to the reachable positions
                if ((upType == PositionType.Empty || upType == PositionType.Target) && !reachablePositionsBag.Contains(upPosition))
                {
                    reachablePositionsBag.Add(upPosition);
                    reachablePositionsQueue.Enqueue(upPosition);
                }
                if ((downType == PositionType.Empty || downType == PositionType.Target) && !reachablePositionsBag.Contains(downPosition))
                {
                    reachablePositionsBag.Add(downPosition);
                    reachablePositionsQueue.Enqueue(downPosition);
                }
                if ((leftType == PositionType.Empty || leftType == PositionType.Target) && !reachablePositionsBag.Contains(leftPosition))
                {
                    reachablePositionsBag.Add(leftPosition);
                    reachablePositionsQueue.Enqueue(leftPosition);
                }
                if ((rightType == PositionType.Empty || rightType == PositionType.Target) && !reachablePositionsBag.Contains(rightPosition))
                {
                    reachablePositionsBag.Add(rightPosition);
                    reachablePositionsQueue.Enqueue(rightPosition);
                }

                // Add boxes of the next positions to the reachableBoxes 
                if ((upType == PositionType.Box || upType == PositionType.BoxOnTarget) && !reachableBoxes.Contains(upPosition))
                    reachableBoxes.Add(upPosition);
                if ((downType == PositionType.Box || downType == PositionType.BoxOnTarget) && !reachableBoxes.Contains(downPosition))
                    reachableBoxes.Add(downPosition);
                if ((leftType == PositionType.Box || leftType == PositionType.BoxOnTarget) && !reachableBoxes.Contains(leftPosition))
                    reachableBoxes.Add(leftPosition);
                if ((rightType == PositionType.Box || rightType == PositionType.BoxOnTarget) && !reachableBoxes.Contains(rightPosition))
                    reachableBoxes.Add(rightPosition);

                // Deque reachable empty position and go back to proces it
                processNext = reachablePositionsQueue.Any();
                if (processNext)
                {
                    BoardPosition next = reachablePositionsQueue.Dequeue();
                    x = next.X;
                    y = next.Y;
                }

            } while (processNext);

            return new ReachableBoxesResult() { ReachableBoxes = reachableBoxes.ToList(), ReachablePositions = reachablePositionsBag };
        }

        /// <summary>
        /// Find all possible box movements.
        /// </summary>
        /// <param name="reachableBoxes">All boxes (over target or over empty space) rechable from the player.</param>
        /// <returns>All possible box movements.</returns>
        public List<BoxMovement> FindPossibleMovements(List<BoardPosition> reachableBoxes, HashSet<BoardPosition> reachablePositions)
        {
            List<BoxMovement> result = new List<BoxMovement>();

            foreach (BoardPosition boxPosition in reachableBoxes)
            {
                // Next positions
                BoardPosition upPosition = new BoardPosition(boxPosition.X, (ushort)(boxPosition.Y - 1));
                BoardPosition downPosition = new BoardPosition(boxPosition.X, (ushort)(boxPosition.Y + 1));
                BoardPosition leftPosition = new BoardPosition((ushort)(boxPosition.X - 1), boxPosition.Y);
                BoardPosition rightPosition = new BoardPosition((ushort)(boxPosition.X + 1), boxPosition.Y);

                // Check next positions and set their types
                _board.TryGetValue(upPosition, out PositionType upType);
                _board.TryGetValue(downPosition, out PositionType downType);
                _board.TryGetValue(leftPosition, out PositionType leftType);
                _board.TryGetValue(rightPosition, out PositionType rightType);

                // Add movements
                if ((upType == PositionType.Empty || upType == PositionType.Target) && reachablePositions.Contains(downPosition))
                    result.Add(new BoxMovement(boxPosition, upPosition));
                if ((downType == PositionType.Empty || downType == PositionType.Target) && reachablePositions.Contains(upPosition))
                    result.Add(new BoxMovement(boxPosition, downPosition));
                if ((leftType == PositionType.Empty || leftType == PositionType.Target) && reachablePositions.Contains(rightPosition))
                    result.Add(new BoxMovement(boxPosition, leftPosition));
                if ((rightType == PositionType.Empty || rightType == PositionType.Target) && reachablePositions.Contains(leftPosition))
                    result.Add(new BoxMovement(boxPosition, rightPosition));
            }

            return result;
        }

        /// <summary>
        /// Filter blocking movements.
        /// NOTE: TO DO -> improve with hard filtration for beter performance.
        /// </summary>
        /// <param name="allBoxMovements">All possible box movements.</param>
        /// <returns>Filtered box movements.</returns>
        public List<BoxMovement> FilterMovements(List<BoxMovement> allBoxMovements)
        {
            List<BoxMovement> result = new List<BoxMovement>();

            foreach (BoxMovement boxMovement in allBoxMovements)
            {
                NextPositionsTypesResult ts = GetNextPositionsTypes(boxMovement, _board);

                // Check for positions in walls angle (diagonal wall parts are not important)
                if (IsWallAngle(ts))
                    continue;// not pass the filter

                // Check for square cluster of four elements (boxes and walls)
                if (IsInSquareCluster( ts))
                    continue;// not pass the filter

                if (IsLayOnDeadEndsWall(boxMovement, ts, _board))
                    continue;// not pass the filter

                result.Add(boxMovement);// pass the filters
            }

            return result;
        }

        /// <summary>
        /// Get board conditions after each possible movement.
        /// </summary>
        /// <param name="possibleMovements">All possible box movements.</param>
        /// <returns>Board after each one possible box movement.</returns>
        public List<Board> FindPossibleBoardConditions(List<BoxMovement> possibleMovements)
        {
            List<Board> result = new List<Board>(possibleMovements.Count);

            foreach (BoxMovement move in possibleMovements)
            {
                // Clone current board positions and modify them to list of BoardPositionContent
                KeyValuePair<BoardPosition, PositionType>[] thisBoard = _board.ToArray();
                BoardPositionContent[] clonedBoardDefinition = new BoardPositionContent[thisBoard.Length];
                for (int i = 0; i < thisBoard.Length; i++)
                {
                    KeyValuePair<BoardPosition, PositionType> t = thisBoard[i];
                    clonedBoardDefinition[i] = new BoardPositionContent(t.Key.X, t.Key.Y, t.Value);
                }

                // Get movement positions type
                // If "from" position is Box that positions becomes Empty
                // If "from" position is BoxOnTarget that positions becomes Target
                // And the oposite
                // If "to" position is Empty that positions becomes Box
                // If "to" position is Target that positions becomes BoxOnTarget
                _board.TryGetValue(move.BoxFrom, out PositionType fromType);
                _board.TryGetValue(move.BoxTo, out PositionType toType);

                // Modify cloned positions list according the movement (apply movement)
                BoardPositionContent searchFrom = new BoardPositionContent(move.BoxFrom.X, move.BoxFrom.Y, fromType);
                BoardPositionContent searchTo = new BoardPositionContent(move.BoxTo.X, move.BoxTo.Y, toType);
                bool fromModified = false;
                bool toModified = false;
                foreach (BoardPositionContent position in clonedBoardDefinition)
                {
                    if (!fromModified && position == searchFrom)
                    {
                        if (position.PositionType == PositionType.Box)
                            position.PositionType = PositionType.Empty;
                        else if (position.PositionType == PositionType.BoxOnTarget)
                            position.PositionType = PositionType.Target;
                        else
                            throw new ApplicationException("Not expected position content! Only box could be moved!?");
                        fromModified = true;
                    }
                    else if (!toModified && position == searchTo)
                    {
                        if (position.PositionType == PositionType.Empty)
                            position.PositionType = PositionType.Box;
                        else if (position.PositionType == PositionType.Target)
                            position.PositionType = PositionType.BoxOnTarget;
                        else
                            throw new ApplicationException("Not expected position content! Only empty and target content could accept box!?");
                        toModified = true;
                    }

                    if (fromModified && toModified)
                        break;
                }

                // Create child board
                Board board = new Board(clonedBoardDefinition, move.BoxFrom.X, move.BoxFrom.Y, move, this);
                result.Add(board);
            }

            return result;
        }

        /// <summary>
        /// Filter blocking conditions.
        /// NOTE: TO DO -> improve with hard filtration for beter performance. But keep blocking conditions detection as simple and efficient as possible.
        /// </summary>
        /// <param name="allBoardConditions">All possible board conditions.</param>
        /// <returns>Filtered board conditions.</returns>
        public List<Board> FilterBoardConditions(List<Board> allBoardConditions)
        {
            // Cycles prevention - remove boards that match with board condition from previous movements.
            List<Board> boardConditionsToRemove = new List<Board>();
            foreach (Board boardToCompare in allBoardConditions)
            {
                // Find root element and count of the tree levels to it
                Board root = boardToCompare.PreviousBoardCondition;
                while (root.PreviousBoardCondition != null)
                    root = root.PreviousBoardCondition;

                // Compare boardToCompare with previous boards in the tree
                if (root != null)
                {
                    bool isMatch = CompareTreeNodes(root, boardToCompare);
                    if (isMatch)
                        boardConditionsToRemove.Add(boardToCompare);
                }
            }

            // Remove filtred items
            foreach (Board itemToRemove in boardConditionsToRemove)
                allBoardConditions.Remove(itemToRemove);

            return allBoardConditions;
        }

        /// <summary>
        /// Analise the board and find next movements and current board state.
        /// </summary>
        /// <returns>Bottom of the tree with potentially solved board which can backtracked for the movements of the solution.</returns>
        public Board SetNext()
        {
            // Check if this bord is solved (there is no boxes out of the target positions)
            bool existUnsolved = _board.Values.Any(i => i == PositionType.Box);
            if (!existUnsolved)
            {
                BoardState = BoardState.Solved;
                return this;
            }

            BoardState = BoardState.HavePotential;

            // Execute chain of analitic methods
            ReachableBoxesResult reachable = FindReachableBoxes();
            List<BoxMovement> possibleMovements = FindPossibleMovements(reachable.ReachableBoxes, reachable.ReachablePositions);
            possibleMovements = FilterMovements(possibleMovements);
            List<Board> nextConditions = FindPossibleBoardConditions(possibleMovements);
            nextConditions = FilterBoardConditions(nextConditions);
            PossibleNextConditions = nextConditions;

            // If blocked exit without solution
            if (!PossibleNextConditions.Any())
            {
                BoardState = BoardState.Blocked;
                return this;
            }

            // Process children boards and exit immediately if solution is found.
            Board result = this;
            foreach (Board child in PossibleNextConditions)
            {
                result = child.SetNext();
                // Exit with the first solution
                if (result.BoardState == BoardState.Solved)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Compare nodes of tree with a node. Starts from root and go up to treeLevels down.
        /// </summary>
        /// <param name="root">Tree root node.</param>
        /// <param name="childToCompare">Node that have to be compared with the tree nodes.</param>
        /// <returns>True if some comparison match.</returns>
        public bool CompareTreeNodes(Board root, Board childToCompare)
        {
            bool isComparisonMatch = root == childToCompare;
            if (isComparisonMatch)
                return true;

            if (root.PossibleNextConditions != null)
                foreach (Board child in root.PossibleNextConditions)
                {
                    isComparisonMatch = CompareTreeNodes(child, childToCompare);
                    if (isComparisonMatch)
                        return true;
                }

            return false;
        }

        #region FilterMovements utils

        /// <summary>
        /// Get all next types of te box destination position.
        /// </summary>
        /// <param name="boxMovement">Box movement.</param>
        /// <param name="board">Entire board definition.</param>
        /// <returns>All next types of te box destination position.</returns>
        private static NextPositionsTypesResult GetNextPositionsTypes(BoxMovement boxMovement, Dictionary<BoardPosition, PositionType> board)
        {
            NextPositionsTypesResult types = new NextPositionsTypesResult();

            // Next positions of the box after the movement
            BoardPosition upPosition = new BoardPosition(boxMovement.BoxTo.X, (ushort)(boxMovement.BoxTo.Y - 1));
            BoardPosition downPosition = new BoardPosition(boxMovement.BoxTo.X, (ushort)(boxMovement.BoxTo.Y + 1));
            BoardPosition leftPosition = new BoardPosition((ushort)(boxMovement.BoxTo.X - 1), boxMovement.BoxTo.Y);
            BoardPosition rightPosition = new BoardPosition((ushort)(boxMovement.BoxTo.X + 1), boxMovement.BoxTo.Y);
            BoardPosition upLeftPosition = new BoardPosition((ushort)(boxMovement.BoxTo.X - 1), (ushort)(boxMovement.BoxTo.Y - 1));
            BoardPosition upRightPosition = new BoardPosition((ushort)(boxMovement.BoxTo.X + 1), (ushort)(boxMovement.BoxTo.Y - 1));
            BoardPosition downLeftPosition = new BoardPosition((ushort)(boxMovement.BoxTo.X - 1), (ushort)(boxMovement.BoxTo.Y + 1));
            BoardPosition downRightPosition = new BoardPosition((ushort)(boxMovement.BoxTo.X + 1), (ushort)(boxMovement.BoxTo.Y + 1));

            // temporary remove the box from "from position"
            if(!board.TryGetValue(boxMovement.BoxFrom, out PositionType boxFromPosType))
                throw new ApplicationException("Invalid box movement!");
            if (boxFromPosType == PositionType.Box)
                board[boxMovement.BoxFrom] = PositionType.Empty;
            else
                board[boxMovement.BoxFrom] = PositionType.Target;

            // Check next positions and set their types
            board.TryGetValue(boxMovement.BoxTo, out types.BoxToPosType);
            board.TryGetValue(upPosition, out types.UpType);
            board.TryGetValue(downPosition, out types.DownType);
            board.TryGetValue(leftPosition, out types.LeftType);
            board.TryGetValue(rightPosition, out types.RightType);
            if (!board.TryGetValue(upLeftPosition, out types.UpLeftType))
                types.UpLeftType = PositionType.NotInBoard;
            if (!board.TryGetValue(upRightPosition, out types.UpRightType))
                types.UpRightType = PositionType.NotInBoard;
            if (!board.TryGetValue(downLeftPosition, out types.DownLeftType))
                types.DownLeftType = PositionType.NotInBoard;
            if (!board.TryGetValue(downRightPosition, out types.DownRightType))
                types.DownRightType = PositionType.NotInBoard;

            // rollback box to its "from position"
            board[boxMovement.BoxFrom] = boxFromPosType;

            return types;
        }

        /// <summary>
        /// Check is position in walls angle (diagonal wall parts are not important).
        /// </summary>
        /// <param name="ts">All next types of te box destination position.</param>
        /// <returns>True if the position is in walls angle.</returns>
        private static bool IsWallAngle(NextPositionsTypesResult ts)
        {
            return ts.BoxToPosType != PositionType.Target
                && (ts.UpType == PositionType.Wall || ts.DownType == PositionType.Wall)
                && (ts.LeftType == PositionType.Wall || ts.RightType == PositionType.Wall);
        }

        /// <summary>
        /// Check for square cluster of four elements (boxes and walls).
        /// </summary>
        /// <param name="ts">All next types of te box destination position.</param>
        /// <returns>True if the position is in square cluster of four elements (boxes and walls).</returns>
        private static bool IsInSquareCluster(NextPositionsTypesResult ts)
        {
            return ts.BoxToPosType != PositionType.Target

                && ((
                        (ts.UpType == PositionType.Box || ts.UpType == PositionType.Wall || ts.UpType == PositionType.BoxOnTarget)
                    && (
                            (ts.LeftType == PositionType.Box || ts.LeftType == PositionType.Wall || ts.LeftType == PositionType.BoxOnTarget) && (ts.UpLeftType == PositionType.Box || ts.UpLeftType == PositionType.Wall || ts.UpLeftType == PositionType.BoxOnTarget || ts.UpLeftType == PositionType.NotInBoard)
                        || (ts.RightType == PositionType.Box || ts.RightType == PositionType.Wall || ts.RightType == PositionType.BoxOnTarget) && (ts.UpRightType == PositionType.Box || ts.UpRightType == PositionType.Wall || ts.UpRightType == PositionType.BoxOnTarget || ts.UpRightType == PositionType.NotInBoard)
                        )
                    )

                || (
                        (ts.DownType == PositionType.Box || ts.DownType == PositionType.Wall || ts.DownType == PositionType.BoxOnTarget)
                    && (
                            (ts.LeftType == PositionType.Box || ts.LeftType == PositionType.Wall || ts.LeftType == PositionType.BoxOnTarget) && (ts.DownLeftType == PositionType.Box || ts.DownLeftType == PositionType.Wall || ts.DownLeftType == PositionType.BoxOnTarget || ts.DownLeftType == PositionType.NotInBoard)
                        || (ts.RightType == PositionType.Box || ts.RightType == PositionType.Wall || ts.RightType == PositionType.BoxOnTarget) && (ts.DownRightType == PositionType.Box || ts.DownRightType == PositionType.Wall || ts.DownRightType == PositionType.BoxOnTarget || ts.DownRightType == PositionType.NotInBoard)
                        )
                    ));
        }

        /// <summary>
        /// Check if position lying on wall without targets in front of it.
        /// </summary>
        /// <param name="boxTo">Box destination position.</param>
        /// <param name="ts">All next types of te box destination position.</param>
        /// <param name="board">Entire board definition.</param>
        /// <returns>True if lying on wall without targets in this line.</returns>
        private static bool IsLayOnDeadEndsWall(BoxMovement boxMovement, NextPositionsTypesResult ts, Dictionary<BoardPosition, PositionType> board)
        {
            bool result = false;

            // If moves is over a target, does not filter it!
            if(!board.TryGetValue(boxMovement.BoxTo, out PositionType boxToPosType))
                throw new ApplicationException("Invalid box movement!");
            if (boxToPosType == PositionType.Target)
                return false;

            // Temporary remove the box from "from position"
            if (!board.TryGetValue(boxMovement.BoxFrom, out PositionType boxFromPosType))
                throw new ApplicationException("Invalid box movement!");
            if (boxFromPosType == PositionType.Box)
                board[boxMovement.BoxFrom] = PositionType.Empty;
            else
                board[boxMovement.BoxFrom] = PositionType.Target;

            // Up wall
            if (ts.UpType == PositionType.Wall)
            {
                HorizontalStepsToTheWallResult stepsInfo = AnaliseHorizontalStepsInFrontOfWall(boxMovement.BoxTo, board);
                // not enough targets on the wall
                result = stepsInfo.LeftBoxesCount + stepsInfo.RightBoxesCount + 1 > stepsInfo.LeftTargetsCount + stepsInfo.RightTargetsCount;

                if(result)
                {
                    ushort Y = (ushort)(boxMovement.BoxTo.Y - 1);// wall at up
                    BoardPosition from = new BoardPosition((ushort)(boxMovement.BoxTo.X - stepsInfo.LeftStepsCount), Y);
                    BoardPosition to = new BoardPosition((ushort)(boxMovement.BoxTo.X + stepsInfo.RightStepsCount), Y);
                    result = IsContinuousWall(from, to, board);
                }
            }

            // Down wall
            if (!result && ts.DownType == PositionType.Wall)
            {
                HorizontalStepsToTheWallResult stepsInfo = AnaliseHorizontalStepsInFrontOfWall(boxMovement.BoxTo, board);
                // not enough targets on the wall
                result = stepsInfo.LeftBoxesCount + stepsInfo.RightBoxesCount + 1 > stepsInfo.LeftTargetsCount + stepsInfo.RightTargetsCount;

                if (result)
                {
                    ushort Y = (ushort)(boxMovement.BoxTo.Y + 1);// wall at down
                    BoardPosition from = new BoardPosition((ushort)(boxMovement.BoxTo.X - stepsInfo.LeftStepsCount), Y);
                    BoardPosition to = new BoardPosition((ushort)(boxMovement.BoxTo.X + stepsInfo.RightStepsCount), Y);
                    result = IsContinuousWall(from, to, board);
                }
            }

            // Left wall
            if (!result && ts.LeftType == PositionType.Wall)
            {
                VerticalStepsToTheWallResult stepsInfo = AnaliseVerticalStepsInFrontOfWall(boxMovement.BoxTo, board);
                // not enough targets on the wall
                result = stepsInfo.UpBoxesCount + stepsInfo.DownBoxesCount + 1 > stepsInfo.UpTargetsCount + stepsInfo.DownTargetsCount;

                if (result)
                {
                    ushort X = (ushort)(boxMovement.BoxTo.X - 1);// wall at left
                    BoardPosition from = new BoardPosition(X, (ushort)(boxMovement.BoxTo.Y - stepsInfo.UpStepsCount));
                    BoardPosition to = new BoardPosition(X, (ushort)(boxMovement.BoxTo.Y + stepsInfo.DownStepsCount));
                    result = IsContinuousWall(from, to, board);
                }
            }

            // Right wall
            if (!result && ts.RightType == PositionType.Wall)
            {
                VerticalStepsToTheWallResult stepsInfo = AnaliseVerticalStepsInFrontOfWall(boxMovement.BoxTo, board);
                // not enough targets on the wall
                result = stepsInfo.UpBoxesCount + stepsInfo.DownBoxesCount + 1 > stepsInfo.UpTargetsCount + stepsInfo.DownTargetsCount;

                if (result)
                {
                    ushort X = (ushort)(boxMovement.BoxTo.X + 1);// wall at right
                    BoardPosition from = new BoardPosition(X, (ushort)(boxMovement.BoxTo.Y - stepsInfo.UpStepsCount));
                    BoardPosition to = new BoardPosition(X, (ushort)(boxMovement.BoxTo.Y + stepsInfo.DownStepsCount));
                    result = IsContinuousWall(from, to, board);
                }
            }

            // rollback box to its "from position"
            board[boxMovement.BoxFrom] = boxFromPosType;

            return result;
        }

        /// <summary>
        /// Analise content in all positions in front of horizontal wall.
        /// </summary>
        /// <param name="position">Box position.</param>
        /// <param name="board">Entire board definition.</param>
        /// <returns>Result analysis.</returns>
        private static HorizontalStepsToTheWallResult AnaliseHorizontalStepsInFrontOfWall(BoardPosition position, Dictionary<BoardPosition, PositionType> board)
        {
            HorizontalStepsToTheWallResult result = new HorizontalStepsToTheWallResult();
            PositionType stepType;

            //Left
            BoardPosition positionClone = new BoardPosition(position.X, position.Y);
            do
            {
                positionClone.X--;
                if (!board.TryGetValue(positionClone, out stepType))
                    stepType = PositionType.NotInBoard;
                result.LeftStepsCount++;
                switch (stepType)
                {
                    case PositionType.Box:
                        result.LeftBoxesCount++;
                        break;
                    case PositionType.Target:
                        result.LeftTargetsCount++;
                        break;
                    case PositionType.BoxOnTarget:
                        result.LeftBoxesCount++;
                        result.LeftTargetsCount++;
                        break;
                    case PositionType.Wall:
                        result.LeftStepsCount--;
                        break;
                    case PositionType.NotInBoard:
                        throw new ApplicationException("Invalid board definition!");
                }
            }
            while (stepType != PositionType.Wall);

            //Right
            positionClone = new BoardPosition(position.X, position.Y);
            do
            {
                positionClone.X++;
                if (!board.TryGetValue(positionClone, out stepType))
                    stepType = PositionType.NotInBoard;
                result.RightStepsCount++;
                switch (stepType)
                {
                    case PositionType.Box:
                        result.RightBoxesCount++;
                        break;
                    case PositionType.Target:
                        result.RightTargetsCount++;
                        break;
                    case PositionType.BoxOnTarget:
                        result.RightBoxesCount++;
                        result.RightTargetsCount++;
                        break;
                    case PositionType.Wall:
                        result.RightStepsCount--;
                        break;
                    case PositionType.NotInBoard:
                        throw new ApplicationException("Invalid board definition!");
                }
            }
            while (stepType != PositionType.Wall);

            return result;
        }

        /// <summary>
        /// Analise content in all positions in front of vertical wall.
        /// </summary>
        /// <param name="position">Box position.</param>
        /// <param name="board">Entire board definition.</param>
        /// <returns>Result analysis.</returns>
        private static VerticalStepsToTheWallResult AnaliseVerticalStepsInFrontOfWall(BoardPosition position, Dictionary<BoardPosition, PositionType> board)
        {
            VerticalStepsToTheWallResult result = new VerticalStepsToTheWallResult();
            PositionType stepType;

            //Up
            BoardPosition positionClone = new BoardPosition(position.X, position.Y);
            do
            {
                positionClone.Y--;
                if (!board.TryGetValue(positionClone, out stepType))
                    stepType = PositionType.NotInBoard;
                result.UpStepsCount++;
                switch (stepType)
                {
                    case PositionType.Box:
                        result.UpBoxesCount++;
                        break;
                    case PositionType.Target:
                        result.UpTargetsCount++;
                        break;
                    case PositionType.BoxOnTarget:
                        result.UpBoxesCount++;
                        result.UpTargetsCount++;
                        break;
                    case PositionType.Wall:
                        result.UpStepsCount--;
                        break;
                    case PositionType.NotInBoard:
                        throw new ApplicationException("Invalid board definition!");
                }
            }
            while (stepType != PositionType.Wall);

            //Down
            positionClone = new BoardPosition(position.X, position.Y);
            do
            {
                positionClone.Y++;
                if (!board.TryGetValue(positionClone, out stepType))
                    stepType = PositionType.NotInBoard;
                result.DownStepsCount++;
                switch (stepType)
                {
                    case PositionType.Box:
                        result.DownBoxesCount++;
                        break;
                    case PositionType.Target:
                        result.DownTargetsCount++;
                        break;
                    case PositionType.BoxOnTarget:
                        result.DownBoxesCount++;
                        result.DownTargetsCount++;
                        break;
                    case PositionType.Wall:
                        result.DownStepsCount--;
                        break;
                    case PositionType.NotInBoard:
                        throw new ApplicationException("Invalid board definition!");
                }
            }
            while (stepType != PositionType.Wall);

            return result;
        }

        /// <summary>
        /// Check horizontal or vertical wall for gaps.
        /// </summary>
        /// <param name="from">From coordinate (left - horizontal search or up-vertical search).</param>
        /// <param name="to">To coordinate (right - horizontal search or down-vertical search).</param>
        /// <param name="board">Entire board definition.</param>
        /// <returns>True if the wall is without gaps.</returns>
        private static bool IsContinuousWall(BoardPosition from, BoardPosition to, Dictionary<BoardPosition, PositionType> board)
        {
            PositionType stepType;
            BoardPosition fromClone = new BoardPosition(from.X, from.Y);

            // Horizontal
            if (from.Y == to.Y)
            {
                if (from.X > to.X)
                    throw new ArgumentException("From is bigger then To!");
                do
                {
                    if (!board.TryGetValue(fromClone, out stepType))
                        throw new ApplicationException("Invalid board definition!");

                    if (stepType != PositionType.Wall)
                        return false;

                    fromClone.X++;// move right

                } while (fromClone.X <= to.X);
            }
            // Vertical
            else if (from.X == to.X)
            {
                if (from.Y > to.Y)
                    throw new ArgumentException("From is bigger then To!");
                do
                {
                    if (!board.TryGetValue(fromClone, out stepType))
                        throw new ApplicationException("Invalid board definition!");

                    if (stepType != PositionType.Wall)
                        return false;

                    fromClone.Y++;// move down

                } while (fromClone.Y <= to.Y);
            }
            else
                throw new ApplicationException("Horizontal or vertical line is expected!");

            return true;
        }

        #endregion

        #region Equals and Overrides

        public bool Equals(Board other)
        {
            if (ReferenceEquals(other, null))
                return false;

            // NOTE: thisBoard and otherBoard is expected allways to have equal length.
            // But order of element of both boards could be different (elements contain position and actual order doesn't matter)
            KeyValuePair<BoardPosition, PositionType>[] thisBoard = _board.ToArray();
            KeyValuePair<BoardPosition, PositionType>[] otherBoard = other._board.ToArray();
            int matchesCount = 0;
            for (int i = 0; i < thisBoard.Length - 1; i++)// this board
            {
                KeyValuePair<BoardPosition, PositionType> t = thisBoard[i];
                for (int j = 0; j < otherBoard.Length - 1; j++)// other board
                {
                    KeyValuePair<BoardPosition, PositionType> o = otherBoard[j];
                    if (t.Key.X == o.Key.X && t.Key.Y == o.Key.Y && t.Value == o.Value)
                    {
                        matchesCount++;
                        break;
                    }
                }

                if (matchesCount != i + 1)
                    return false;
            }

            // In equal boards the player could be in different cavity (different side of the boxes),
            // so additional comparison of reachable positions have to be made
            HashSet<BoardPosition> thisReachablePositions = this.FindReachableBoxes().ReachablePositions;
            HashSet<BoardPosition> otherReachablePositions = other.FindReachableBoxes().ReachablePositions;
            if (thisReachablePositions.Count != otherReachablePositions.Count)
                return false;
            foreach (BoardPosition thisPosition in thisReachablePositions)
                if (!otherReachablePositions.Contains(thisPosition))
                    return false;

            return true;
        }

        public static bool operator ==(Board obj1, Board obj2)
        {
            if (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
                return true;
            else if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
                return false;
            else
                return obj1.Equals(obj2);
        }

        public static bool operator !=(Board obj1, Board obj2)
        {
                return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Board);
        }

        public override int GetHashCode()
        {
            return (_playerXPosition*1000 + _playerYPosition).GetHashCode();
        }

        public override string ToString()
        {
            string boardState = Enum.GetName(typeof(BoardState), BoardState);

            string movement = string.Empty;
            if (BoxMovement != null)
                movement = $" {BoxMovement}";
            
            string nextConditionsCount = string.Empty;
            if (PossibleNextConditions != null)
                nextConditionsCount = $" : {PossibleNextConditions.Count} children";

            return $"{boardState}{movement}{nextConditionsCount}";
        }

        #endregion
    }
}
