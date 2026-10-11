using UnityEngine;

namespace BlockBlast.Data
{
    [CreateAssetMenu(menuName = "Block Blast/Theme")]
    public sealed class GameTheme : ScriptableObject
    {
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite logo;
        [SerializeField] private Sprite board;
        [SerializeField] private Sprite button;
        [SerializeField] private Sprite emptyCell;
        [SerializeField] private Sprite[] blocks;
        [SerializeField] private Font font;
        public Sprite Background => background;
        public Sprite Logo => logo;
        public Sprite Board => board;
        public Sprite Button => button;
        public Sprite EmptyCell => emptyCell;
        public int ColorCount => blocks == null ? 0 : blocks.Length;
        public Font Font => font;
        public Sprite GetBlock(int index) => blocks[index % blocks.Length];

        public void Configure(Sprite backgroundSprite, Sprite logoSprite, Sprite boardSprite,
            Sprite buttonSprite, Sprite emptySprite, Sprite[] blockSprites, Font uiFont)
        {
            background = backgroundSprite;
            logo = logoSprite;
            board = boardSprite;
            button = buttonSprite;
            emptyCell = emptySprite;
            blocks = blockSprites;
            font = uiFont;
        }
    }
}
