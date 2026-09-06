using UnityEngine;

namespace HwigiTower.Core
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Runtime Settings", fileName = "SO_Prototype_RuntimeSettings")]
    public sealed class PrototypeRuntimeSettings : ScriptableObject
    {
        [SerializeField, Min(1)] private int targetWidth = 1080;
        [SerializeField, Min(1)] private int targetHeight = 1920;
        [SerializeField] private bool fullscreen;

        public int TargetWidth => targetWidth;
        public int TargetHeight => targetHeight;
        public bool Fullscreen => fullscreen;
    }
}
