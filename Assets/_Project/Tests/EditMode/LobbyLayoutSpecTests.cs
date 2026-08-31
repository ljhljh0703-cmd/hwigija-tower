using System;
using System.IO;
using HwigiTower.Lobby;
using NUnit.Framework;

namespace HwigiTower.Tests.EditMode
{
    public sealed class LobbyLayoutSpecTests
    {
        private const string LayoutSpecPath = "Docs/Portfolio/assets/concept-to-ui/lobby_layout_spec.json";
        private const string TokenSpecPath = "Docs/Portfolio/assets/concept-to-ui/ui_tokens.json";

        [Test]
        public void LobbyLayoutSpec_RoundThreeRevisionAndAllEightConstraintsArePresent()
        {
            var spec = LoadLayoutSpec();

            Assert.AreEqual("lobby", spec.screen);
            Assert.AreEqual(8, spec.constraints.Length);
            StringAssert.Contains("v1.1", string.Join("\n", spec.revisions));
            StringAssert.Contains("v1.2", string.Join("\n", spec.revisions));
            StringAssert.Contains("layer=title", Constraint(spec, "L-03").exempt);
            StringAssert.Contains("불투명 패널", Constraint(spec, "L-03").text);
            StringAssert.Contains("불투명 패널", Constraint(spec, "L-08").text);
            Assert.AreEqual("저장 유무 · 최고 도달 층 · 기억 조각", spec.slots.runStatus.content);
            Assert.AreEqual("저장 유무 · 최고 도달 층 · 기억 조각", LobbyLayout.RunStatus.Content);
        }

        [Test]
        public void LobbyLayoutSpec_AllConstraintsPassAgainstTheV11Contract()
        {
            var spec = LoadLayoutSpec();
            var tokens = LoadTokenSpec();
            var slots = spec.slots.All();

            Assert.IsTrue(AllInteractiveSlotsAreInActionZone(slots, tokens.interaction.actionZone.yFrom));
            Assert.AreEqual(1, CountPrimarySlots(slots));
            Assert.IsTrue(NoProtectedAxisViolations(slots));
            Assert.IsTrue(NoPrototypeText(slots));
            Assert.IsTrue(PrimaryAndSecondaryHaveFrames(slots));
            Assert.IsTrue(AllInteractiveSlotsMeetTapTarget(slots, tokens.referenceResolution.height, tokens.interaction.tapTargetMinPx));
            CollectionAssert.AreEqual(LobbyLayout.BandOrder, tokens.bands.order);
            Assert.IsTrue(TitleSlotsHaveNoFrames(spec.slots));
        }

        [Test]
        public void LobbyLayout_ContractSlotsMatchTheCheckedInSpecification()
        {
            var slots = LoadLayoutSpec().slots;

            AssertSlot(slots.runStatus, LobbyLayout.RunStatus);
            AssertSlot(slots.towerArt, LobbyLayout.TowerArt);
            AssertSlot(slots.titleMark, LobbyLayout.TitleMark);
            AssertSlot(slots.tagline, LobbyLayout.Tagline);
            AssertSlot(slots.primaryAction, LobbyLayout.PrimaryAction);
            AssertSlot(slots.secondaryAction, LobbyLayout.SecondaryAction);
            AssertSlot(slots.utilityRow, LobbyLayout.UtilityRow);
            AssertSlot(slots.buildStamp, LobbyLayout.BuildStamp);
        }

