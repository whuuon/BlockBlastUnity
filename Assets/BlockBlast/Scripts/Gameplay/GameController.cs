using System.Collections;
using BlockBlast.Core;
using BlockBlast.Data;
using BlockBlast.Services;
using BlockBlast.UI;
using UnityEngine;

namespace BlockBlast.Gameplay
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private GameConfiguration configuration;
        [SerializeField] private GameTheme theme;
        [SerializeField] private BoardView boardView;
        [SerializeField] private TrayView trayView;
        [SerializeField] private HudView hudView;
        [SerializeField] private ScreenView screenView;
        [SerializeField] private AudioService audioService;
        private BoardModel board;
        private readonly ScoreModel score = new ScoreModel();
        private readonly SaveService save = new SaveService();
        private PieceGenerator generator;
        private PieceInstance[] tray;
        private bool busy;
        public GameState State { get; private set; } = GameState.Menu;
        public bool CanInteract => State == GameState.Playing && !busy;
        public BoardModel Board => board;
        public PieceInstance[] Tray => tray;
        public int Score => score.Score;

        private void Awake()
        {
            board = new BoardModel(configuration.BoardSize);
            generator = new PieceGenerator(configuration, theme.ColorCount, System.Environment.TickCount);
            boardView.Initialize(configuration.BoardSize, theme);
            trayView.Initialize(this, boardView, theme);
            audioService.SetEnabled(save.SoundEnabled);
            screenView.SetSound(save.SoundEnabled);
            screenView.PlayRequested += StartGame;
            screenView.PauseRequested += Pause;
            screenView.ResumeRequested += Resume;
            screenView.MenuRequested += ReturnToMenu;
            screenView.SoundRequested += ToggleSound;
            boardView.Render(board);
            SetState(GameState.Menu);
        }

        private void OnDestroy()
        {
            screenView.PlayRequested -= StartGame;
            screenView.PauseRequested -= Pause;
            screenView.ResumeRequested -= Resume;
            screenView.MenuRequested -= ReturnToMenu;
            screenView.SoundRequested -= ToggleSound;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (State == GameState.Playing) Pause();
                else if (State == GameState.Paused) Resume();
            }
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused && State == GameState.Playing) Pause();
        }

        public void StartGame()
        {
            StopAllCoroutines();
            trayView.CancelDrag();
            busy = false;
            board.Reset();
            score.Reset();
            hudView.ClearFeedback();
            boardView.Render(board);
            tray = generator.CreateTray(board);
            trayView.Render(tray, board);
            SetState(GameState.Playing);
        }

        public bool CanPlace(PieceInstance piece, GridPosition origin) =>
            CanInteract && piece != null && board.CanPlace(piece.Shape, origin);

        public bool TryPlace(int slot, GridPosition origin)
        {
            if (!CanInteract || slot < 0 || slot >= tray.Length || tray[slot] == null) return false;
            PieceInstance piece = tray[slot];
            if (!board.CanPlace(piece.Shape, origin))
            {
                audioService.PlayInvalid();
                return false;
            }
            boardView.ShowPlaced(piece, origin);
            PlacementResult result = board.Place(piece.Shape, origin, piece.ColorIndex);
            tray[slot] = null;
            int earned = score.Apply(result);
            save.RecordScore(score.Score);
            busy = true;
            hudView.SetPauseEnabled(false);
            trayView.Render(tray, board);
            hudView.Refresh(score.Score, save.BestScore);
            StartCoroutine(CompletePlacement(result, earned));
            return true;
        }

        private IEnumerator CompletePlacement(PlacementResult result, int earned)
        {
            if (result.ClearedLines > 0)
            {
                audioService.PlayClear();
                hudView.ShowFeedback(result.ClearedLines + " HÀNG/CỘT  •  COMBO " + score.Combo + "  +" + earned,
                    new Color(1f, 0.82f, 0.2f));
                yield return boardView.AnimateClear(result);
            }
            else audioService.PlayPlace();
            boardView.Render(board);
            if (System.Array.TrueForAll(tray, piece => piece == null)) tray = generator.CreateTray(board);
            trayView.Render(tray, board);
            busy = false;
            hudView.SetPauseEnabled(true);
            bool anyMove = System.Array.Exists(tray, piece => piece != null && board.HasPlacement(piece.Shape));
            if (!anyMove)
            {
                audioService.PlayGameOver();
                SetState(GameState.GameOver);
            }
        }

        public void Pause()
        {
            if (!CanInteract) return;
            trayView.CancelDrag();
            SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (State == GameState.Paused) SetState(GameState.Playing);
        }

        public void ReturnToMenu()
        {
            StopAllCoroutines();
            trayView.CancelDrag();
            busy = false;
            boardView.Render(board);
            hudView.ClearFeedback();
            SetState(GameState.Menu);
        }

        private void ToggleSound()
        {
            bool enabled = !audioService.SoundEnabled;
            audioService.SetEnabled(enabled);
            save.SetSound(enabled);
            screenView.SetSound(enabled);
        }

        private void SetState(GameState state)
        {
            State = state;
            hudView.Refresh(score.Score, save.BestScore);
            hudView.SetPauseEnabled(state == GameState.Playing && !busy);
            screenView.Show(state, score.Score, save.BestScore);
        }
    }
}
