using BlockBlast.Core;
using BlockBlast.Data;
using BlockBlast.Gameplay;
using BlockBlast.Input;
using UnityEngine;

namespace BlockBlast.UI
{
    public sealed class TrayView : MonoBehaviour
    {
        [SerializeField] private PieceView[] slots;
        [SerializeField] private RectTransform dragLayer;
        private GameTheme theme;
        private float pitch;

        public void Initialize(GameController controller, BoardView board, GameTheme gameTheme)
        {
            theme = gameTheme;
            pitch = board.CellPitch;
            foreach (PieceView slot in slots)
                slot.GetComponent<PieceDragHandler>().Initialize(controller, board, dragLayer);
        }

        public void Render(PieceInstance[] pieces, BoardModel board)
        {
            for (int index = 0; index < slots.Length; index++)
            {
                slots[index].Bind(pieces[index], index, theme, pitch);
                if (pieces[index] != null) slots[index].SetAvailable(board.HasPlacement(pieces[index].Shape));
            }
        }

        public void CancelDrag()
        {
            foreach (PieceView slot in slots) slot.GetComponent<PieceDragHandler>().CancelDrag();
        }
    }
}
