namespace BlockBlast.Core
{
    public sealed class ScoreModel
    {
        public int Score { get; private set; }
        public int Combo { get; private set; }

        public void Reset()
        {
            Score = 0;
            Combo = 0;
        }

        public int Apply(PlacementResult result)
        {
            if (!result.Succeeded) return 0;
            Combo = result.ClearedLines > 0 ? Combo + 1 : 0;
            int earned = result.PlacedCells * 10 + result.ClearedLines * 100 * Combo;
            Score += earned;
            return earned;
        }
    }
}
