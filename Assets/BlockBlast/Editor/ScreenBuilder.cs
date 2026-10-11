using BlockBlast.Data;
using BlockBlast.UI;
using UnityEngine;
using UnityEngine.UI;
using static BlockBlast.Editor.UiFactory;

namespace BlockBlast.Editor
{
    public static class ScreenBuilder
    {
        public static void Create(RectTransform root, ScreenView screens, GameTheme theme, UiFactory ui)
        {
            RectTransform menu = ui.Overlay("MainMenu", root);
            Image logo = ui.Image("Logo", menu, theme.Logo, new Vector2(0, 300), new Vector2(300, 300));
            logo.preserveAspect = true;
            ui.Text("Tagline", menu, "MỘT KHỐI NHỎ. MỘT KỶ LỤC MỚI.", new Vector2(0, 130), new Vector2(650, 50), 25);
            Text menuBest = ui.Text("Best", menu, "KỶ LỤC  0", new Vector2(0, 45), new Vector2(550, 55), 30,
                new Color(1f, 0.82f, 0.25f));
            ui.Button("Play", menu, "CHƠI NGAY", new Vector2(0, -65), new Vector2(380, 100), screens.Play, 34);
            ui.Button("Help", menu, "CÁCH CHƠI", new Vector2(0, -190), new Vector2(330, 80), screens.OpenHelp, 26);
            Button sound = ui.Button("Sound", menu, "ÂM THANH: BẬT", new Vector2(0, -295), new Vector2(330, 70), screens.ToggleSound, 24);
            ui.Text("Footer", menu, "BLOCK BLAST  /  UNITY 2D", new Vector2(0, -540), new Vector2(650, 40), 20,
                new Color(0.64f, 0.74f, 0.93f));

            RectTransform pause = ui.Overlay("PauseMenu", root);
            ui.Text("Title", pause, "TẠM DỪNG", new Vector2(0, 220), new Vector2(600, 100), 48);
            ui.Button("Resume", pause, "TIẾP TỤC", new Vector2(0, 70), new Vector2(360, 95), screens.Resume, 30);
            ui.Button("Restart", pause, "CHƠI LẠI", new Vector2(0, -50), new Vector2(330, 80), screens.Play, 26);
            ui.Button("Home", pause, "VỀ TRANG CHỦ", new Vector2(0, -160), new Vector2(330, 80), screens.ReturnToMenu, 25);

            RectTransform gameOver = ui.Overlay("GameOver", root);
            ui.Text("Title", gameOver, "HẾT NƯỚC ĐI", new Vector2(0, 270), new Vector2(650, 100), 46,
                new Color(1f, 0.82f, 0.25f));
            Text result = ui.Text("Result", gameOver, "", new Vector2(0, 65), new Vector2(600, 260), 27);
            ui.Button("Retry", gameOver, "CHƠI LẠI", new Vector2(0, -160), new Vector2(360, 95), screens.Play, 31);
            ui.Button("Home", gameOver, "VỀ TRANG CHỦ", new Vector2(0, -280), new Vector2(330, 80), screens.ReturnToMenu, 25);

            RectTransform help = ui.Overlay("HowToPlay", root);
            ui.Text("Title", help, "CÁCH CHƠI", new Vector2(0, 335), new Vector2(600, 100), 46);
            Text rules = ui.Text("Rules", help,
                "1. Kéo một trong ba khối vào ô trống.\n\n2. Lấp đầy hàng hoặc cột để xóa chúng.\n\n3. Dùng hết ba khối để nhận bộ mới.\n\n4. Xóa liên tiếp để tăng combo.\n\n5. Ván chơi kết thúc khi không khối nào còn đặt được.",
                new Vector2(0, 20), new Vector2(570, 500), 27);
            rules.alignment = TextAnchor.UpperLeft;
            ui.Button("Close", help, "ĐÃ HIỂU", new Vector2(0, -355), new Vector2(350, 90), screens.CloseHelp, 30);

            Assign(screens, "menuPanel", menu.gameObject);
            Assign(screens, "pausePanel", pause.gameObject);
            Assign(screens, "gameOverPanel", gameOver.gameObject);
            Assign(screens, "helpPanel", help.gameObject);
            Assign(screens, "resultText", result);
            Assign(screens, "menuBestText", menuBest);
            Assign(screens, "soundText", sound.GetComponentInChildren<Text>());
            menu.gameObject.SetActive(true);
            pause.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            help.gameObject.SetActive(false);
        }
    }
}
