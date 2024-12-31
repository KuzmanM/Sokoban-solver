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
            // TO DO -> Board and player positions validation
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
                _board.TryGetValue(boxMovement.BoxFrom, out PositionType boxFromPosType);
                if(boxFromPosType == PositionType.Box)
                    _board[boxMovement.BoxFrom] = PositionType.Empty;
                else
                    _board[boxMovement.BoxFrom] = PositionType.Target;

                // Check next positions and set their types
                _board.TryGetValue(boxMovement.BoxTo, out PositionType boxToPosType);
                _board.TryGetValue(upPosition, out PositionType upType);
                _board.TryGetValue(downPosition, out PositionType downType);
                _board.TryGetValue(leftPosition, out PositionType leftType);
                _board.TryGetValue(rightPosition, out PositionType rightType);
                if (!_board.TryGetValue(upLeftPosition, out PositionType upLeftType))
                    upLeftType = PositionType.NotInBoard;
                if (!_board.TryGetValue(upRightPosition, out PositionType upRightType))
                    upRightType = PositionType.NotInBoard;
                if (!_board.TryGetValue(downLeftPosition, out PositionType downLeftType))
                    downLeftType = PositionType.NotInBoard;
                if (!_board.TryGetValue(downRightPosition, out PositionType downRightType))
                    downRightType = PositionType.NotInBoard;
                
                // rollback box to its "from position"
                _board[boxMovement.BoxFrom] = boxFromPosType;

                // Check for positions in walls angle
                if (
                    boxToPosType != PositionType.Target
                    && (upType == PositionType.Wall || downType == PositionType.Wall)
                    && (leftType == PositionType.Wall || rightType == PositionType.Wall)
                   )
                    continue;// not pass the filter

                // Check for positions from square cluster of four boxes
                if (
                    boxToPosType != PositionType.Target

                    && ((
                           (upType == PositionType.Box || upType == PositionType.Wall || upType == PositionType.BoxOnTarget)
                        && (
                               (leftType == PositionType.Box || leftType == PositionType.Wall || leftType == PositionType.BoxOnTarget) && (upLeftType == PositionType.Box || upLeftType == PositionType.Wall || upLeftType == PositionType.BoxOnTarget || upLeftType == PositionType.NotInBoard)
                            || (rightType == PositionType.Box || rightType == PositionType.Wall || rightType == PositionType.BoxOnTarget) && (upRightType == PositionType.Box || upRightType == PositionType.Wall || upRightType == PositionType.BoxOnTarget || upRightType == PositionType.NotInBoard)
                           )
                       )

                    || (
                           (downType == PositionType.Box || downType == PositionType.Wall || downType == PositionType.BoxOnTarget)
                        && (
                               (leftType == PositionType.Box || leftType == PositionType.Wall || leftType == PositionType.BoxOnTarget) && (downLeftType == PositionType.Box || downLeftType == PositionType.Wall || downLeftType == PositionType.BoxOnTarget || downLeftType == PositionType.NotInBoard)
                            || (rightType == PositionType.Box || rightType == PositionType.Wall || rightType == PositionType.BoxOnTarget) && (downRightType == PositionType.Box || downRightType == PositionType.Wall || downRightType == PositionType.BoxOnTarget || downRightType == PositionType.NotInBoard)
                           )
                        ))
                   )
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
                int stepsCount = 1;
                Board root = boardToCompare.PreviousBoardCondition;
                while (root.PreviousBoardCondition != null)
                {
                    root = root.PreviousBoardCondition;
                    stepsCount++;
                }

                // Compare boardToCompare with previous boards in the tree
                if (root != null)
                {
                    bool isMatch = CompareSubTree(root, stepsCount - 1, boardToCompare);
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
        /// Compare sub part of tree. Starts from root and go up to treeLevels down.
        /// </summary>
        /// <param name="root">Tree root node.</param>
        /// <param name="treeLevels">Tree levels under the root node to be compared.</param>
        /// <param name="childToCompare">Node of the tree, that have to be compared with the upper nodes.</param>
        /// <returns>True if some comparison match.</returns>
        public bool CompareSubTree(Board root, int treeLevels, Board childToCompare)
        {
            bool isComparisonMatch = root == childToCompare;
            if (isComparisonMatch)
                return true;

            if (treeLevels > 0 && root.PossibleNextConditions != null)
                foreach (Board child in root.PossibleNextConditions)
                {
                    isComparisonMatch = CompareSubTree(child, (ushort)(treeLevels - 1), childToCompare);
                    if (isComparisonMatch)
                        return true;
                }

            return false;
        }

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
