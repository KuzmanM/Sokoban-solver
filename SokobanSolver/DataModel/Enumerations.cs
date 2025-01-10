namespace SokobanSolver.DataModel
{
    /// <summary>
    /// Board quadrants types.
    /// </summary>
    public enum PositionType
    {
        /// <summary>
        /// This position is out of the board.
        /// </summary>
        NotInBoard,
        /// <summary>
        /// Board empty position - can walk through.
        /// </summary>
        Empty,
        /// <summary>
        ///  Board postion wit wall - can't walk through.
        /// </summary>
        Wall,
        /// <summary>
        /// Box target position - can walk through.
        /// </summary>
        Target,
        /// <summary>
        /// Box that have to be moved to the target postions.
        /// </summary>
        Box,
        /// <summary>
        /// Box on target position.
        /// </summary>
        BoxOnTarget,
    }
}
