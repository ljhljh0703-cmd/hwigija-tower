namespace HwigiTower.LLM
{
    public interface ILLMProvider
    {
        bool TryComplete(LLMRequest request, out LLMResponse response);
    }
}
