namespace SokobanSolver.DataModel
{
    public class BoxMovement
    {
        public BoardPosition BoxFrom { get; private set; }
        public BoardPosition BoxTo { get; private set; }

        public BoxMovement(BoardPosition boxFrom, BoardPosition boxTo)
        {
            BoxFrom = boxFrom;
            BoxTo = boxTo;
        }

        public override string ToString()
        {
            return $"(X-{BoxFrom.X}, Y-{BoxFrom.Y}) -> (X-{BoxTo.X}, Y-{BoxTo.Y})";
        }
    }
}
