using System;
using HwigiTower.Lobby;
using UnityEngine;

namespace HwigiTower.UI
{
    public static class UiColorTokens
    {
        public static readonly Color NoTint = Parse(UiTokenContract.NoTintHex);

        public static Color WithAlpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }

        private static Color Parse(string html)
        {
            if (!ColorUtility.TryParseHtmlString(html, out var color))
            {
                throw new InvalidOperationException("Invalid UI color token: " + html);
            }

            return color;
        }
    }
}
