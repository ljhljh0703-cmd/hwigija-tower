using HwigiTower.Combat;
using HwigiTower.Run;
using UnityEngine;

namespace HwigiTower.UI
{
    [System.Serializable]
    public sealed class DemoPresentationSlot
    {
        [SerializeField] private string encounterStableId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private string titleTextKey = string.Empty;
        [SerializeField] private string bodyTextKey = string.Empty;
        [SerializeField] private Sprite mataiosPortrait;
        [SerializeField] private Sprite enemySprite;
        [SerializeField] private Sprite memoryFragmentSprite;
        [SerializeField] private AudioClip sfxCue;
        [SerializeField] private AudioClip bgmCue;
        [SerializeField] private CutsceneData memoryFragmentCutscene;
        [SerializeField] private CutsceneData combatStartCutscene;
        [SerializeField] private CutsceneData demoCompleteCutscene;

        public string EncounterStableId => encounterStableId;
        public string DisplayName => displayName;
        public Sprite BackgroundSprite => backgroundSprite;
        public string TitleTextKey => titleTextKey;
        public string BodyTextKey => bodyTextKey;
        public Sprite MataiosPortrait => mataiosPortrait;
        public Sprite EnemySprite => enemySprite;
        public Sprite MemoryFragmentSprite => memoryFragmentSprite;
        public AudioClip SfxCue => sfxCue;
        public AudioClip BgmCue => bgmCue;
        public CutsceneData MemoryFragmentCutscene => memoryFragmentCutscene;
        public CutsceneData CombatStartCutscene => combatStartCutscene;
        public CutsceneData DemoCompleteCutscene => demoCompleteCutscene;
    }

    [System.Serializable]
    public sealed class DemoMerchantPresentationSlot
    {
        [SerializeField, Min(1)] private int floor = 1;
        [SerializeField] private string stateKey = string.Empty;
        [SerializeField] private Sprite merchantSprite;

        public int Floor => floor < 1 ? 1 : floor;
        public string StateKey => stateKey;
        public Sprite MerchantSprite => merchantSprite;
    }

    [System.Serializable]
    public sealed class DemoNodeIconSlot
    {
        [SerializeField] private PrototypeFloorMapNodeType nodeType;
        [SerializeField] private Sprite icon;

        public PrototypeFloorMapNodeType NodeType => nodeType;
        public Sprite Icon => icon;
    }

    [System.Serializable]
    public sealed class DemoCombatActionIconSlot
    {
        [SerializeField] private CombatAction action;
        [SerializeField] private Sprite icon;

        public CombatAction Action => action;
        public Sprite Icon => icon;
    }

    [System.Serializable]
    public sealed class DemoRestActionIconSlot
    {
        [SerializeField] private string actionId = string.Empty;
        [SerializeField] private Sprite icon;

        public string ActionId => actionId;
        public Sprite Icon => icon;
    }

    [System.Serializable]
    public sealed class DemoIconSlot
    {
        [SerializeField] private string key = string.Empty;
        [SerializeField] private Sprite icon;

        public string Key => key;
        public Sprite Icon => icon;
    }

    [System.Serializable]
    public sealed class DemoFloorMapBackgroundSlot
    {
        [SerializeField, Min(1)] private int floor = 1;
        [SerializeField] private Sprite mapBackground;

        public int Floor => floor < 1 ? 1 : floor;
        public Sprite MapBackground => mapBackground;
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Demo Presentation Data", fileName = "SO_DemoPresentationData")]
    public sealed class DemoPresentationData : ScriptableObject
    {
        [SerializeField] private Sprite defaultPlayerPortrait;
        [SerializeField] private Sprite defaultMataiosPortrait;
        [SerializeField] private Sprite combatMataiosPortrait;
        [SerializeField] private Sprite combatPortraitFrame;
        [SerializeField] private Sprite completedNodeBackground;
        [SerializeField] private Sprite selectedNodeRing;
        [SerializeField] private Sprite lockedNodeOverlay;
        [SerializeField] private Sprite currentPositionMarker;
        [SerializeField] private Sprite spotlightGradient;
        [SerializeField] private Sprite npcDialoguePlate;
        [SerializeField] private Sprite shopProductCard;
        [SerializeField] private Sprite shopLockedCard;
        [SerializeField] private Sprite completeBackgroundSprite;
        [SerializeField] private DemoFloorMapBackgroundSlot[] floorMapBackgroundSlots = new DemoFloorMapBackgroundSlot[0];
        [SerializeField] private DemoMerchantPresentationSlot[] merchantSlots = new DemoMerchantPresentationSlot[0];
        [SerializeField] private DemoNodeIconSlot[] nodeIconSlots = new DemoNodeIconSlot[0];
        [SerializeField] private DemoCombatActionIconSlot[] combatActionIconSlots = new DemoCombatActionIconSlot[0];
        [SerializeField] private DemoRestActionIconSlot[] restActionIconSlots = new DemoRestActionIconSlot[0];
        [SerializeField] private DemoIconSlot[] iconSlots = new DemoIconSlot[0];
        [SerializeField] private DemoPresentationSlot[] slots = new DemoPresentationSlot[0];

