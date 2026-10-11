using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class CanvasFit : MonoBehaviour
    {
        private CanvasScaler scaler;
        private void Awake() => scaler = GetComponent<CanvasScaler>();
        private void Update()
        {
            float match = Screen.width / (float)Mathf.Max(1, Screen.height) < 720f / 1280f ? 0f : 1f;
            if (!Mathf.Approximately(scaler.matchWidthOrHeight, match)) scaler.matchWidthOrHeight = match;
        }
    }
}
