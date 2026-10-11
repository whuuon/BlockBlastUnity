using BlockBlast.Core;
using UnityEngine;

namespace BlockBlast.Data
{
    [CreateAssetMenu(menuName = "Block Blast/Shape")]
    public sealed class ShapeDefinition : ScriptableObject
    {
        [SerializeField] private GridPosition[] cells;
        public ShapeData CreateData() => new ShapeData(cells);
        public void Configure(GridPosition[] positions) => cells = positions;
    }
}