        [Test]
        public void LobbyUiTokens_MatchTheCheckedInTokenSource()
        {
            var tokens = LoadTokenSpec();

            Assert.AreEqual(tokens.color.voidBg, LobbyUiTokens.VoidBgHex);
            Assert.AreEqual(tokens.color.panelBg, LobbyUiTokens.PanelBgHex);
            Assert.AreEqual(tokens.color.panelBgAlt, LobbyUiTokens.PanelBgAltHex);
            Assert.AreEqual(tokens.color.frame, LobbyUiTokens.FrameHex);
            Assert.AreEqual(tokens.color.frameHi, LobbyUiTokens.FrameHiHex);
            Assert.AreEqual(tokens.color.gold, LobbyUiTokens.GoldHex);
            Assert.AreEqual(tokens.color.goldDim, LobbyUiTokens.GoldDimHex);
            Assert.AreEqual(tokens.color.ink, LobbyUiTokens.InkHex);
            Assert.AreEqual(tokens.color.inkDim, LobbyUiTokens.InkDimHex);
            Assert.AreEqual(tokens.color.inkMute, LobbyUiTokens.InkMuteHex);
            Assert.AreEqual(tokens.typeScale.title.size, LobbyUiTokens.TitleFontSize);
            Assert.AreEqual(tokens.typeScale.primary.size, LobbyUiTokens.PrimaryFontSize);
            Assert.AreEqual(tokens.typeScale.body.size, LobbyUiTokens.BodyFontSize);
            Assert.AreEqual(tokens.typeScale.label.size, LobbyUiTokens.LabelFontSize);
            Assert.AreEqual(tokens.typeScale.micro.size, LobbyUiTokens.MicroFontSize);
            Assert.AreEqual(tokens.typeScale.title.lineHeight, LobbyUiTokens.TitleLineHeight, 0.0001f);
            Assert.AreEqual(tokens.typeScale.primary.lineHeight, LobbyUiTokens.PrimaryLineHeight, 0.0001f);
            Assert.AreEqual(tokens.typeScale.body.lineHeight, LobbyUiTokens.BodyLineHeight, 0.0001f);
            Assert.AreEqual(tokens.typeScale.label.lineHeight, LobbyUiTokens.LabelLineHeight, 0.0001f);
            Assert.AreEqual(tokens.spacing.unit, LobbyUiTokens.SpacingUnit);
            Assert.AreEqual(tokens.spacing.gutter, LobbyUiTokens.Gutter);
            Assert.AreEqual(tokens.frameStyle.border.widthPx, LobbyUiTokens.FrameBorderWidth);
            Assert.AreEqual(tokens.frameStyle.corner.sizePx, LobbyUiTokens.CornerSize);
            Assert.AreEqual(tokens.interaction.tapTargetMinPx, LobbyUiTokens.TapTargetMinPx);
        }

        [Test]
        public void LobbyController_HasNoLiteralColorConstructionAndDoesNotDependOnPrototypeHud()
        {
            var source = File.ReadAllText("Assets/_Project/Scripts/Lobby/LobbyController.cs");

            StringAssert.DoesNotContain("new Color(", source);
            StringAssert.DoesNotContain("ColorUtility.TryParseHtmlString", source);
            StringAssert.DoesNotContain("PrototypeHud", source);
        }

        [Test]
        public void LobbyLayoutContract_IsPureAndTheUnityAdapterOwnsVectorConversion()
        {
            var contract = File.ReadAllText("Assets/_Project/Scripts/Lobby/LobbyLayoutContract.cs");
            var adapter = File.ReadAllText("Assets/_Project/Scripts/Lobby/LobbyLayout.cs");

            StringAssert.DoesNotContain("UnityEngine", contract);
            StringAssert.Contains("Vector2", adapter);
            StringAssert.Contains("Color", adapter);
        }

        private static LayoutSpec LoadLayoutSpec()
        {
            Assert.IsTrue(File.Exists(LayoutSpecPath), "Missing lobby layout specification: " + LayoutSpecPath);
            return UnityEngine.JsonUtility.FromJson<LayoutSpec>(File.ReadAllText(LayoutSpecPath));
        }

        private static TokenSpec LoadTokenSpec()
        {
            Assert.IsTrue(File.Exists(TokenSpecPath), "Missing UI token source: " + TokenSpecPath);
            return UnityEngine.JsonUtility.FromJson<TokenSpec>(File.ReadAllText(TokenSpecPath));
        }

        private static LayoutConstraint Constraint(LayoutSpec spec, string id)
        {
            for (var i = 0; i < spec.constraints.Length; i++)
            {
                if (spec.constraints[i].id == id)
                {
                    return spec.constraints[i];
                }
            }

            Assert.Fail("Missing layout constraint: " + id);
            return null;
        }

