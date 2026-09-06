using HwigiTower.LLM;

namespace HwigiTower.NPC
{
    public sealed class SqliteNpcMemoryRepo : INPCMemoryRepo
    {
        public const string RunsSchema =
            "CREATE TABLE IF NOT EXISTS runs (run_id TEXT PRIMARY KEY, started_at TEXT, ended_at TEXT, cause_of_end TEXT, floor_reached INTEGER, npc_state TEXT);";

        public const string ReflectionsSchema =
            "CREATE TABLE IF NOT EXISTS reflections (run_id TEXT PRIMARY KEY, prompt_hash TEXT, summary TEXT);";

        public const string CacheSchema =
            "CREATE TABLE IF NOT EXISTS llm_cache (run_id TEXT, prompt_hash TEXT, response TEXT, PRIMARY KEY (run_id, prompt_hash));";

        private readonly InMemoryNpcMemoryRepo _fallback = new InMemoryNpcMemoryRepo();

        public SqliteNpcMemoryRepo(string databasePath)
        {
            DatabasePath = databasePath ?? string.Empty;
        }

        public string DatabasePath { get; }
        public string[] SchemaStatements => new[] { RunsSchema, ReflectionsSchema, CacheSchema };

        public void SaveReflection(RunReflection reflection)
        {
            _fallback.SaveReflection(reflection);
        }

        public bool TryGetReflection(string runId, out RunReflection reflection)
        {
            return _fallback.TryGetReflection(runId, out reflection);
        }

        public RunReflection[] GetRecentReflections(int count)
        {
            return _fallback.GetRecentReflections(count);
        }

        public void SaveCachedResponse(LLMResponse response)
        {
            _fallback.SaveCachedResponse(response);
        }

        public bool TryGetCachedResponse(DeterministicCacheKey cacheKey, out LLMResponse response)
        {
            return _fallback.TryGetCachedResponse(cacheKey, out response);
        }

        public void Clear()
        {
            _fallback.Clear();
        }
    }
}
