using System.Collections.Generic;

namespace SokobanSolver.DataModel
{
    public class BoardDrawResult
    {
        public List<BoardPositionContent> BoardDefinition;
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

    public class NextPositionsTypesResult
    {
        public PositionType BoxToPosType;
        public PositionType UpType;
        public PositionType DownType;
        public PositionType LeftType;
        public PositionType RightType;
        public PositionType UpLeftType;
        public PositionType UpRightType;
        public PositionType DownLeftType;
        public PositionType DownRightType;
    }

    public class HorizontalStepsToTheWallResult
    {
        /// <summary>
        /// Count of steps in left direction to the wall.
        /// </summary>
        public ushort LeftStepsCount = 0;
        /// <summary>
        /// Count of steps in right direction to the wall.
        /// </summary>
        public ushort RightStepsCount = 0;
        /// <summary>
        /// Cont of targets in left direction to the wall.
        /// </summary>
        public ushort LeftTargetsCount = 0;
        /// <summary>
        /// Count of targets in right direction to the wall.
        /// </summary>
        public ushort RightTargetsCount = 0;
        /// <summary>
        /// Count of boxes in left direction to the wall.
        /// </summary>
        public ushort LeftBoxesCount = 0;
        /// <summary>
        /// Count of boxes in right direction to the wall.
        /// </summary>
        public ushort RightBoxesCount = 0;
    }

    public class VerticalStepsToTheWallResult
    {
        /// <summary>
        /// Count of steps in up direction to the wall.
        /// </summary>
        public ushort UpStepsCount = 0;
        /// <summary>
        /// Count of steps in down direction to the wall.
        /// </summary>
        public ushort DownStepsCount = 0;
        /// <summary>
        /// Cont of targets in up direction to the wall.
        /// </summary>
        public ushort UpTargetsCount = 0;
        /// <summary>
        /// Count of targets in down direction to the wall.
        /// </summary>
        public ushort DownTargetsCount = 0;
        /// <summary>
        /// Count of boxes in up direction to the wall.
        /// </summary>
        public ushort UpBoxesCount = 0;
        /// <summary>
        /// Count of boxes in down direction to the wall.
        /// </summary>
        public ushort DownBoxesCount = 0;
    }
}
