using BlockBlast.Data;
using BlockBlast.Input;
using BlockBlast.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static BlockBlast.Editor.UiFactory;

namespace BlockBlast.Editor
{
    public static class PrefabBuilder
    {
        public static CellView CreateCell(GameTheme theme, UiFactory ui)
        {
            Image image = ui.Image("BoardCell", null, theme.EmptyCell, Vector2.zero, Vector2.one * 64f);
            CellView view = image.gameObject.AddComponent<CellView>();
            Assign(view, "image", image);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(view.gameObject, GameDataBuilder.Root + "/Prefabs/BoardCell.prefab");
            Object.DestroyImmediate(view.gameObject);
            return prefab.GetComponent<CellView>();
        }

        public static PieceView CreatePiece(GameTheme theme, UiFactory ui)
        {
            Image tile = ui.Image("PieceTile", null, theme.GetBlock(0), Vector2.zero, Vector2.one * 64);
            GameObject tilePrefab = PrefabUtility.SaveAsPrefabAsset(tile.gameObject, GameDataBuilder.Root + "/Prefabs/PieceTile.prefab");
            Object.DestroyImmediate(tile.gameObject);
            RectTransform root = ui.Rect("DraggablePiece", null, Vector2.zero, new Vector2(190, 180));
            Image hitArea = root.gameObject.AddComponent<Image>();
            hitArea.color = Color.clear;
            hitArea.raycastTarget = true;
            CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
            RectTransform visuals = ui.Rect("Tiles", root, Vector2.zero, Vector2.one * 68);
            PieceView view = root.gameObject.AddComponent<PieceView>();
            root.gameObject.AddComponent<PieceDragHandler>();
            Assign(view, "visualRoot", visuals);
            Assign(view, "tilePrefab", tilePrefab.GetComponent<Image>());
            Assign(view, "canvasGroup", group);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root.gameObject, GameDataBuilder.Root + "/Prefabs/DraggablePiece.prefab");
            Object.DestroyImmediate(root.gameObject);
            return prefab.GetComponent<PieceView>();
        }
    }
}
