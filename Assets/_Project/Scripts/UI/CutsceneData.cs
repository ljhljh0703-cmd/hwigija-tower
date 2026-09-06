using UnityEngine;

namespace HwigiTower.UI
{
    public enum PrototypeCutsceneTrigger
    {
        None = 0,
        MemoryFragmentUnlock = 1,
        CombatGateStart = 2,
        DemoComplete = 3
    }

    [System.Serializable]
    public sealed class PrototypeCutsceneStep
    {
        [SerializeField] private Sprite image;
        [SerializeField] private string textKey = string.Empty;
        [SerializeField] private float durationSeconds = 1.5f;
        [SerializeField] private float fadeInSeconds = 0.15f;
        [SerializeField] private float fadeOutSeconds = 0.15f;
        [SerializeField] private AudioClip sfx;
        [SerializeField] private bool screenShake;
        [SerializeField] private bool glitchPulse;

        public Sprite Image => image;
        public string TextKey => textKey;
        public float DurationSeconds => Mathf.Max(0f, durationSeconds);
        public float FadeInSeconds => Mathf.Max(0f, fadeInSeconds);
        public float FadeOutSeconds => Mathf.Max(0f, fadeOutSeconds);
        public AudioClip Sfx => sfx;
        public bool ScreenShake => screenShake;
        public bool GlitchPulse => glitchPulse;
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Cutscene Data", fileName = "SO_Cutscene_Prototype")]
    public sealed class CutsceneData : ScriptableObject
    {
        [SerializeField] private string cutsceneId = string.Empty;
        [SerializeField] private string animationCutsceneId = string.Empty;
        [SerializeField] private string animationAssetPath = string.Empty;
        [SerializeField] private Sprite fallbackSprite;
        [SerializeField] private PrototypeCutsceneStep[] steps = new PrototypeCutsceneStep[0];

        public string CutsceneId => cutsceneId;
        public string AnimationCutsceneId => animationCutsceneId;
        public string AnimationAssetPath => animationAssetPath;
        public Sprite FallbackSprite => fallbackSprite;
        public PrototypeCutsceneStep[] Steps => steps ?? new PrototypeCutsceneStep[0];
        public bool HasSteps => Steps.Length > 0;
        public bool HasPlayableContent => HasSteps || fallbackSprite != null;
    }
}
