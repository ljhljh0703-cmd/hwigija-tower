using UnityEngine;

namespace HwigiTower.LLM
{
    [CreateAssetMenu(menuName = "Hwigi Tower/LLM/Runtime Settings", fileName = "SO_LLM_RuntimeSettings")]
    public sealed class LLMRuntimeSettings : ScriptableObject
    {
        [SerializeField] private LLMRuntimeConfig config = LLMRuntimeConfig.FakeDefault();

        public LLMRuntimeConfig Config => config ?? LLMRuntimeConfig.FakeDefault();
    }
}
