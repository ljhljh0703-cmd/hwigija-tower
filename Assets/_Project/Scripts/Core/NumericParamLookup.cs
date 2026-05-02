using System.Collections.Generic;

namespace HwigiTower.Core
{
    public static class NumericParamLookup
    {
        public static float Sum(IReadOnlyList<NumericParam> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return 0f;
            }

            var total = 0f;
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if (parameter != null && parameter.Key == key)
                {
                    total += parameter.Value;
                }
            }

            return total;
        }
    }
}
