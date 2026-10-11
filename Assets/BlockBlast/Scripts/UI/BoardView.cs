using System.Collections;
using BlockBlast.Core;
using BlockBlast.Data;
using BlockBlast.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast.UI
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private CellView cellPrefab;
        [SerializeField] private float cellPitch = 68f;
        [SerializeField] private float cellSize = 64f;
        private CellView[,] cells;
        private CellView[,] preview;
        private GameTheme theme;
        private int size;
        public float CellPitch => cellPitch;

        public void Initialize(int boardSize, GameTheme gameTheme)
        {
            size = boardSize;
            cellPitch = gridRoot.rect.width / size;
            cellSize = Mathf.Max(4f, cellPitch - 4f);
            theme = gameTheme;
            cells = new CellView[size, size];
            preview = new CellView[size, size];
            for (int row = 0; row < size; row++)
                for (int column = 0; column < size; column++)
                {
                    cells[column, row] = CreateCell(column, row);
                    preview[column, row] = CreateCell(column, row);
                    preview[column, row].gameObject.SetActive(false);
                }
        }

        private CellView CreateCell(int column, int row)
        {
            CellView view = Instantiate(cellPrefab, gridRoot);
            var rect = (RectTransform)view.transform;
            rect.anchorMin = rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.one * cellSize;
            rect.anchoredPosition = new Vector2(column + 0.5f, row + 0.5f) * cellPitch;
            view.GetComponent<Image>().raycastTarget = false;
            return view;
        }

        public void Render(BoardModel board)
        {
            for (int row = 0; row < size; row++)
                for (int column = 0; column < size; column++)
                {
                    int color = board[column, row];
                    cells[column, row].transform.localScale = Vector3.one;
                    cells[column, row].Set(color < 0 ? theme.EmptyCell : theme.GetBlock(color), Color.white);
                }
        }

        public bool TryGetOrigin(Vector2 screenPoint, Camera eventCamera, out GridPosition origin)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRoot, screenPoint, eventCamera,
                out Vector2 local);
            origin = new GridPosition(Mathf.RoundToInt(local.x / cellPitch - 0.5f),
                Mathf.RoundToInt(local.y / cellPitch - 0.5f));
            return local.x >= -cellPitch * 0.5f && local.y >= -cellPitch * 0.5f &&
                local.x <= size * cellPitch + cellPitch * 0.5f &&
                local.y <= size * cellPitch + cellPitch * 0.5f;
        }

        public void ShowPreview(PieceInstance piece, GridPosition origin, bool valid)
        {
            ClearPreview();
            foreach (GridPosition offset in piece.Shape.Cells)
            {
                GridPosition cell = origin + offset;
                if (cell.column < 0 || cell.row < 0 || cell.column >= size || cell.row >= size) continue;
                CellView view = preview[cell.column, cell.row];
                view.gameObject.SetActive(true);
                view.Set(theme.GetBlock(piece.ColorIndex), valid
                    ? new Color(0.65f, 1f, 0.7f, 0.75f) : new Color(1f, 0.25f, 0.35f, 0.8f));
                view.transform.SetAsLastSibling();
            }
        }

        public void ClearPreview()
        {
            if (preview == null) return;
            foreach (CellView cell in preview) cell.gameObject.SetActive(false);
        }

        public void ShowPlaced(PieceInstance piece, GridPosition origin)
        {
            foreach (GridPosition offset in piece.Shape.Cells)
            {
                GridPosition cell = origin + offset;
                cells[cell.column, cell.row].Set(theme.GetBlock(piece.ColorIndex), Color.white);
            }
        }

        public IEnumerator AnimateClear(PlacementResult result)
        {
            const float duration = 0.22f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float scale = Mathf.Lerp(1f, 0.2f, elapsed / duration);
                foreach (GridPosition cell in result.ClearedCells)
                {
                    CellView view = cells[cell.column, cell.row];
                    view.transform.localScale = Vector3.one * scale;
                    view.SetTint(new Color(1f, 1f, 1f, 1f - elapsed / duration));
                }
                yield return null;
            }
            foreach (GridPosition cell in result.ClearedCells)
                cells[cell.column, cell.row].transform.localScale = Vector3.one;
        }
    }
}
