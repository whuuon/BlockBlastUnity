using BlockBlast.Data;
using BlockBlast.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast.UI
{
    public sealed class PieceView : MonoBehaviour
    {
        [SerializeField] private RectTransform visualRoot;
        [SerializeField] private Image tilePrefab;
        [SerializeField] private CanvasGroup canvasGroup;
        private readonly System.Collections.Generic.List<Image> tiles = new System.Collections.Generic.List<Image>();
        public RectTransform VisualRoot => visualRoot;
        public PieceInstance Piece { get; private set; }
        public int Slot { get; private set; }

        public void Bind(PieceInstance piece, int slot, GameTheme theme, float pitch)
        {
            Piece = piece;
            Slot = slot;
            foreach (Image tile in tiles) Destroy(tile.gameObject);
            tiles.Clear();
            gameObject.SetActive(piece != null);
            if (piece == null) return;
            visualRoot.sizeDelta = new Vector2(piece.Shape.Width, piece.Shape.Height) * pitch;
            foreach (var cell in piece.Shape.Cells)
            {
                Image tile = Instantiate(tilePrefab, visualRoot);
                tiles.Add(tile);
                tile.raycastTarget = false;
                tile.sprite = theme.GetBlock(piece.ColorIndex);
                tile.rectTransform.anchorMin = tile.rectTransform.anchorMax = Vector2.zero;
                tile.rectTransform.pivot = Vector2.one * 0.5f;
                tile.rectTransform.sizeDelta = Vector2.one * (pitch - 4f);
                tile.rectTransform.anchoredPosition = new Vector2(cell.column + 0.5f, cell.row + 0.5f) * pitch;
            }
            SetAvailable(true);
        }

        public void SetAvailable(bool available) => canvasGroup.alpha = available ? 1f : 0.35f;
    }
}
