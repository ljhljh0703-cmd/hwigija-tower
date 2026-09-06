namespace HwigiTower.LLM
{
    public sealed class OnDeviceLLMProvider : ILLMProvider
    {
        private readonly int _maxOutputTokens;
        private readonly string _modelPath;
        private readonly string _tokenizerPath;
        private readonly ILLMProvider _fallback;
        private bool _initializeAttempted;
        private bool _initialized;

        public OnDeviceLLMProvider(int maxOutputTokens, ILLMProvider fallback)
            : this(maxOutputTokens, string.Empty, string.Empty, fallback)
        {
        }

        public OnDeviceLLMProvider(int maxOutputTokens, string modelPath, string tokenizerPath, ILLMProvider fallback)
        {
            _maxOutputTokens = maxOutputTokens <= 0 ? 100 : maxOutputTokens;
            _modelPath = modelPath ?? string.Empty;
            _tokenizerPath = tokenizerPath ?? string.Empty;
            _fallback = fallback;
        }

        public bool TryComplete(LLMRequest request, out LLMResponse response)
        {
            response = default;

            if (EnsureInitialized() && MLCBridge.TryComplete(request.Prompt, _maxOutputTokens, out var text))
            {
                response = new LLMResponse(request.CacheKey, text);
                return true;
            }

            return _fallback != null && _fallback.TryComplete(request, out response);
        }

        private bool EnsureInitialized()
        {
            if (_initialized)
            {
                return true;
            }

            if (_initializeAttempted)
            {
                return false;
            }

            _initializeAttempted = true;
            if (string.IsNullOrWhiteSpace(_modelPath) || string.IsNullOrWhiteSpace(_tokenizerPath))
            {
                _initialized = true;
                return true;
            }

            _initialized = MLCBridge.TryInitialize(_modelPath, _tokenizerPath);
            return _initialized;
        }
    }
}
