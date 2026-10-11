using BlockBlast.Core;
using BlockBlast.Gameplay;
using BlockBlast.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlockBlast.Input
{
    [RequireComponent(typeof(PieceView))]
    public sealed class PieceDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private PieceView view;
        private GameController controller;
        private BoardView board;
        private RectTransform dragLayer;
        private Transform homeParent;
        private Vector2 homePosition;
        private Vector3 homeScale;
        private GridPosition origin;
        private bool overBoard;
        private bool dragging;
        public bool IsDragging => dragging;

        public void Initialize(GameController gameController, BoardView boardView, RectTransform layer)
        {
            view = GetComponent<PieceView>();
            controller = gameController;
            board = boardView;
            dragLayer = layer;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (controller == null || !controller.CanInteract || view.Piece == null) return;
            RectTransform visual = view.VisualRoot;
            homeParent = visual.parent;
            homePosition = visual.anchoredPosition;
            homeScale = visual.localScale;
            visual.SetParent(dragLayer, false);
            visual.localScale = Vector3.one;
            dragging = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            if (!controller.CanInteract) { CancelDrag(); return; }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragLayer, eventData.position,
                eventData.pressEventCamera, out Vector2 point);
            float lift = eventData.pointerId >= 0 ? 90f : 35f;
            view.VisualRoot.anchoredPosition = point + Vector2.up * lift;
            Vector3 bottomLeft = view.VisualRoot.TransformPoint(
                new Vector3(view.VisualRoot.rect.xMin + board.CellPitch * 0.5f,
                    view.VisualRoot.rect.yMin + board.CellPitch * 0.5f));
            Vector2 anchorScreen = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, bottomLeft);
            overBoard = board.TryGetOrigin(anchorScreen, eventData.pressEventCamera, out origin);
            if (overBoard) board.ShowPreview(view.Piece, origin, controller.CanPlace(view.Piece, origin));
            else board.ClearPreview();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            OnDrag(eventData);
            if (!dragging) return;
            bool shouldPlace = overBoard;
            int slot = view.Slot;
            GridPosition target = origin;
            CancelDrag();
            if (shouldPlace) controller.TryPlace(slot, target);
        }

        public void CancelDrag()
        {
            if (!dragging) return;
            dragging = false;
            view.VisualRoot.SetParent(homeParent, false);
            view.VisualRoot.anchoredPosition = homePosition;
            view.VisualRoot.localScale = homeScale;
            board.ClearPreview();
        }

        private void OnDisable() => CancelDrag();
    }
}
