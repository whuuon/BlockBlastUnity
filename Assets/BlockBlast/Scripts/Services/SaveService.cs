using UnityEngine;

namespace BlockBlast.Services
{
    public sealed class SaveService
    {
        private const string BestKey = "BlockBlast.BestScore.v1";
        private const string SoundKey = "BlockBlast.SoundEnabled.v1";
        public int BestScore => PlayerPrefs.GetInt(BestKey, 0);
        public bool SoundEnabled => PlayerPrefs.GetInt(SoundKey, 1) == 1;

        public int RecordScore(int score)
        {
            int best = Mathf.Max(score, BestScore);
            PlayerPrefs.SetInt(BestKey, best);
            PlayerPrefs.Save();
            return best;
        }

        public void SetSound(bool enabled)
        {
            PlayerPrefs.SetInt(SoundKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
