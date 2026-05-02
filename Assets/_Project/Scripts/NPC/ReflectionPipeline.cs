using System.Text;
using HwigiTower.Core;
using HwigiTower.LLM;

namespace HwigiTower.NPC
{
    public sealed class ReflectionPipeline
    {
        private readonly INPCMemoryRepo _repo;
        private readonly ILLMProvider _provider;
        private readonly GameFlowEventBus _eventBus;

        public ReflectionPipeline(INPCMemoryRepo repo, ILLMProvider provider, GameFlowEventBus eventBus)
        {
            _repo = repo;
            _provider = provider;
            _eventBus = eventBus;
        }

        public bool TrySaveReflection(string runId, string runSummary, out RunReflection reflection)
        {
            var prompt = $"reflection\nrun:{runId}\nsummary:{runSummary}";
            var request = new LLMRequest(runId, prompt, "reflection.v1");
            if (_provider == null || !_provider.TryComplete(request, out var response))
            {
                reflection = default;
                return false;
            }

            reflection = new RunReflection(runId, request.CacheKey.PromptHash, response.Text);
            _repo?.SaveReflection(reflection);
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.ReflectionSaved, runId, runId, request.CacheKey.PromptHash));
            return true;
        }

        public string LoadRecallPrompt(string runId, int recentCount)
        {
            var reflections = _repo == null ? new RunReflection[0] : _repo.GetRecentReflections(recentCount);
            var builder = new StringBuilder();
            builder.Append("recall");
            for (var i = 0; i < reflections.Length; i++)
            {
                builder.Append('\n');
                builder.Append(reflections[i].RunId);
                builder.Append(':');
                builder.Append(reflections[i].Summary);
            }

            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.RecallLoaded, runId, runId, reflections.Length.ToString()));
            return builder.ToString();
        }
    }
}
