using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlockBlast.Core;
using BlockBlast.Data;
using UnityEditor;
using UnityEngine;

namespace BlockBlast.Editor
{
    public static class GameDataBuilder
    {
        public const string Root = "Assets/BlockBlast";

        public static GameTheme CreateTheme()
        {
            foreach (string directory in new[] { "Art/Figma", "Audio", "Data/Shapes", "Prefabs", "Scenes" })
                Directory.CreateDirectory(Root + "/" + directory);
            AssetDatabase.Refresh();
            string[] files = Directory.GetFiles(Root + "/Art/Figma", "*.png", SearchOption.AllDirectories);
            Array.Sort(files, StringComparer.Ordinal);
            foreach (string file in files)
            {
                string path = file.Replace('\\', '/');
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 4096;
                string assetName = Path.GetFileNameWithoutExtension(path);
                if (assetName == "empty_cell")
                    Slice(importer, assetName, new Rect(12, 576, 40, 42));
                if (assetName == "background")
                    Slice(importer, assetName, new Rect(0, 1252, 941, 700));
                if (Path.GetFileNameWithoutExtension(path) == "button")
                    importer.spriteBorder = new Vector4(180, 180, 180, 180);
                importer.SaveAndReimport();
            }
            var blocks = new List<Sprite>();
            foreach (string file in files)
                if (Path.GetFileName(file).StartsWith("block_", StringComparison.Ordinal))
                    blocks.Add(AssetDatabase.LoadAssetAtPath<Sprite>(file.Replace('\\', '/')));
            if (blocks.Count == 0) throw new FileNotFoundException("Original Figma block_*.png sprites are required in Art/Figma.");
            GameTheme theme = LoadOrCreate<GameTheme>(Root + "/Data/Theme.asset");
            theme.Configure(Sprite("background"), Sprite("logo"), Sprite("board"), Sprite("button"),
                Sprite("empty_cell"), blocks.ToArray(), Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
            EditorUtility.SetDirty(theme);
            return theme;
        }

        public static GameConfiguration CreateConfiguration()
        {
            var shapes = new List<ShapeDefinition>();
            string[] descriptions = {
                "Single:0,0", "Horizontal2:0,0 1,0", "Horizontal3:0,0 1,0 2,0", "Horizontal4:0,0 1,0 2,0 3,0",
                "Horizontal5:0,0 1,0 2,0 3,0 4,0", "Vertical2:0,0 0,1", "Vertical3:0,0 0,1 0,2",
                "Vertical4:0,0 0,1 0,2 0,3", "Vertical5:0,0 0,1 0,2 0,3 0,4", "Square2:0,0 1,0 0,1 1,1",
                "Square3:0,0 1,0 2,0 0,1 1,1 2,1 0,2 1,2 2,2", "Rectangle3x2:0,0 1,0 2,0 0,1 1,1 2,1",
                "Rectangle2x3:0,0 1,0 0,1 1,1 0,2 1,2", "CornerLeft:0,0 0,1 1,0", "CornerRight:0,0 1,0 1,1",
                "CornerUpperLeft:0,0 0,1 1,1", "CornerUpperRight:0,1 1,0 1,1", "LLeft:0,0 0,1 0,2 1,0",
                "LRight:0,0 1,0 1,1 1,2", "TUp:0,0 1,0 2,0 1,1", "TDown:0,1 1,1 2,1 1,0",
                "ZigLeft:0,0 1,0 1,1 2,1", "ZigRight:0,1 1,1 1,0 2,0" };
            foreach (string description in descriptions)
            {
                string[] parts = description.Split(':');
                string[] pairs = parts[1].Split(' ');
                var cells = new GridPosition[pairs.Length];
                for (int i = 0; i < pairs.Length; i++)
                {
                    string[] coordinate = pairs[i].Split(',');
                    cells[i] = new GridPosition(int.Parse(coordinate[0]), int.Parse(coordinate[1]));
                }
                ShapeDefinition definition = LoadOrCreate<ShapeDefinition>(Root + "/Data/Shapes/" + parts[0] + ".asset");
                definition.Configure(cells);
                EditorUtility.SetDirty(definition);
                shapes.Add(definition);
            }
            GameConfiguration configuration = LoadOrCreate<GameConfiguration>(Root + "/Data/GameConfiguration.asset");
            configuration.Configure(shapes.ToArray());
            EditorUtility.SetDirty(configuration);
            return configuration;
        }

        private static Sprite Sprite(string name)
        {
            string path = Root + "/Art/Figma/" + name + ".png";
            Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
            if (sprite == null) throw new FileNotFoundException("Missing original Figma sprite: " + path);
            return sprite;
        }

        private static void Slice(TextureImporter importer, string name, Rect rect)
        {
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritesheet = new[] { new SpriteMetaData
            {
                name = name, rect = rect, alignment = (int)SpriteAlignment.Center, pivot = Vector2.one * 0.5f
            } };
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }
}
