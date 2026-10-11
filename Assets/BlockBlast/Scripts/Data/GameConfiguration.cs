using UnityEngine;

namespace BlockBlast.Data
{
    [CreateAssetMenu(menuName = "Block Blast/Configuration")]
    public sealed class GameConfiguration : ScriptableObject
    {
        [SerializeField, Range(4, 12)] private int boardSize = 8;
        [SerializeField] private ShapeDefinition[] shapes;
        public int BoardSize => boardSize;
        public ShapeDefinition[] Shapes => shapes;
        public void Configure(ShapeDefinition[] definitions) => shapes = definitions;
    }
}
