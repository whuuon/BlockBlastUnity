using UnityEngine;

namespace BlockBlast.Services
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioClip placeClip;
        [SerializeField] private AudioClip clearClip;
        [SerializeField] private AudioClip invalidClip;
        [SerializeField] private AudioClip gameOverClip;
        private AudioSource source;
        public bool SoundEnabled { get; private set; } = true;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
        }

        public void SetEnabled(bool enabled)
        {
            SoundEnabled = enabled;
            if (!enabled) source.Stop();
        }

        public void PlayPlace() => Play(placeClip);
        public void PlayClear() => Play(clearClip);
        public void PlayInvalid() => Play(invalidClip);
        public void PlayGameOver() => Play(gameOverClip);

        private void Play(AudioClip clip)
        {
            if (SoundEnabled && clip != null) source.PlayOneShot(clip, 0.65f);
        }
    }
}
