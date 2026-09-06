using System;
using HwigiTower.Lobby;
using UnityEngine;

namespace HwigiTower.UI
{
    public static class EndingLayoutRuntime
    {
        public static Vector2 AnchorMin(EndingSlot slot) => new Vector2(slot.X, 1f - slot.Y - slot.Height);
        public static Vector2 AnchorMax(EndingSlot slot) => new Vector2(slot.X + slot.Width, 1f - slot.Y);
    }

    public static class EndingUiTokens
    {
        public static readonly Color VoidBg = Parse(UiTokenContract.VoidBgHex);
        public static readonly Color PanelBg = Parse(UiTokenContract.PanelBgHex);
        public static readonly Color PanelBgAlt = Parse(UiTokenContract.PanelBgAltHex);
        public static readonly Color Frame = Parse(UiTokenContract.FrameHex);
        public static readonly Color Ink = Parse(UiTokenContract.InkHex);
        public static readonly Color InkDim = Parse(UiTokenContract.InkDimHex);

        public static Font ResolveRuntimeFont() => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        private static Color Parse(string html)
        {
            if (!ColorUtility.TryParseHtmlString(html, out var color))
            {
                throw new InvalidOperationException("Invalid ending UI token: " + html);
            }

            return color;
        }
    }
}
