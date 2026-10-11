using System;

namespace BlockBlast.Core
{
    [Serializable]
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int column;
        public int row;

        public GridPosition(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        public bool Equals(GridPosition other) => column == other.column && row == other.row;
        public override bool Equals(object obj) => obj is GridPosition other && Equals(other);
        public override int GetHashCode() => (column * 397) ^ row;
        public static GridPosition operator +(GridPosition a, GridPosition b) =>
            new GridPosition(a.column + b.column, a.row + b.row);
    }
}
