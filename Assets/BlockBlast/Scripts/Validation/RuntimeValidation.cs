#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BlockBlast.Core;
using BlockBlast.Gameplay;
using BlockBlast.Input;
using BlockBlast.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockBlast.Validation
{
    public sealed class RuntimeValidation : MonoBehaviour
    {
        private readonly List<string> results = new List<string>();
        private string output;
        private int savedBest;
        private bool hadBest;
        private int savedSound;
        private bool hadSound;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--smoke-test") < 0) return;
            new GameObject("RuntimeValidation").AddComponent<RuntimeValidation>();
        }

        private IEnumerator Start()
        {
            output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../BlockBlastUnity/Validation"));
            Directory.CreateDirectory(output);
            hadBest = PlayerPrefs.HasKey("BlockBlast.BestScore.v1");
            savedBest = PlayerPrefs.GetInt("BlockBlast.BestScore.v1", 0);
            hadSound = PlayerPrefs.HasKey("BlockBlast.SoundEnabled.v1");
            savedSound = PlayerPrefs.GetInt("BlockBlast.SoundEnabled.v1", 1);
            Application.logMessageReceived += HandleLog;
            yield return new WaitForSecondsRealtime(0.5f);
            GameController game = FindObjectOfType<GameController>();
            ScreenView screens = FindObjectOfType<ScreenView>();
            Require(game != null && game.State == GameState.Menu, "Scene loads into menu");
            yield return Capture("01-menu.png");
            Button play = GameObject.Find("GameCanvas/MainMenu/Play").GetComponent<Button>();
            play.onClick.Invoke();
            yield return null;
            Require(game.CanInteract && game.Tray.Length == 3, "Menu Play button starts a valid game");
            yield return Capture("02-gameplay.png");

            BoardView boardView = FindObjectOfType<BoardView>();
            PieceView piece = GameObject.Find("GameCanvas/Tray/PieceSlot1").GetComponent<PieceView>();
            PieceDragHandler drag = piece.GetComponent<PieceDragHandler>();
            int expectedCells = piece.Piece.Shape.Cells.Count;
            var pointer = new PointerEventData(EventSystem.current) { pointerId = -1 };
            pointer.position = RectTransformUtility.WorldToScreenPoint(null, piece.transform.position);
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            Require(hits.Exists(hit => hit.gameObject == piece.gameObject), "Piece hit area receives pointer events");
            ExecuteEvents.Execute(piece.gameObject, pointer, ExecuteEvents.beginDragHandler);
            Require(drag.IsDragging, "Begin-drag creates an active drag");
            Vector3 targetCenter = boardView.transform.TransformPoint(new Vector3(
                piece.Piece.Shape.Width * boardView.CellPitch * 0.5f,
                piece.Piece.Shape.Height * boardView.CellPitch * 0.5f));
            float canvasScale = FindObjectOfType<Canvas>().scaleFactor;
            pointer.position = RectTransformUtility.WorldToScreenPoint(null, targetCenter) - Vector2.up * 35f * canvasScale;
            ExecuteEvents.Execute(piece.gameObject, pointer, ExecuteEvents.dragHandler);
            yield return Capture("03-drag-preview.png");
            ExecuteEvents.Execute(piece.gameObject, pointer, ExecuteEvents.endDragHandler);
            yield return new WaitForSecondsRealtime(0.3f);
            Require(!drag.IsDragging && game.Score == expectedCells * 10, "Dropping through pointer events places and scores the shape");
            Require(game.Tray[0] == null, "Used tray slot is consumed exactly once");
            yield return Capture("04-placed.png");

            game.Pause();
            Require(game.State == GameState.Paused && !game.TryPlace(1, new GridPosition(4, 4)), "Pause blocks placements");
            yield return Capture("05-paused.png");
            screens.Resume();
            Require(game.CanInteract, "Resume restores input");
            screens.ToggleSound();
            screens.ToggleSound();
            Require(PlayerPrefs.GetInt("BlockBlast.SoundEnabled.v1", 1) == savedSound, "Sound toggle persists and restores setting");

            game.StartGame();
            Require(game.Score == 0 && game.Board[0, 0] == -1, "Restart resets score and board");
            for (int slot = 0; slot < 3; slot++)
            {
                game.Tray[slot] = new PieceInstance(new ShapeData(new GridPosition(0, 0)), slot);
                Require(game.TryPlace(slot, new GridPosition(slot, 0)), "Tray piece " + slot + " can be placed");
                yield return new WaitForSecondsRealtime(0.3f);
            }
            Require(Array.TrueForAll(game.Tray, item => item != null), "Using three pieces refills the tray");

            game.StartGame();
            ShapeData single = new ShapeData(new GridPosition(0, 0));
            for (int x = 1; x < 8; x++) game.Board.Place(single, new GridPosition(x, 0), 0);
            game.Tray[0] = new PieceInstance(single, 0);
            boardView.Render(game.Board);
            Require(game.TryPlace(0, new GridPosition(0, 0)), "Clear-triggering placement succeeds");
            Require(!game.CanInteract, "Input is locked during clear animation");
            yield return new WaitForSecondsRealtime(0.35f);
            Require(game.CanInteract && game.Board[0, 0] == -1 && game.Score == 110, "Clear animation resolves to empty row with correct score");

            game.StartGame();
            for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
                if ((x + y) % 2 == 0) game.Board.Place(single, new GridPosition(x, y), (x + y) % 3);
            var pair = new ShapeData(new GridPosition(0, 0), new GridPosition(1, 0));
            game.Tray[0] = new PieceInstance(single, 0);
            game.Tray[1] = new PieceInstance(pair, 1);
            game.Tray[2] = new PieceInstance(pair, 2);
            boardView.Render(game.Board);
            Require(game.TryPlace(0, new GridPosition(1, 0)), "Final legal move succeeds");
            yield return new WaitForSecondsRealtime(0.3f);
            Require(game.State == GameState.GameOver && !game.CanInteract, "Game over detects no remaining legal shape");
            yield return Capture("06-game-over.png");
            screens.Play();
            Require(game.CanInteract && game.Score == 0, "Game over retry starts a clean game");
            screens.ReturnToMenu();
            Require(game.State == GameState.Menu, "Home returns to menu");
            screens.OpenHelp();
            Require(GameObject.Find("GameCanvas/HowToPlay") != null, "Help opens from menu");
            yield return Capture("07-how-to-play.png");
            screens.CloseHelp();
            RestorePreferences();
            File.WriteAllLines(Path.Combine(output, "RuntimeChecks.txt"), results);
            Debug.Log("RUNTIME VALIDATION PASSED: " + results.Count + " checks.");
            Application.Quit(0);
        }

        private IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(output, name));
            yield return new WaitForSecondsRealtime(0.15f);
        }

        private void Require(bool condition, string name)
        {
            if (!condition) throw new InvalidOperationException("Runtime validation failed: " + name);
            results.Add("PASS | " + name);
        }

        private void RestorePreferences()
        {
            if (hadBest) PlayerPrefs.SetInt("BlockBlast.BestScore.v1", savedBest);
            else PlayerPrefs.DeleteKey("BlockBlast.BestScore.v1");
            if (hadSound) PlayerPrefs.SetInt("BlockBlast.SoundEnabled.v1", savedSound);
            else PlayerPrefs.DeleteKey("BlockBlast.SoundEnabled.v1");
            PlayerPrefs.Save();
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert) return;
            RestorePreferences();
            File.WriteAllText(Path.Combine(output, "RuntimeFailure.txt"), message + "\n" + stackTrace);
            Application.Quit(1);
        }

        private void OnDestroy() => Application.logMessageReceived -= HandleLog;
    }
}
#endif
