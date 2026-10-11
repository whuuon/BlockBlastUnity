using System;
using BlockBlast.Core;
using BlockBlast.Data;

namespace BlockBlast.Gameplay
{
    public sealed class PieceGenerator
    {
        private readonly GameConfiguration configuration;
        private readonly Random random;
        private readonly int colorCount;

        public PieceGenerator(GameConfiguration configuration, int colorCount, int seed)
        {
            this.configuration = configuration;
            this.colorCount = colorCount;
            random = new Random(seed);
        }

        public PieceInstance[] CreateTray(BoardModel board)
        {
            var tray = new PieceInstance[3];
            var fitting = new System.Collections.Generic.List<ShapeData>();
            foreach (ShapeDefinition definition in configuration.Shapes)
            {
                ShapeData shape = definition.CreateData();
                if (board.HasPlacement(shape)) fitting.Add(shape);
            }
            for (int index = 0; index < tray.Length; index++)
            {
                ShapeData shape = index == 0 && fitting.Count > 0
                    ? fitting[random.Next(fitting.Count)]
                    : configuration.Shapes[random.Next(configuration.Shapes.Length)].CreateData();
                tray[index] = new PieceInstance(shape, random.Next(colorCount));
            }
            return tray;
        }
    }
}
