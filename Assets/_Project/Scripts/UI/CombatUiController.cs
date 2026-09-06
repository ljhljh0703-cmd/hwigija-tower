using System;
using HwigiTower.Combat;
using HwigiTower.Lobby;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class CombatUiController : MonoBehaviour
    {
        private RectTransform _root;
        private Text _floorText;
        private Text _sanityText;
        private Text _hpText;
        private Text _goldText;
        private Image _sanityFill;
        private Image _topHpFill;
        private Text _threatReadout;
        private Image _playerSanityFill;
        private Text _mataiosStateLabel;

        public RectTransform Root => _root;
        public bool Visible => _root != null && _root.gameObject.activeSelf;
        public Text ThreatReadoutText => _threatReadout;
        public RectTransform EnemyPanel { get; private set; }
        public Image EnemyImage { get; private set; }
        public Text EnemyTitleText { get; private set; }
        public Text EnemyStatusText { get; private set; }
        public Image EnemyHpFill { get; private set; }
        public Text EnemyDamageText { get; private set; }
        public Text MataiosDamageText { get; private set; }
        public Text DefeatFeedbackText { get; private set; }
        public RectTransform CombatLogPanel { get; private set; }
        public Text CombatLogText { get; private set; }
        public RectTransform PartyDock { get; private set; }
        public RectTransform PlayerCard { get; private set; }
        public RectTransform MataiosCard { get; private set; }
        public Image PlayerPortrait { get; private set; }
        public Image MataiosPortrait { get; private set; }
        public Image PlayerPortraitFrame { get; private set; }
        public Image MataiosPortraitFrame { get; private set; }
        public Text PlayerPortraitFallbackText { get; private set; }
        public Text PlayerCardText { get; private set; }
        public Text MataiosCardText { get; private set; }
        public Text PlayerDamageText { get; private set; }
        public Image PlayerHpFill { get; private set; }
        public Image MataiosHpFill { get; private set; }
        public Image TrainingStatusIcon { get; private set; }
        public Image BandageStatusIcon { get; private set; }
        public Image RecallStatusIcon { get; private set; }
        public Button AttackButton { get; private set; }
        public Button DefendButton { get; private set; }
        public Button SkillButton { get; private set; }
        public Image AttackIcon { get; private set; }
        public Image DefendIcon { get; private set; }
        public Image SkillIcon { get; private set; }
        public Button ItemInspectButton { get; private set; }

        public void Initialize(Transform parent, Action<CombatAction> onAction, Action onSkill, Action onItemInspect)
        {
            if (_root != null)
            {
                return;
            }

            transform.SetParent(parent, false);
            _root = transform as RectTransform;
            _root.anchorMin = Vector2.zero;
            _root.anchorMax = Vector2.one;
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;
            var backdrop = gameObject.AddComponent<Image>();
            backdrop.color = CombatUiTokens.VoidBg;
            backdrop.raycastTarget = false;

            BuildResourceStrip();
            BuildThreatReadout();
            BuildEnemySurface();
            BuildPartySurface();
            BuildActions(onAction, onSkill, onItemInspect);
            BuildCombatLog();
            Hide();
        }

        public void Show(PrototypeRunSnapshot snapshot)
        {
            _root.gameObject.SetActive(true);
            _floorText.text = "층 " + snapshot.CurrentFloor;
            _sanityText.text = "이성 " + snapshot.Mental;
            _hpText.text = "체력 " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp;
            _goldText.text = "골드 " + snapshot.Gold;
            _sanityFill.fillAmount = Mathf.Clamp01((snapshot.Mental + 100f) / 200f);
            _topHpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            _playerSanityFill.fillAmount = _sanityFill.fillAmount;
            _threatReadout.text = BuildThreatReadout(snapshot);
            _mataiosStateLabel.text = "마타이오스 동행 중";
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        public void SetActionSprite(CombatAction action, Sprite sprite)
        {
            var target = action == CombatAction.Attack ? AttackIcon :
                action == CombatAction.Defend ? DefendIcon :
                action == CombatAction.Skill ? SkillIcon : null;
            if (target == null)
            {
                return;
            }

            target.sprite = sprite;
            target.color = sprite == null ? CombatUiTokens.Gold : UiColorTokens.NoTint;
            target.gameObject.SetActive(true);
        }

        private void BuildResourceStrip()
        {
            _floorText = CreateChip(CombatLayout.FloorChip, CombatUiTokens.GoldCoin);
            (_sanityText, _sanityFill) = CreateGauge(CombatLayout.SanityChip, CombatUiTokens.Sanity);
            (_hpText, _topHpFill) = CreateGauge(CombatLayout.HpChip, CombatUiTokens.Hp);
            _goldText = CreateChip(CombatLayout.GoldChip, CombatUiTokens.GoldCoin);
        }

        private void BuildThreatReadout()
        {
            _threatReadout = CreateTextSlot("Combat Threat Readout", CombatLayout.ThreatReadout, CombatUiTokens.Ink);
            _threatReadout.fontStyle = FontStyle.Bold;
            AddFrame(_threatReadout.gameObject, CombatUiTokens.FrameHi);
        }

        private void BuildEnemySurface()
        {
            EnemyPanel = CreatePanelSlot("Combat Enemy Stage", CombatLayout.EnemyPanel, CombatUiTokens.PanelBg, true);
            EnemyTitleText = CreateTextSlot("Combat Enemy Title", CombatLayout.EnemyTitle, CombatUiTokens.Ink);
            EnemyHpFill = CreateGaugeSlot("Enemy HP Bar", CombatLayout.EnemyHpBar, CombatUiTokens.Hp);
            EnemyStatusText = CreateTextSlot("Combat Enemy Status Chips", CombatLayout.EnemyStatusChips, CombatUiTokens.InkDim);
            EnemyImage = CreateImageSlot("Combat Enemy Image", CombatLayout.EnemyImage, CombatUiTokens.InkDim);
            EnemyImage.preserveAspect = true;
            EnemyDamageText = CreateTextSlot("Combat Enemy Damage Number", CombatLayout.DamageEffectOverlay, CombatUiTokens.Gold);
            EnemyDamageText.gameObject.SetActive(false);
            MataiosDamageText = CreateChildText(EnemyPanel, "Combat Mataios Damage Number", string.Empty, UiTokenContract.LabelFontSize, CombatUiTokens.Sanity, new Vector2(0.54f, 0.30f), new Vector2(0.94f, 0.40f));
            MataiosDamageText.gameObject.SetActive(false);
            DefeatFeedbackText = CreateChildText(EnemyPanel, "Combat Defeat Feedback", string.Empty, UiTokenContract.PrimaryFontSize, CombatUiTokens.Gold, new Vector2(0.20f, 0.08f), new Vector2(0.80f, 0.20f));
            DefeatFeedbackText.gameObject.SetActive(false);
        }

        private void BuildPartySurface()
        {
            PartyDock = CreateFullScreenContainer("Combat Party Dock");
            PlayerCard = CreatePanelSlot("Combat Player Card", CombatLayout.PlayerCard, CombatUiTokens.PanelBgAlt, true, PartyDock);
            MataiosCard = CreatePanelSlot("Combat Mataios Card", CombatLayout.MataiosCard, CombatUiTokens.PanelBgAlt, true, PartyDock);
            PlayerHpFill = CreateGaugeSlot("Player HP Bar", CombatLayout.PlayerHpBar, CombatUiTokens.Hp, PartyDock);
            _playerSanityFill = CreateGaugeSlot("Player Sanity Bar", CombatLayout.PlayerSanityBar, CombatUiTokens.Sanity, PartyDock);
            MataiosHpFill = CreateGaugeSlot("Mataios HP Bar", CombatLayout.MataiosHpBar, CombatUiTokens.Sanity, PartyDock);
            _mataiosStateLabel = CreateTextSlot("Combat Mataios State Label", CombatLayout.MataiosStateLabel, CombatUiTokens.InkDim, PartyDock);

            PlayerPortrait = CreatePortrait(PlayerCard, "Combat Player Portrait", "P", out var fallback);
            PlayerPortraitFallbackText = fallback;
            MataiosPortrait = CreatePortrait(MataiosCard, "Combat Mataios Portrait", string.Empty, out _);
            PlayerPortraitFrame = CreatePortraitFrame(PlayerPortrait, "Combat Player Portrait Frame");
            MataiosPortraitFrame = CreatePortraitFrame(MataiosPortrait, "Combat Mataios Portrait Frame");
            PlayerCardText = CreateChildText(PlayerCard, "Combat Player Card Text", string.Empty, UiTokenContract.MicroFontSize, CombatUiTokens.Ink, new Vector2(0.34f, 0.34f), new Vector2(0.94f, 0.88f));
            MataiosCardText = CreateChildText(MataiosCard, "Combat Mataios Card Text", string.Empty, UiTokenContract.MicroFontSize, CombatUiTokens.Ink, new Vector2(0.34f, 0.34f), new Vector2(0.94f, 0.88f));
            PlayerDamageText = CreateChildText(PlayerCard, "Combat Player Damage Number", string.Empty, UiTokenContract.BodyFontSize, CombatUiTokens.Hp, new Vector2(0.04f, 0.48f), new Vector2(0.30f, 0.78f));
            PlayerDamageText.gameObject.SetActive(false);
            TrainingStatusIcon = CreateStatusIcon(PlayerCard, "Combat Training Status Icon", new Vector2(0.76f, 0.72f), new Vector2(0.84f, 0.90f));
            BandageStatusIcon = CreateStatusIcon(PlayerCard, "Combat Bandage Status Icon", new Vector2(0.84f, 0.72f), new Vector2(0.92f, 0.90f));
            RecallStatusIcon = CreateStatusIcon(PlayerCard, "Combat Recall Status Icon", new Vector2(0.68f, 0.72f), new Vector2(0.76f, 0.90f));
        }

        private void BuildActions(Action<CombatAction> onAction, Action onSkill, Action onItemInspect)
        {
            AttackButton = CreateActionButton("Combat Button Attack", CombatLayout.AttackButton, CombatAction.Attack, onAction, out var attackIcon);
            AttackIcon = attackIcon;
            DefendButton = CreateActionButton("Combat Button Defend", CombatLayout.DefendButton, CombatAction.Defend, onAction, out var defendIcon);
            DefendIcon = defendIcon;
            SkillButton = CreateActionButton("Combat Button Skill", CombatLayout.SkillButton, CombatAction.Skill, null, out var skillIcon);
            SkillIcon = skillIcon;
            SkillButton.onClick.AddListener(() => onSkill());
            ItemInspectButton = CreateSlotButton("Combat Item Inspect Button", CombatLayout.ItemInspectButton, CombatUiTokens.Frame);
            ItemInspectButton.onClick.AddListener(() => onItemInspect());
        }

        private void BuildCombatLog()
        {
            CombatLogPanel = CreatePanelSlot("Combat Log Panel", CombatLayout.CombatLog, CombatUiTokens.PanelBg, false);
            CombatLogText = CreateChildText(CombatLogPanel, "Combat Status Text", string.Empty, UiTokenContract.MicroFontSize, CombatUiTokens.InkDim, new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.92f));
            CombatLogText.alignment = TextAnchor.MiddleLeft;
        }

        private Text CreateChip(CombatSlot slot, Color accent)
        {
            var panel = CreatePanelSlot("Combat " + slot.Key, slot, CombatUiTokens.PanelBg, true);
            var accentImage = CreateImage(panel, "Accent", new Vector2(0.03f, 0.18f), new Vector2(0.06f, 0.82f), accent);
            accentImage.raycastTarget = false;
            return CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, CombatUiTokens.Ink, new Vector2(0.10f, 0.10f), new Vector2(0.94f, 0.90f));
        }

        private (Text text, Image fill) CreateGauge(CombatSlot slot, Color fillColor)
        {
            var panel = CreatePanelSlot("Combat " + slot.Key, slot, CombatUiTokens.PanelBg, true);
            var text = CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, CombatUiTokens.Ink, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.92f));
            var fill = CreateGaugeFill(panel, "Fill", new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.32f), fillColor);
            return (text, fill);
        }

        private Image CreateGaugeSlot(string name, CombatSlot slot, Color fillColor, Transform parent = null)
        {
            var frame = CreatePanelSlot(name + " Frame", slot, CombatUiTokens.VoidBg, false, parent);
            return CreateGaugeFill(frame, "Fill", Vector2.zero, Vector2.one, fillColor);
        }

        private Button CreateActionButton(string name, CombatSlot slot, CombatAction action, Action<CombatAction> onAction, out Image icon)
        {
            var button = CreateSlotButton(name, slot, CombatUiTokens.FrameHi);
            button.onClick.AddListener(() => onAction?.Invoke(action));
            icon = CreateImage(button.transform as RectTransform, "Icon", new Vector2(0.34f, 0.52f), new Vector2(0.66f, 0.88f), CombatUiTokens.Gold);
            icon.preserveAspect = true;
            var label = button.GetComponentInChildren<Text>();
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = UiTokenContract.BodyFontSize;
            return button;
        }

        private Button CreateSlotButton(string name, CombatSlot slot, Color frameColor)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(_root, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = CombatUiTokens.PanelBgAlt;
            AddFrame(buttonObject, frameColor);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(rect, "Label", slot.Content, slot.FontSize, CombatUiTokens.Ink, Vector2.zero, Vector2.one);
            label.raycastTarget = false;
            return button;
        }

        private RectTransform CreatePanelSlot(string name, CombatSlot slot, Color color, bool framed, Transform parent = null)
        {
            var panelObject = new GameObject(name, typeof(RectTransform));
            panelObject.transform.SetParent(parent ?? _root, false);
            var rect = panelObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            if (framed)
            {
                AddFrame(panelObject, CombatUiTokens.Frame);
            }

            return rect;
        }

        private Image CreateImageSlot(string name, CombatSlot slot, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform));
            imageObject.transform.SetParent(_root, false);
            var rect = imageObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateTextSlot(string name, CombatSlot slot, Color color, Transform parent = null)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent ?? _root, false);
            var rect = textObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var text = textObject.AddComponent<Text>();
            text.font = CombatUiTokens.ResolveRuntimeFont();
            text.fontSize = slot.FontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = slot.Content;
            text.raycastTarget = false;
            return text;
        }

        private RectTransform CreateFullScreenContainer(string name)
        {
            var containerObject = new GameObject(name, typeof(RectTransform));
            containerObject.transform.SetParent(_root, false);
            var rect = containerObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static Image CreateImage(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform));
            imageObject.transform.SetParent(parent, false);
            var rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text CreateChildText(RectTransform parent, string name, string content, int fontSize, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var text = textObject.AddComponent<Text>();
            text.font = CombatUiTokens.ResolveRuntimeFont();
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = content;
            text.raycastTarget = false;
            return text;
        }

        private static Image CreateGaugeFill(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var frame = CreateImage(parent, name + " Frame", anchorMin, anchorMax, CombatUiTokens.VoidBg);
            var fill = CreateImage(frame.rectTransform, name, new Vector2(0.02f, 0.18f), new Vector2(0.98f, 0.82f), color);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;
            return fill;
        }

        private static Image CreatePortrait(RectTransform parent, string name, string fallback, out Text fallbackText)
        {
            var portrait = CreateImage(parent, name, new Vector2(0.04f, 0.26f), new Vector2(0.30f, 0.90f), CombatUiTokens.InkDim);
            portrait.preserveAspect = true;
            fallbackText = string.IsNullOrEmpty(fallback)
                ? null
                : CreateChildText(portrait.rectTransform, name + " Label", fallback, UiTokenContract.BodyFontSize, CombatUiTokens.Ink, Vector2.zero, Vector2.one);
            return portrait;
        }

        private static Image CreatePortraitFrame(Image portrait, string name)
        {
            var frame = CreateImage(portrait.rectTransform.parent as RectTransform, name, portrait.rectTransform.anchorMin, portrait.rectTransform.anchorMax, CombatUiTokens.FrameHi);
            frame.gameObject.SetActive(false);
            return frame;
        }

        private static Image CreateStatusIcon(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var icon = CreateImage(parent, name, anchorMin, anchorMax, CombatUiTokens.Gold);
            icon.preserveAspect = true;
            return icon;
        }

        private static void AddFrame(GameObject target, Color color)
        {
            var outline = target.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
        }

        private static void ApplySlot(RectTransform rect, CombatSlot slot)
        {
            rect.anchorMin = CombatLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = CombatLayoutRuntime.AnchorMax(slot);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static float Ratio(int current, int maximum)
        {
            return maximum <= 0 ? 0f : Mathf.Clamp01((float)current / maximum);
        }

        private static string BuildThreatReadout(PrototypeRunSnapshot snapshot)
        {
            var survivableTurns = snapshot.EnemyAttack <= 0 ? 0 : Mathf.CeilToInt((float)Mathf.Max(0, snapshot.PlayerHp) / snapshot.EnemyAttack);
            return "⚔ > " + snapshot.EnemyAttack + "  ·  " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp + "  ·  " + survivableTurns + "T";
        }
    }
}
