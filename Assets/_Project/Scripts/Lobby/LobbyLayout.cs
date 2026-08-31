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
        public const string VoidBgHex = LobbyUiTokenContract.VoidBgHex;
        public const string PanelBgHex = LobbyUiTokenContract.PanelBgHex;
        public const string PanelBgAltHex = LobbyUiTokenContract.PanelBgAltHex;
        public const string FrameHex = LobbyUiTokenContract.FrameHex;
        public const string FrameHiHex = LobbyUiTokenContract.FrameHiHex;
        public const string GoldHex = LobbyUiTokenContract.GoldHex;
        public const string GoldDimHex = LobbyUiTokenContract.GoldDimHex;
        public const string InkHex = LobbyUiTokenContract.InkHex;
        public const string InkDimHex = LobbyUiTokenContract.InkDimHex;
        public const string InkMuteHex = LobbyUiTokenContract.InkMuteHex;

        public const int TitleFontSize = LobbyUiTokenContract.TitleFontSize;
        public const int PrimaryFontSize = LobbyUiTokenContract.PrimaryFontSize;
        public const int BodyFontSize = LobbyUiTokenContract.BodyFontSize;
        public const int LabelFontSize = LobbyUiTokenContract.LabelFontSize;
        public const int MicroFontSize = LobbyUiTokenContract.MicroFontSize;
        public const float TitleLineHeight = LobbyUiTokenContract.TitleLineHeight;
        public const float PrimaryLineHeight = LobbyUiTokenContract.PrimaryLineHeight;
        public const float BodyLineHeight = LobbyUiTokenContract.BodyLineHeight;
        public const float LabelLineHeight = LobbyUiTokenContract.LabelLineHeight;
        public const float DisplayLetterSpacing = LobbyUiTokenContract.DisplayLetterSpacing;
        public const float LabelLetterSpacing = LobbyUiTokenContract.LabelLetterSpacing;
        public const int SpacingUnit = LobbyUiTokenContract.SpacingUnit;
        public const int Gutter = LobbyUiTokenContract.Gutter;
        public const int FrameBorderWidth = LobbyUiTokenContract.FrameBorderWidth;
        public const int CornerSize = LobbyUiTokenContract.CornerSize;
        public const int TapTargetMinPx = LobbyUiTokenContract.TapTargetMinPx;
        public const float VignetteStrength = LobbyUiTokenContract.VignetteStrength;

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
