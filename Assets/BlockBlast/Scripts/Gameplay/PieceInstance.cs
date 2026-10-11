using BlockBlast.Core;

namespace BlockBlast.Gameplay
{
    public sealed class PieceInstance
    {
        public ShapeData Shape { get; }
        public int ColorIndex { get; }
        public PieceInstance(ShapeData shape, int colorIndex)
        {
            Shape = shape;
            ColorIndex = colorIndex;
        }
    }
}
