using System;
using HwigiTower.Lobby;
using UnityEngine;

namespace HwigiTower.UI
{
    public static class FloorMapLayoutRuntime
    {
        public static Vector2 AnchorMin(FloorMapSlot slot)
        {
            return new Vector2(slot.X, 1f - slot.Y - slot.Height);
        }

        public static Vector2 AnchorMax(FloorMapSlot slot)
        {
            return new Vector2(slot.X + slot.Width, 1f - slot.Y);
        }
    }

    public static class FloorMapUiTokens
    {
        public static readonly Color VoidBg = Parse(UiTokenContract.VoidBgHex);
        public static readonly Color PanelBg = Parse(UiTokenContract.PanelBgHex);
        public static readonly Color PanelBgAlt = Parse(UiTokenContract.PanelBgAltHex);
        public static readonly Color Frame = Parse(UiTokenContract.FrameHex);
        public static readonly Color FrameHi = Parse(UiTokenContract.FrameHiHex);
        public static readonly Color Gold = Parse(UiTokenContract.GoldHex);
        public static readonly Color Ink = Parse(UiTokenContract.InkHex);
        public static readonly Color InkDim = Parse(UiTokenContract.InkDimHex);
        public static readonly Color InkMute = Parse(UiTokenContract.InkMuteHex);
        public static readonly Color Hp = Parse(UiTokenContract.HpHex);
        public static readonly Color Sanity = Parse(UiTokenContract.SanityHex);
        public static readonly Color GoldCoin = Parse(UiTokenContract.GoldCoinHex);

        public static Font ResolveRuntimeFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static Color Parse(string html)
        {
            if (!ColorUtility.TryParseHtmlString(html, out var color))
            {
                throw new InvalidOperationException("Invalid floor map UI token: " + html);
            }

            return color;
        }
    }
}
