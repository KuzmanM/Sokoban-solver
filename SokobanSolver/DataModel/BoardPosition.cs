using System;

namespace SokobanSolver.DataModel
{
    /// <summary>
    /// Bord position.
    /// </summary>
    public class BoardPosition : IEquatable<BoardPosition>
    {
        /// <summary>
        /// X Position.
        /// </summary>
        public ushort X { get; set; }

        /// <summary>
        /// Y Position.
        /// </summary>
        public ushort Y { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X Position.</param>
        /// <param name="y">Y Position.</param>
        public BoardPosition(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        #region Equals and Overrides

        public bool Equals(BoardPosition other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return other.X == X && other.Y == Y;
        }

        public static bool operator ==(BoardPosition obj1, BoardPosition obj2)
        {
            if (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
                return true;
            else if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
                return false;
            else
                return obj1.Equals(obj2);
        }

        public static bool operator !=(BoardPosition obj1, BoardPosition obj2)
        {
            return !(obj1==obj2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoardPosition);
        }

        public override int GetHashCode()
        {
            return (X * 1000 + Y).GetHashCode();
        }

        public override string ToString()
        {
            return $"(X-{X}, Y-{Y})";
        }

        #endregion
    }
}
