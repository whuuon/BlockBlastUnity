using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast.UI
{
    public sealed class CellView : MonoBehaviour
    {
        [SerializeField] private Image image;
        public void Set(Sprite sprite, Color tint)
        {
            image.sprite = sprite;
            image.color = tint;
        }

        public void SetTint(Color tint) => image.color = tint;
    }
}
