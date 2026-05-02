using System;

namespace HwigiTower.LLM
{
    public static class TokenEstimate
    {
        public static int FromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            return Math.Max(1, (text.Length + 3) / 4);
        }
    }
}
