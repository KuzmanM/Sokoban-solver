using System.Collections.Generic;

namespace SokobanSolver.DataModel
{
    public class BoardDrawResult
    {
        public List<BoardPositionContent> BoardDfinition;
        public ushort PlayerPositionX;
        public ushort PlayerPositionY;
    }

    public class ReachableBoxesResult
    {
        /// <summary>
        /// List of reachable boxes (over target or over empty space).
        /// </summary>
        public List<BoardPosition> ReachableBoxes;
        public HashSet<BoardPosition> ReachablePositions;
    }
}
