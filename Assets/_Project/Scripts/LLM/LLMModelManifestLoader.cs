using System;
using System.IO;
using UnityEngine;

namespace HwigiTower.LLM
{
    public static class LLMModelManifestLoader
    {
        private const string ModelConfigFileName = "model_config.json";

        public static bool TryLoadProjectModelConfig(string modelId, out LLMRuntimeConfig config)
        {
            config = LLMRuntimeConfig.FakeDefault();
            if (string.IsNullOrWhiteSpace(modelId))
            {
                return false;
            }

            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var path = Path.Combine(projectRoot, "Assets", "_Project", "Models", modelId, ModelConfigFileName);
            return TryLoadConfigFile(path, out config);
        }

        public static bool TryLoadConfigFile(string path, out LLMRuntimeConfig config)
        {
            config = LLMRuntimeConfig.FakeDefault();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            try
            {
                var json = File.ReadAllText(path);
                var parsed = JsonUtility.FromJson<LLMRuntimeConfig>(json);
                if (parsed == null)
                {
                    return false;
                }

                config = parsed;
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
