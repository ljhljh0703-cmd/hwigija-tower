using System;
using System.IO;
using UnityEngine;

namespace HwigiTower.LLM
{
    [Serializable]
    public sealed class LLMRuntimeConfig
    {
        [SerializeField] private LLMProviderMode providerMode = LLMProviderMode.Fake;
        [SerializeField] private string modelId = string.Empty;
        [SerializeField] private string streamingAssetsRelativePath = string.Empty;
        [SerializeField] private string tokenizerRelativePath = string.Empty;
        [SerializeField, Min(1)] private int maxInputTokens = 2048;
        [SerializeField, Min(1)] private int maxOutputTokens = 100;
        [SerializeField] private bool deterministicCacheEnabled = true;
        [SerializeField] private bool fallbackEnabled = true;

        public LLMProviderMode ProviderMode => providerMode;
        public string ModelId => modelId ?? string.Empty;
        public string StreamingAssetsRelativePath => streamingAssetsRelativePath ?? string.Empty;
        public string TokenizerRelativePath => tokenizerRelativePath ?? string.Empty;
        public int MaxInputTokens => maxInputTokens <= 0 ? 2048 : maxInputTokens;
        public int MaxOutputTokens => maxOutputTokens <= 0 ? 100 : maxOutputTokens;
        public bool DeterministicCacheEnabled => deterministicCacheEnabled;
        public bool FallbackEnabled => fallbackEnabled;
        public bool HasModelPath => !string.IsNullOrWhiteSpace(StreamingAssetsRelativePath);
        public bool HasTokenizerPath => !string.IsNullOrWhiteSpace(TokenizerRelativePath);

        public static LLMRuntimeConfig FakeDefault()
        {
            return new LLMRuntimeConfig();
        }

        public string ResolveModelPath()
        {
            return ResolveStreamingAssetsPath(StreamingAssetsRelativePath);
        }

        public string ResolveTokenizerPath()
        {
            return ResolveStreamingAssetsPath(TokenizerRelativePath);
        }

        private static string ResolveStreamingAssetsPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }

            return Path.Combine(Application.streamingAssetsPath, relativePath.TrimStart('/', '\\'));
        }
    }
}
