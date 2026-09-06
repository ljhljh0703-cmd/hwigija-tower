using System;
using UnityEngine;

namespace HwigiTower.Lobby
{
    public static class LobbyLayoutRuntime
    {
        public static Vector2 AnchorMin(LobbySlot slot)
        {
            return new Vector2(slot.X, 1f - slot.Y - slot.Height);
        }

        public static Vector2 AnchorMax(LobbySlot slot)
        {
            return new Vector2(slot.X + slot.Width, 1f - slot.Y);
        }
    }

    public static class LobbyUiTokens
    {
        public const string VoidBgHex = UiTokenContract.VoidBgHex;
        public const string PanelBgHex = UiTokenContract.PanelBgHex;
        public const string PanelBgAltHex = UiTokenContract.PanelBgAltHex;
        public const string FrameHex = UiTokenContract.FrameHex;
        public const string FrameHiHex = UiTokenContract.FrameHiHex;
        public const string GoldHex = UiTokenContract.GoldHex;
        public const string GoldDimHex = UiTokenContract.GoldDimHex;
        public const string InkHex = UiTokenContract.InkHex;
        public const string InkDimHex = UiTokenContract.InkDimHex;
        public const string InkMuteHex = UiTokenContract.InkMuteHex;

        public const int TitleFontSize = UiTokenContract.TitleFontSize;
        public const int PrimaryFontSize = UiTokenContract.PrimaryFontSize;
        public const int BodyFontSize = UiTokenContract.BodyFontSize;
        public const int LabelFontSize = UiTokenContract.LabelFontSize;
        public const int MicroFontSize = UiTokenContract.MicroFontSize;
        public const float TitleLineHeight = UiTokenContract.TitleLineHeight;
        public const float PrimaryLineHeight = UiTokenContract.PrimaryLineHeight;
        public const float BodyLineHeight = UiTokenContract.BodyLineHeight;
        public const float LabelLineHeight = UiTokenContract.LabelLineHeight;
        public const float DisplayLetterSpacing = UiTokenContract.DisplayLetterSpacing;
        public const float LabelLetterSpacing = UiTokenContract.LabelLetterSpacing;
        public const int SpacingUnit = UiTokenContract.SpacingUnit;
        public const int Gutter = UiTokenContract.Gutter;
        public const int FrameBorderWidth = UiTokenContract.FrameBorderWidth;
        public const int CornerSize = UiTokenContract.CornerSize;
        public const int TapTargetMinPx = UiTokenContract.TapTargetMinPx;
        public const float VignetteStrength = UiTokenContract.VignetteStrength;

        public static readonly Color VoidBg = Parse(VoidBgHex);
        public static readonly Color PanelBg = Parse(PanelBgHex);
        public static readonly Color PanelBgAlt = Parse(PanelBgAltHex);
        public static readonly Color Frame = Parse(FrameHex);
        public static readonly Color FrameHi = Parse(FrameHiHex);
        public static readonly Color Gold = Parse(GoldHex);
        public static readonly Color GoldDim = Parse(GoldDimHex);
        public static readonly Color Ink = Parse(InkHex);
        public static readonly Color InkDim = Parse(InkDimHex);
        public static readonly Color InkMute = Parse(InkMuteHex);

        public static Color WithAlpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }

        public static Font ResolveRuntimeFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static Color Parse(string html)
        {
            if (!ColorUtility.TryParseHtmlString(html, out var color))
            {
                throw new InvalidOperationException("Invalid lobby UI token: " + html);
            }

            return color;
        }
    }
}
