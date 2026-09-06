using System.IO;
using UnityEngine;

namespace HwigiTower.Run
{
    public static class PrototypeRunSaveStore
    {
        private const string FileName = "prototype-run-save.json";

        public static string DefaultPath => Path.Combine(Application.persistentDataPath, FileName);

        public static bool HasSave(string path = null)
        {
            return File.Exists(ResolvePath(path));
        }

        public static bool TryLoad(out PrototypeRunSaveData data, string path = null)
        {
            data = null;
            var resolvedPath = ResolvePath(path);
            if (!File.Exists(resolvedPath))
            {
                return false;
            }

            try
            {
                data = JsonUtility.FromJson<PrototypeRunSaveData>(File.ReadAllText(resolvedPath));
                return data != null && !string.IsNullOrEmpty(data.runId);
            }
            catch
            {
                data = null;
                return false;
            }
        }

        public static bool TryLoadSummary(out PrototypeRunSaveSummary summary, string path = null)
        {
            summary = default;
            if (!TryLoad(out var data, path))
            {
                return false;
            }

            var status = data.endingRest ? "ending.rest" :
                data.endingContinue ? "ending.continue" :
                data.runFailed ? "run.failed" :
                data.runClear ? "run.clear" :
                data.restartReady ? "run.restartReady" :
                "run.active";
            summary = new PrototypeRunSaveSummary(
                data.runId,
                data.currentFloor,
                data.playerHp,
                data.playerMaxHp,
                data.memoryFragmentRefs == null ? 0 : data.memoryFragmentRefs.Length,
                status);
            return true;
        }

        public static void Save(PrototypeRunSaveData data, string path = null)
        {
            if (data == null || string.IsNullOrEmpty(data.runId))
            {
                return;
            }

            var resolvedPath = ResolvePath(path);
            var directory = Path.GetDirectoryName(resolvedPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(resolvedPath, JsonUtility.ToJson(data, true));
        }

        public static void Delete(string path = null)
        {
            var resolvedPath = ResolvePath(path);
            if (File.Exists(resolvedPath))
            {
                File.Delete(resolvedPath);
            }
        }

        private static string ResolvePath(string path)
        {
            return string.IsNullOrEmpty(path) ? DefaultPath : path;
        }
    }

    public static class PrototypeRunSaveRequest
    {
        private static bool _continueRequested;

        public static void RequestContinue()
        {
            _continueRequested = true;
        }

        public static void RequestNewGame()
        {
            _continueRequested = false;
        }

        public static bool ConsumeContinueRequested()
        {
            var requested = _continueRequested;
            _continueRequested = false;
            return requested;
        }
    }
}
