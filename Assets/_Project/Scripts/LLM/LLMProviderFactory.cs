using System.IO;
using HwigiTower.NPC;

namespace HwigiTower.LLM
{
    public static class LLMProviderFactory
    {
        public static ILLMProvider Create(LLMRuntimeSettings settings, INPCMemoryRepo repo)
        {
            return Create(settings == null ? null : settings.Config, repo);
        }

        public static ILLMProvider Create(LLMRuntimeConfig config, INPCMemoryRepo repo)
        {
            config ??= LLMRuntimeConfig.FakeDefault();

            var provider = CreateUncached(config);
            if (config.DeterministicCacheEnabled && repo != null &&
                (config.ProviderMode == LLMProviderMode.CachedOnDevice || config.ProviderMode == LLMProviderMode.Fake))
            {
                return new CachedLLMProvider(repo, provider);
            }

            return provider;
        }

        private static ILLMProvider CreateUncached(LLMRuntimeConfig config)
        {
            if (config.ProviderMode == LLMProviderMode.Fake)
            {
                return new DeterministicFakeLLMProvider();
            }

            var modelPath = config.ResolveModelPath();
            var tokenizerPath = config.ResolveTokenizerPath();
            if (!IsReadableModelPath(modelPath) || !IsReadableModelPath(tokenizerPath))
            {
                return config.FallbackEnabled ? new DeterministicFakeLLMProvider() : new NullLLMProvider();
            }

            var fallback = config.FallbackEnabled ? new DeterministicFakeLLMProvider() : null;
            return new OnDeviceLLMProvider(config.MaxOutputTokens, modelPath, tokenizerPath, fallback);
        }

        private static bool IsReadableModelPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && (Directory.Exists(path) || File.Exists(path));
        }
    }
}
