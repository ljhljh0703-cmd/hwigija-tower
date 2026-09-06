namespace HwigiTower.LLM
{
    public sealed class NullLLMProvider : ILLMProvider
    {
        public bool TryComplete(LLMRequest request, out LLMResponse response)
        {
            response = default;
            return false;
        }
    }
}
