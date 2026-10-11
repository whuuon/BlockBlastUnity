using System;
using System.IO;
using BlockBlast.Data;
using BlockBlast.Gameplay;
using BlockBlast.Services;
using BlockBlast.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BlockBlast.Editor.UiFactory;

namespace BlockBlast.Editor
{
    public static class ProjectBuilder
    {
        [MenuItem("Block Blast/Build Scene From Figma Assets")]
        public static void BuildProject()
        {
            GameTheme theme = GameDataBuilder.CreateTheme();
            GameConfiguration configuration = GameDataBuilder.CreateConfiguration();
            var ui = new UiFactory(theme);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            var canvasObject = new GameObject("GameCanvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(720, 1280);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<CanvasFit>();
            RectTransform root = (RectTransform)canvasObject.transform;
            Image backdrop = ui.Image("Background", root, theme.Background, Vector2.zero, new Vector2(720, 1280));
            Stretch(backdrop.rectTransform);
            var systems = new GameObject("GameSystems");
            GameController controller = systems.AddComponent<GameController>();
            AudioService audio = systems.AddComponent<AudioService>();
            systems.GetComponent<AudioSource>().playOnAwake = false;
            BindAudio(audio);
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            ui.Text("Title", root, "BLOCK CRASH!", new Vector2(0, 510), new Vector2(660, 90), 56,
                new Color(1f, 0.82f, 0.25f));
            ui.Text("Subtitle", root, "XẾP KHỐI • PHÁ KỶ LỤC", new Vector2(0, 415), new Vector2(500, 40), 22,
                new Color(0.72f, 0.82f, 1f));
            HudView hud = canvasObject.AddComponent<HudView>();
            Text score = ui.Text("Score", root, "0", new Vector2(0, 340), new Vector2(330, 90), 66);
            Text best = ui.Text("BestScore", root, "KỶ LỤC  0", new Vector2(0, 278), new Vector2(400, 36), 23,
                new Color(1f, 0.82f, 0.25f));
            ui.Image("BoardBackground", root, theme.Board, new Vector2(0, -40), new Vector2(576, 576));
            RectTransform grid = ui.Rect("Grid", root, new Vector2(-272, -312), new Vector2(544, 544));
            grid.pivot = Vector2.zero;
            BoardView board = grid.gameObject.AddComponent<BoardView>();
            Assign(board, "gridRoot", grid);
            Assign(board, "cellPrefab", PrefabBuilder.CreateCell(theme, ui));
            Text feedback = ui.Text("MoveFeedback", root, "", new Vector2(0, -352), new Vector2(680, 48), 24);
            RectTransform trayRoot = ui.Rect("Tray", root, new Vector2(0, -445), new Vector2(660, 160));
            TrayView tray = trayRoot.gameObject.AddComponent<TrayView>();
            PieceView piecePrefab = PrefabBuilder.CreatePiece(theme, ui);
            var slots = new PieceView[3];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = (PieceView)PrefabUtility.InstantiatePrefab(piecePrefab, trayRoot);
                slots[i].name = "PieceSlot" + (i + 1);
                ((RectTransform)slots[i].transform).anchoredPosition = new Vector2((i - 1) * 210f, 0);
                slots[i].VisualRoot.localScale = Vector3.one * 0.5f;
                slots[i].gameObject.SetActive(false);
            }
            RectTransform dragLayer = ui.Rect("DragLayer", root, Vector2.zero, new Vector2(720, 1280));
            Stretch(dragLayer);
            AssignArray(tray, "slots", slots);
            Assign(tray, "dragLayer", dragLayer);
            ui.Text("DragHint", root, "Kéo khối vào bàn • Lấp đầy hàng hoặc cột", new Vector2(0, -550),
                new Vector2(660, 45), 22, new Color(0.7f, 0.8f, 0.98f));
            ScreenView screens = canvasObject.AddComponent<ScreenView>();
            Button pause = ui.Button("Pause", root, "TẠM DỪNG", new Vector2(240, 340), new Vector2(150, 64), screens.Pause, 19);
            Assign(hud, "scoreText", score);
            Assign(hud, "bestText", best);
            Assign(hud, "feedbackText", feedback);
            Assign(hud, "pauseButton", pause);
            ScreenBuilder.Create(root, screens, theme, ui);
            Assign(controller, "configuration", configuration);
            Assign(controller, "theme", theme);
            Assign(controller, "boardView", board);
            Assign(controller, "trayView", tray);
            Assign(controller, "hudView", hud);
            Assign(controller, "screenView", screens);
            Assign(controller, "audioService", audio);
            ConfigurePlayer();
            string scene = GameDataBuilder.Root + "/Scenes/Main.unity";
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scene);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scene, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Block Blast scene and prefabs created with original Figma sprites.");
        }

        [MenuItem("Block Blast/Build Windows Player")]
        public static void BuildWindows()
        {
            BuildPlayer(BuildOptions.None);
        }

        private static void BuildPlayer(BuildOptions options)
        {
            string destination = Path.GetFullPath("../BlockBlast-Windows/BlockBlast.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { GameDataBuilder.Root + "/Scenes/Main.unity" },
                locationPathName = destination,
                target = BuildTarget.StandaloneWindows64,
                options = options
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new InvalidOperationException("Windows build failed: " + report.summary.result);
            Debug.Log("Windows build succeeded: " + destination);
        }

        public static void ValidateAndBuildWindows()
        {
            Tests.CoreValidation.Run();
            BuildProject();
            BuildPlayer(BuildOptions.Development);
        }

        private static void ConfigurePlayer()
        {
            PlayerSettings.companyName = "UnityCourse";
            PlayerSettings.productName = "Block Blast";
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.colorSpace = ColorSpace.Gamma;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
            QualitySettings.vSyncCount = 1;
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, 0, -10);
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.055f, 0.2f);
        }

        private static void BindAudio(AudioService audio)
        {
            Assign(audio, "placeClip", AssetDatabase.LoadAssetAtPath<AudioClip>(GameDataBuilder.Root + "/Audio/place.wav"));
            Assign(audio, "clearClip", AssetDatabase.LoadAssetAtPath<AudioClip>(GameDataBuilder.Root + "/Audio/clear.wav"));
            Assign(audio, "invalidClip", AssetDatabase.LoadAssetAtPath<AudioClip>(GameDataBuilder.Root + "/Audio/invalid.wav"));
            Assign(audio, "gameOverClip", AssetDatabase.LoadAssetAtPath<AudioClip>(GameDataBuilder.Root + "/Audio/gameover.wav"));
        }
    }
}
