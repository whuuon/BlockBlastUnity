using System;
using BlockBlast.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast.UI
{
    public sealed class ScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private Text resultText;
        [SerializeField] private Text menuBestText;
        [SerializeField] private Text soundText;
        public event Action PlayRequested;
        public event Action PauseRequested;
        public event Action ResumeRequested;
        public event Action MenuRequested;
        public event Action SoundRequested;

        public void Play() => PlayRequested?.Invoke();
        public void Pause() => PauseRequested?.Invoke();
        public void Resume() => ResumeRequested?.Invoke();
        public void ReturnToMenu() => MenuRequested?.Invoke();
        public void ToggleSound() => SoundRequested?.Invoke();
        public void OpenHelp() => helpPanel.SetActive(true);
        public void CloseHelp() => helpPanel.SetActive(false);

        public void Show(GameState state, int score, int best)
        {
            menuPanel.SetActive(state == GameState.Menu);
            pausePanel.SetActive(state == GameState.Paused);
            gameOverPanel.SetActive(state == GameState.GameOver);
            helpPanel.SetActive(false);
            resultText.text = "ĐIỂM CỦA BẠN\n<size=62>" + score.ToString("N0") + "</size>\n\nKỶ LỤC  " + best.ToString("N0");
            menuBestText.text = "KỶ LỤC  " + best.ToString("N0");
        }

        public void SetSound(bool enabled) => soundText.text = enabled ? "ÂM THANH: BẬT" : "ÂM THANH: TẮT";
    }
}
