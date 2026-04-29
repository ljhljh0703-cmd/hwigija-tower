using UnityEngine;

namespace HwigiTower.Core
{
    public sealed class PrototypeRuntimeBootstrap : MonoBehaviour
    {
        [SerializeField] private PrototypeRuntimeSettings runtimeSettings;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            if (runtimeSettings != null && !Application.isMobilePlatform)
            {
                Screen.SetResolution(runtimeSettings.TargetWidth, runtimeSettings.TargetHeight, runtimeSettings.Fullscreen);
            }
        }
    }
}
