using System;
using System.Collections.Generic;
using System.IO;
using BlockBlast.Core;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor.Tests
{
    public static class CoreValidation
    {
        private static readonly ShapeData Single = new ShapeData(new GridPosition(0, 0));
        private static readonly List<string> Results = new List<string>();

        [MenuItem("Block Blast/Validate Core Rules")]
        public static void Run()
        {
            Results.Clear();
            Check("Empty board accepts a piece", () => Require(new BoardModel(8).CanPlace(Single, new GridPosition(7, 7))));
            Check("Negative and overflowing placement are rejected", () =>
            {
                var board = new BoardModel(8);
                Require(!board.CanPlace(Single, new GridPosition(-1, 0)));
                Require(!board.CanPlace(Single, new GridPosition(8, 0)));
                Require(!board.CanPlace(new ShapeData(new GridPosition(0, 0), new GridPosition(1, 0)), new GridPosition(7, 0)));
            });
            Check("Occupied cells reject overlap without mutation", () =>
            {
                var board = new BoardModel(8);
                board.Place(Single, new GridPosition(2, 2), 3);
                Require(!board.Place(Single, new GridPosition(2, 2), 1).Succeeded);
                Require(board[2, 2] == 3);
            });
            Check("Invalid multi-cell placement is atomic", () =>
            {
                var board = new BoardModel(8);
                var pair = new ShapeData(new GridPosition(0, 0), new GridPosition(1, 0));
                Require(!board.Place(pair, new GridPosition(7, 0), 1).Succeeded);
                Require(board[7, 0] == -1);
            });
            Check("Full row clears without gravity", () =>
            {
                var board = new BoardModel(4);
                board.Place(Single, new GridPosition(1, 2), 2);
                for (int x = 0; x < 3; x++) board.Place(Single, new GridPosition(x, 0), 0);
                PlacementResult result = board.Place(Single, new GridPosition(3, 0), 0);
                Require(result.ClearedLines == 1 && result.ClearedCells.Count == 4);
                Require(board[1, 2] == 2 && board[1, 0] == -1);
            });
            Check("Full column clears", () =>
            {
                var board = new BoardModel(4);
                for (int y = 0; y < 3; y++) board.Place(Single, new GridPosition(0, y), 0);
                PlacementResult result = board.Place(Single, new GridPosition(0, 3), 0);
                Require(result.ClearedLines == 1 && result.ClearedCells.Count == 4 && board[0, 2] == -1);
            });
            Check("Crossing row and column clear simultaneously, intersection once", () =>
            {
                var board = new BoardModel(4);
                for (int i = 1; i < 4; i++)
                {
                    board.Place(Single, new GridPosition(i, 0), 0);
                    board.Place(Single, new GridPosition(0, i), 0);
                }
                PlacementResult result = board.Place(Single, new GridPosition(0, 0), 0);
                Require(result.ClearedLines == 2 && result.ClearedCells.Count == 7);
                for (int i = 0; i < 4; i++) Require(board[i, 0] == -1 && board[0, i] == -1);
            });
            Check("Two full rows clear in one placement", () =>
            {
                var board = new BoardModel(4);
                for (int y = 0; y < 2; y++)
                    for (int x = 0; x < 3; x++) board.Place(Single, new GridPosition(x, y), 0);
                PlacementResult result = board.Place(new ShapeData(new GridPosition(0, 0), new GridPosition(0, 1)), new GridPosition(3, 0), 1);
                Require(result.ClearedLines == 2 && result.ClearedCells.Count == 8);
            });
            Check("No-move detection respects remaining shape geometry", () =>
            {
                var board = new BoardModel(4);
                for (int y = 0; y < 4; y++)
                    for (int x = 0; x < 4; x++)
                        if ((x + y) % 2 == 0) board.Place(Single, new GridPosition(x, y), 0);
                Require(board.HasPlacement(Single));
                Require(!board.HasPlacement(new ShapeData(new GridPosition(0, 0), new GridPosition(1, 0))));
            });
            Check("Reset empties every cell", () =>
            {
                var board = new BoardModel(8);
                board.Place(Single, new GridPosition(4, 4), 0);
                board.Reset();
                for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++) Require(board[x, y] == -1);
            });
            Check("Placement scores ten per cell", () =>
            {
                var score = new ScoreModel();
                Require(score.Apply(new PlacementResult(true, 4)) == 40 && score.Score == 40);
            });
            Check("Consecutive clears multiply line bonus", () =>
            {
                var score = new ScoreModel();
                Require(score.Apply(new PlacementResult(true, 1, 1)) == 110);
                Require(score.Apply(new PlacementResult(true, 2, 2)) == 420);
                Require(score.Combo == 2 && score.Score == 530);
            });
            Check("Non-clear resets combo; invalid move keeps score and combo", () =>
            {
                var score = new ScoreModel();
                score.Apply(new PlacementResult(true, 1, 1));
                Require(score.Apply(new PlacementResult(false)) == 0 && score.Combo == 1 && score.Score == 110);
                score.Apply(new PlacementResult(true, 1));
                Require(score.Combo == 0 && score.Score == 120);
                score.Reset();
                Require(score.Combo == 0 && score.Score == 0);
            });
            Check("Malformed shapes fail early", () =>
            {
                RequireThrows(() => new ShapeData());
                RequireThrows(() => new ShapeData(new GridPosition(-1, 0)));
                RequireThrows(() => new ShapeData(new GridPosition(0, 0), new GridPosition(0, 0)));
            });
            Check("Deterministic 2000-turn simulation preserves board invariants", Simulate);
            string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "../Validation"));
            Directory.CreateDirectory(directory);
            File.WriteAllLines(Path.Combine(directory, "CoreChecks.txt"), Results);
            Debug.Log("CORE VALIDATION PASSED: " + Results.Count + " checks.");
        }

        private static void Simulate()
        {
            var random = new System.Random(2701);
            var board = new BoardModel(8);
            var shapes = new[] { Single,
                new ShapeData(new GridPosition(0, 0), new GridPosition(1, 0)),
                new ShapeData(new GridPosition(0, 0), new GridPosition(0, 1), new GridPosition(1, 0)),
                new ShapeData(new GridPosition(0, 0), new GridPosition(1, 0), new GridPosition(0, 1), new GridPosition(1, 1)) };
            for (int turn = 0; turn < 2000; turn++)
            {
                ShapeData shape = shapes[random.Next(shapes.Length)];
                var moves = new List<GridPosition>();
                for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
                    if (board.CanPlace(shape, new GridPosition(x, y))) moves.Add(new GridPosition(x, y));
                Require(board.HasPlacement(shape) == (moves.Count > 0));
                if (moves.Count == 0) { board.Reset(); continue; }
                PlacementResult result = board.Place(shape, moves[random.Next(moves.Count)], random.Next(6));
                Require(result.Succeeded);
                foreach (GridPosition cell in result.ClearedCells) Require(board[cell.column, cell.row] == -1);
                for (int i = 0; i < 8; i++)
                {
                    int rowFilled = 0, columnFilled = 0;
                    for (int scan = 0; scan < 8; scan++)
                    {
                        if (board[scan, i] >= 0) rowFilled++;
                        if (board[i, scan] >= 0) columnFilled++;
                    }
                    Require(rowFilled < 8 && columnFilled < 8);
                }
            }
        }

        private static void Check(string name, Action action)
        {
            action();
            Results.Add("PASS | " + name);
        }

        private static void Require(bool condition)
        {
            if (!condition) throw new InvalidOperationException("Core validation failed.");
        }

        private static void RequireThrows(Action action)
        {
            try { action(); }
            catch (ArgumentException) { return; }
            throw new InvalidOperationException("Expected an argument validation error.");
        }
    }
}
