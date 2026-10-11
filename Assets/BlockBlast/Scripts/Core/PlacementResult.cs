using System;
using System.Collections.Generic;

namespace BlockBlast.Core
{
    public sealed class PlacementResult
    {
        public bool Succeeded { get; }
        public int PlacedCells { get; }
        public int ClearedLines { get; }
        public IReadOnlyList<GridPosition> ClearedCells { get; }

        public PlacementResult(bool succeeded, int placedCells = 0, int clearedLines = 0,
            GridPosition[] clearedCells = null)
        {
            Succeeded = succeeded;
            PlacedCells = placedCells;
            ClearedLines = clearedLines;
            ClearedCells = Array.AsReadOnly(clearedCells ?? Array.Empty<GridPosition>());
        }
    }
}
