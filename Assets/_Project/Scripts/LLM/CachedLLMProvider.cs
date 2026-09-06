using HwigiTower.NPC;

namespace HwigiTower.LLM
{
    public sealed class CachedLLMProvider : ILLMProvider
    {
        private readonly INPCMemoryRepo _repo;
        private readonly ILLMProvider _inner;

        public CachedLLMProvider(INPCMemoryRepo repo, ILLMProvider inner)
        {
            _repo = repo;
            _inner = inner;
        }

        public bool TryComplete(LLMRequest request, out LLMResponse response)
        {
            response = default;

            if (_repo != null && _repo.TryGetCachedResponse(request.CacheKey, out response))
            {
                return true;
            }

            if (_inner == null || !_inner.TryComplete(request, out response))
            {
                return false;
            }

            _repo?.SaveCachedResponse(response);
            return true;
        }
    }
}
