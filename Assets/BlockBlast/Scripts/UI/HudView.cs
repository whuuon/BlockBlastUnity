using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text bestText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Button pauseButton;
        private Coroutine feedbackRoutine;

        public void Refresh(int score, int best)
        {
            scoreText.text = score.ToString("N0");
            bestText.text = "KỶ LỤC  " + best.ToString("N0");
        }

        public void SetPauseEnabled(bool enabled) => pauseButton.interactable = enabled;

        public void ShowFeedback(string message, Color color)
        {
            if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
            feedbackRoutine = StartCoroutine(Feedback(message, color));
        }

        public void ClearFeedback()
        {
            if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
            feedbackText.text = string.Empty;
        }

        private IEnumerator Feedback(string message, Color color)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            yield return new WaitForSecondsRealtime(1.1f);
            feedbackText.text = string.Empty;
            feedbackRoutine = null;
        }
    }
}
