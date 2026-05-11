using UnityEngine;

namespace HwigiTower.Audio
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Audio/Audio Cue Catalog", fileName = "SO_AudioCueCatalog")]
    public sealed class AudioCueCatalog : ScriptableObject
    {
        [SerializeField] private AudioCueData[] cues = new AudioCueData[0];

        public AudioCueData[] Cues => cues ?? new AudioCueData[0];

        public bool TryGetCue(PrototypeAudioContext context, out AudioCueData cue)
        {
            var source = Cues;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].Context == context)
                {
                    cue = source[i];
                    return true;
                }
            }

            cue = null;
            return false;
        }
    }
}
