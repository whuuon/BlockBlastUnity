using System;
using System.Collections.Generic;

namespace BlockBlast.Core
{
    public sealed class ShapeData
    {
        private readonly GridPosition[] cells;
        public IReadOnlyList<GridPosition> Cells { get; }
        public int Width { get; }
        public int Height { get; }

        public ShapeData(params GridPosition[] cells)
        {
            if (cells == null || cells.Length == 0)
                throw new ArgumentException("A shape must contain at least one cell.", nameof(cells));

            this.cells = (GridPosition[])cells.Clone();
            var uniqueCells = new HashSet<GridPosition>();
            foreach (GridPosition cell in this.cells)
            {
                if (cell.column < 0 || cell.row < 0 || !uniqueCells.Add(cell))
                    throw new ArgumentException("Shape cells must be unique and non-negative.", nameof(cells));
                Width = Math.Max(Width, cell.column + 1);
                Height = Math.Max(Height, cell.row + 1);
            }
            Cells = Array.AsReadOnly(this.cells);
        }
    }
}