        public Sprite DefaultPlayerPortrait => defaultPlayerPortrait;
        public Sprite DefaultMataiosPortrait => defaultMataiosPortrait;
        public Sprite CombatMataiosPortrait => combatMataiosPortrait == null ? defaultMataiosPortrait : combatMataiosPortrait;
        public Sprite CombatPortraitFrame => combatPortraitFrame;
        public Sprite CompletedNodeBackground => completedNodeBackground;
        public Sprite SelectedNodeRing => selectedNodeRing;
        public Sprite LockedNodeOverlay => lockedNodeOverlay;
        public Sprite CurrentPositionMarker => currentPositionMarker;
        public Sprite SpotlightGradient => spotlightGradient;
        public Sprite NpcDialoguePlate => npcDialoguePlate;
        public Sprite ShopProductCard => shopProductCard;
        public Sprite ShopLockedCard => shopLockedCard;
        public Sprite CompleteBackgroundSprite => completeBackgroundSprite;
        public DemoFloorMapBackgroundSlot[] FloorMapBackgroundSlots => floorMapBackgroundSlots ?? new DemoFloorMapBackgroundSlot[0];
        public DemoMerchantPresentationSlot[] MerchantSlots => merchantSlots ?? new DemoMerchantPresentationSlot[0];
        public DemoNodeIconSlot[] NodeIconSlots => nodeIconSlots ?? new DemoNodeIconSlot[0];
        public DemoCombatActionIconSlot[] CombatActionIconSlots => combatActionIconSlots ?? new DemoCombatActionIconSlot[0];
        public DemoRestActionIconSlot[] RestActionIconSlots => restActionIconSlots ?? new DemoRestActionIconSlot[0];
        public DemoIconSlot[] IconSlots => iconSlots ?? new DemoIconSlot[0];
        public DemoPresentationSlot[] Slots => slots ?? new DemoPresentationSlot[0];

        public bool TryGetCombatActionIcon(CombatAction action, out Sprite icon)
        {
            var source = CombatActionIconSlots;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].Action == action && source[i].Icon != null)
                {
                    icon = source[i].Icon;
                    return true;
                }
            }

            icon = null;
            return false;
        }

        public bool TryGetRestActionIcon(string actionId, out Sprite icon)
        {
            var source = RestActionIconSlots;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].ActionId == actionId && source[i].Icon != null)
                {
                    icon = source[i].Icon;
                    return true;
                }
            }

            icon = null;
            return false;
        }

        public bool TryGetIcon(string key, out Sprite icon)
        {
            var source = IconSlots;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].Key == key && source[i].Icon != null)
                {
                    icon = source[i].Icon;
                    return true;
                }
            }

            icon = null;
            return false;
        }

        public bool TryGetNodeIcon(PrototypeFloorMapNodeType nodeType, out Sprite icon)
        {
            var source = NodeIconSlots;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].NodeType == nodeType && source[i].Icon != null)
                {
                    icon = source[i].Icon;
                    return true;
                }
            }

            icon = null;
            return false;
        }

        public bool TryGetCompletedNodeBackground(out Sprite sprite)
        {
            sprite = completedNodeBackground;
            return sprite != null;
        }

        public bool TryGetFloorMapBackground(int floor, out Sprite sprite)
        {
            sprite = null;
            var source = FloorMapBackgroundSlots;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].Floor == floor && source[i].MapBackground != null)
                {
                    sprite = source[i].MapBackground;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetMerchantSprite(int floor, out Sprite sprite)
        {
            sprite = null;
            var source = MerchantSlots;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].Floor == floor && source[i].MerchantSprite != null)
                {
                    sprite = source[i].MerchantSprite;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetSlot(string encounterStableId, out DemoPresentationSlot slot)
        {
            slot = null;
            if (string.IsNullOrEmpty(encounterStableId))
            {
                return false;
            }

            var source = Slots;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].EncounterStableId == encounterStableId)
                {
                    slot = source[i];
                    return true;
                }
            }

            return false;
        }

        public bool TryGetCutscene(string encounterStableId, PrototypeCutsceneTrigger trigger, out CutsceneData cutscene)
        {
            cutscene = null;
            if (!TryGetSlot(encounterStableId, out var slot))
            {
                return false;
            }

            cutscene = trigger switch
            {
                PrototypeCutsceneTrigger.MemoryFragmentUnlock => slot.MemoryFragmentCutscene,
                PrototypeCutsceneTrigger.CombatGateStart => slot.CombatStartCutscene,
                PrototypeCutsceneTrigger.DemoComplete => slot.DemoCompleteCutscene,
                _ => null
            };

            return cutscene != null && cutscene.HasSteps;
        }
    }
}