        private static bool AllInteractiveSlotsAreInActionZone(LayoutSlot[] slots, float actionY)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i].interactive && slots[i].y < actionY)
                {
                    return false;
                }
            }

            return true;
        }

        private static int CountPrimarySlots(LayoutSlot[] slots)
        {
            var count = 0;
            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i].role == "primary")
                {
                    count++;
                }
            }

            return count;
        }

        private static bool NoProtectedAxisViolations(LayoutSlot[] slots)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                var isProtectedType = slot.layer == "ui" || slot.layer == "actions" || slot.frameStyle;
                if (isProtectedType && OverlapsTowerAxis(slot))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool OverlapsTowerAxis(LayoutSlot slot)
        {
            return slot.x < LobbyLayout.TowerProtectedXMax &&
                slot.x + slot.w > LobbyLayout.TowerProtectedXMin &&
                slot.y < LobbyLayout.TowerProtectedYMax &&
                slot.y + slot.h > LobbyLayout.TowerProtectedYMin;
        }

        private static bool NoPrototypeText(LayoutSlot[] slots)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i].type == "text" && !string.IsNullOrEmpty(slots[i].content) && slots[i].content.Contains("Prototype"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PrimaryAndSecondaryHaveFrames(LayoutSlot[] slots)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if ((slots[i].role == "primary" || slots[i].role == "secondary") && !slots[i].frameStyle)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AllInteractiveSlotsMeetTapTarget(LayoutSlot[] slots, int referenceHeight, int minimumPx)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i].interactive && slots[i].h * referenceHeight < minimumPx)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TitleSlotsHaveNoFrames(LayoutSlots slots)
        {
            return slots.titleMark.layer == "title" &&
                slots.tagline.layer == "title" &&
                !slots.titleMark.frameStyle &&
                !slots.tagline.frameStyle;
        }

        private static void AssertSlot(LayoutSlot actual, LobbySlot expected)
        {
            Assert.IsNotNull(actual, "Missing spec slot: " + expected.Key);
            Assert.AreEqual(expected.X, actual.x, 0.0001f, expected.Key + " x");
            Assert.AreEqual(expected.Y, actual.y, 0.0001f, expected.Key + " y");
            Assert.AreEqual(expected.Width, actual.w, 0.0001f, expected.Key + " width");
            Assert.AreEqual(expected.Height, actual.h, 0.0001f, expected.Key + " height");
            Assert.AreEqual(expected.Layer ?? string.Empty, actual.layer ?? string.Empty, expected.Key + " layer");
            Assert.AreEqual(expected.Type ?? string.Empty, actual.type ?? string.Empty, expected.Key + " type");
            Assert.AreEqual(expected.Role ?? string.Empty, actual.role ?? string.Empty, expected.Key + " role");
            Assert.AreEqual(expected.Interactive, actual.interactive, expected.Key + " interactive");
            Assert.AreEqual(expected.HasFrame, actual.frameStyle, expected.Key + " frameStyle");
            Assert.AreEqual(expected.Content ?? string.Empty, actual.content ?? string.Empty, expected.Key + " content");
        }

        [Serializable]
        private sealed class LayoutSpec
        {
            public string screen;
            public LayoutConstraint[] constraints;
            public LayoutSlots slots;
            public string[] revisions;
        }

        [Serializable]
        private sealed class LayoutConstraint
        {
            public string id;
            public string text;
            public string exempt;
        }

        [Serializable]
        private sealed class LayoutSlots
        {
            public LayoutSlot runStatus;
            public LayoutSlot towerArt;
            public LayoutSlot titleMark;
            public LayoutSlot tagline;
            public LayoutSlot primaryAction;
            public LayoutSlot secondaryAction;
            public LayoutSlot utilityRow;
            public LayoutSlot buildStamp;

            public LayoutSlot[] All()
            {
                return new[]
                {
                    runStatus,
                    towerArt,
                    titleMark,
                    tagline,
                    primaryAction,
                    secondaryAction,
                    utilityRow,
                    buildStamp
                };
            }
        }

        [Serializable]
        private sealed class LayoutSlot
        {
            public string key;
            public float x;
            public float y;
            public float w;
            public float h;
            public string layer;
            public string type;
            public string role;
            public bool interactive;
            public bool frameStyle;
            public string content;
        }

        [Serializable]
        private sealed class TokenSpec
        {
            public ReferenceResolution referenceResolution;
            public TokenColors color;
            public TokenTypeScale typeScale;
            public TokenSpacing spacing;
            public TokenFrameStyle frameStyle;
            public TokenBands bands;
            public TokenInteraction interaction;
        }

        [Serializable]
        private sealed class ReferenceResolution
        {
            public int width;
            public int height;
        }

        [Serializable]
        private sealed class TokenColors
        {
            public string voidBg;
            public string panelBg;
            public string panelBgAlt;
            public string frame;
            public string frameHi;
            public string gold;
            public string goldDim;
            public string ink;
            public string inkDim;
            public string inkMute;
        }

        [Serializable]
        private sealed class TokenTypeScale
        {
            public TokenTextSize title;
            public TokenTextSize primary;
            public TokenTextSize body;
            public TokenTextSize label;
            public TokenTextSize micro;
        }

        [Serializable]
        private sealed class TokenTextSize
        {
            public int size;
            public float lineHeight;
        }

        [Serializable]
        private sealed class TokenSpacing
        {
            public int unit;
            public int gutter;
        }

        [Serializable]
        private sealed class TokenFrameStyle
        {
            public TokenBorder border;
            public TokenCorner corner;
        }

        [Serializable]
        private sealed class TokenBorder
        {
            public int widthPx;
        }

        [Serializable]
        private sealed class TokenCorner
        {
            public int sizePx;
        }

        [Serializable]
        private sealed class TokenBands
        {
            public string[] order;
        }

        [Serializable]
        private sealed class TokenInteraction
        {
            public int tapTargetMinPx;
            public TokenActionZone actionZone;
        }

        [Serializable]
        private sealed class TokenActionZone
        {
            public float yFrom;
        }
    }
}
