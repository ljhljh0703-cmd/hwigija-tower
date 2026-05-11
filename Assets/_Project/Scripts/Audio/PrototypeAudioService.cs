using UnityEngine;

namespace HwigiTower.Audio
{
    public sealed class PrototypeAudioService : MonoBehaviour
    {
        [SerializeField] private AudioCueCatalog cueCatalog;
        [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.75f;
        [SerializeField, Range(0f, 1f)] private float ambienceVolume = 0.65f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.85f;

        private AudioSource _bgmSource;
        private AudioSource _ambienceSource;
        private AudioSource _sfxSource;

        public static PrototypeAudioService Instance { get; private set; }
        public float BgmVolume => bgmVolume;
        public float AmbienceVolume => ambienceVolume;
        public float SfxVolume => sfxVolume;
        public PrototypeAudioContext LastContext { get; private set; }

        public static PrototypeAudioService GetOrCreate()
        {
            if (Instance != null)
            {
                return Instance;
            }

            var existing = FindFirstObjectByType<PrototypeAudioService>();
            if (existing != null)
            {
                return existing;
            }

            return new GameObject("Prototype Audio Service").AddComponent<PrototypeAudioService>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureSources();
            ApplyVolumes();
        }

        public void Configure(AudioCueCatalog catalog)
        {
            cueCatalog = catalog;
        }

        public void SetBgmVolume(float value)
        {
            bgmVolume = Mathf.Clamp01(value);
            ApplyVolumes();
        }

        public void SetAmbienceVolume(float value)
        {
            ambienceVolume = Mathf.Clamp01(value);
            ApplyVolumes();
        }

        public void SetSfxVolume(float value)
        {
            sfxVolume = Mathf.Clamp01(value);
            ApplyVolumes();
        }

        public void PlayContext(PrototypeAudioContext context)
        {
            LastContext = context;
            if (cueCatalog == null || !cueCatalog.TryGetCue(context, out var cue))
            {
                return;
            }

            PlayCue(cue);
        }

        public void PlayCue(AudioCueData cue)
        {
            if (cue == null || !cue.HasClip)
            {
                return;
            }

            EnsureSources();
            var source = ResolveSource(cue.Channel);
            if (source == null)
            {
                return;
            }

            if (cue.Channel == PrototypeAudioChannel.Sfx)
            {
                source.PlayOneShot(cue.Clip, cue.Volume * sfxVolume);
                return;
            }

            if (source.clip == cue.Clip && source.isPlaying)
            {
                return;
            }

            source.clip = cue.Clip;
            source.loop = cue.Loop;
            source.volume = cue.Volume * ResolveChannelVolume(cue.Channel);
            source.Play();
        }

        private AudioSource ResolveSource(PrototypeAudioChannel channel)
        {
            return channel switch
            {
                PrototypeAudioChannel.Bgm => _bgmSource,
                PrototypeAudioChannel.Ambience => _ambienceSource,
                PrototypeAudioChannel.Sfx => _sfxSource,
                _ => null
            };
        }

        private float ResolveChannelVolume(PrototypeAudioChannel channel)
        {
            return channel switch
            {
                PrototypeAudioChannel.Bgm => bgmVolume,
                PrototypeAudioChannel.Ambience => ambienceVolume,
                PrototypeAudioChannel.Sfx => sfxVolume,
                _ => 1f
            };
        }

        private void EnsureSources()
        {
            _bgmSource = EnsureSource(_bgmSource, "BGM Channel");
            _ambienceSource = EnsureSource(_ambienceSource, "Ambience Channel");
            _sfxSource = EnsureSource(_sfxSource, "SFX Channel");
        }

        private AudioSource EnsureSource(AudioSource source, string name)
        {
            if (source != null)
            {
                return source;
            }

            var child = new GameObject(name);
            child.transform.SetParent(transform, false);
            var created = child.AddComponent<AudioSource>();
            created.playOnAwake = false;
            return created;
        }

        private void ApplyVolumes()
        {
            bgmVolume = Mathf.Clamp01(bgmVolume);
            ambienceVolume = Mathf.Clamp01(ambienceVolume);
            sfxVolume = Mathf.Clamp01(sfxVolume);
            if (_bgmSource != null)
            {
                _bgmSource.volume = bgmVolume;
            }

            if (_ambienceSource != null)
            {
                _ambienceSource.volume = ambienceVolume;
            }
        }
    }
}
