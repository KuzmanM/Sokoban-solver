using System;

namespace SokobanSolver.DataModel
{
    /// <summary>
    /// Bord position and content type.
    /// </summary>
    public class BoardPositionContent : BoardPosition, IEquatable<BoardPositionContent>
    {
        /// <summary>
        /// Board position type (content).
        /// </summary>
        public PositionType PositionType { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X Position.</param>
        /// <param name="y">Y Position.</param>
        /// <param name="positionType">Board position type (content).</param>
        public BoardPositionContent(ushort x, ushort y, PositionType positionType) : base(x, y)
        {
            PositionType = positionType;
        }

        #region Equals and Overrides

        public bool Equals(BoardPositionContent other)
        {
            if (ReferenceEquals(other, null))
                return false;
            
            return base.Equals(other) && other.PositionType == PositionType;
        }

        public static bool operator ==(BoardPositionContent obj1, BoardPositionContent obj2)
        {
            if (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
                return true;
            else if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
                return false;
            else
                return obj1.Equals(obj2);
        }

        public static bool operator !=(BoardPositionContent obj1, BoardPositionContent obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoardPositionContent);
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() - (int)PositionType).GetHashCode();
        }

        public override string ToString()
        {
            return $"{base.ToString()} {Enum.GetName(typeof(PositionType), PositionType)}";
        }

        #endregion
    }
}
