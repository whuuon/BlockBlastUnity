using System;
using System.Collections.Generic;

namespace BlockBlast.Core
{
    public sealed class BoardModel
    {
        private readonly int[,] cells;
        public int Size { get; }
        public int this[int column, int row] => cells[column, row];

        public BoardModel(int size)
        {
            if (size < 2) throw new ArgumentOutOfRangeException(nameof(size));
            Size = size;
            cells = new int[size, size];
            Reset();
        }

        public void Reset()
        {
            for (int row = 0; row < Size; row++)
                for (int column = 0; column < Size; column++)
                    cells[column, row] = -1;
        }

        public bool CanPlace(ShapeData shape, GridPosition origin)
        {
            if (shape == null) return false;
            foreach (GridPosition offset in shape.Cells)
            {
                GridPosition cell = origin + offset;
                if (cell.column < 0 || cell.row < 0 || cell.column >= Size || cell.row >= Size ||
                    cells[cell.column, cell.row] >= 0) return false;
            }
            return true;
        }

        public bool HasPlacement(ShapeData shape)
        {
            for (int row = 0; row < Size; row++)
                for (int column = 0; column < Size; column++)
                    if (CanPlace(shape, new GridPosition(column, row))) return true;
            return false;
        }

        public PlacementResult Place(ShapeData shape, GridPosition origin, int colorIndex)
        {
            if (colorIndex < 0 || !CanPlace(shape, origin)) return new PlacementResult(false);
            foreach (GridPosition offset in shape.Cells)
            {
                GridPosition cell = origin + offset;
                cells[cell.column, cell.row] = colorIndex;
            }

            var fullRows = new List<int>();
            var fullColumns = new List<int>();
            for (int index = 0; index < Size; index++)
            {
                bool fullRow = true;
                bool fullColumn = true;
                for (int scan = 0; scan < Size; scan++)
                {
                    fullRow &= cells[scan, index] >= 0;
                    fullColumn &= cells[index, scan] >= 0;
                }
                if (fullRow) fullRows.Add(index);
                if (fullColumn) fullColumns.Add(index);
            }

            var cleared = new HashSet<GridPosition>();
            foreach (int row in fullRows)
                for (int column = 0; column < Size; column++) cleared.Add(new GridPosition(column, row));
            foreach (int column in fullColumns)
                for (int row = 0; row < Size; row++) cleared.Add(new GridPosition(column, row));
            foreach (GridPosition cell in cleared) cells[cell.column, cell.row] = -1;
            var clearedArray = new GridPosition[cleared.Count];
            cleared.CopyTo(clearedArray);
            return new PlacementResult(true, shape.Cells.Count, fullRows.Count + fullColumns.Count, clearedArray);
        }
    }
}
