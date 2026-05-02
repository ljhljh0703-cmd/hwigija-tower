namespace HwigiTower.LLM
{
    public sealed class DeterministicFakeLLMProvider : ILLMProvider
    {
        public bool TryComplete(LLMRequest request, out LLMResponse response)
        {
            if (!request.CacheKey.IsValid)
            {
                response = default;
                return false;
            }

            var prefix = string.IsNullOrEmpty(request.PromptProfileId) ? "fake" : request.PromptProfileId;
            response = new LLMResponse(request.CacheKey, $"{prefix}:{request.CacheKey.PromptHash}");
            return true;
        }
    }
}
