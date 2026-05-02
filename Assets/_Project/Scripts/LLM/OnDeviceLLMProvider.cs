namespace HwigiTower.LLM
{
    public sealed class OnDeviceLLMProvider : ILLMProvider
    {
        private readonly int _maxOutputTokens;
        private readonly ILLMProvider _fallback;

        public OnDeviceLLMProvider(int maxOutputTokens, ILLMProvider fallback)
        {
            _maxOutputTokens = maxOutputTokens <= 0 ? 100 : maxOutputTokens;
            _fallback = fallback;
        }

        public bool TryComplete(LLMRequest request, out LLMResponse response)
        {
            response = default;

            if (MLCBridge.TryComplete(request.Prompt, _maxOutputTokens, out var text))
            {
                response = new LLMResponse(request.CacheKey, text);
                return true;
            }

            return _fallback != null && _fallback.TryComplete(request, out response);
        }
    }
}
