using UnityEngine;

namespace HwigiTower.Audio
{
    public enum PrototypeAudioContext
    {
        None = 0,
        Lobby = 1,
        Exploration = 2,
        Combat = 3,
        Boss = 4,
        Rest = 5,
        Shop = 6,
        Event = 7,
        Ending = 8,
        Floor01Ambience = 101,
        Floor02Ambience = 102,
        Floor03Ambience = 103,
        Floor04Ambience = 104,
        Floor05Ambience = 105,
        UiSfx = 201,
        CombatSfx = 202,
        EventSfx = 203,
        ShopSfx = 204,
        RestSfx = 205,
        MemorySfx = 206,
        CutsceneSfx = 207
    }

    public enum PrototypeAudioChannel
    {
        Bgm = 0,
        Ambience = 1,
        Sfx = 2
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Audio/Audio Cue", fileName = "SO_AudioCue")]
    public sealed class AudioCueData : ScriptableObject
    {
        [SerializeField] private string cueId = string.Empty;
        [SerializeField] private PrototypeAudioContext context;
        [SerializeField] private PrototypeAudioChannel channel;
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool loop = true;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        public string CueId => cueId;
        public PrototypeAudioContext Context => context;
        public PrototypeAudioChannel Channel => channel;
        public AudioClip Clip => clip;
        public bool Loop => loop;
        public float Volume => Mathf.Clamp01(volume);
        public bool HasClip => clip != null;
    }
}
