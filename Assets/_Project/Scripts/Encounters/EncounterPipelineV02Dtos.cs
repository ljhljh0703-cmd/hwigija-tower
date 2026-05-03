using System;

namespace HwigiTower.Encounters
{
    [Serializable]
    public sealed class EncounterPack
    {
        public string schemaVersion;
        public string packStableId;
        public string sourceHash;
        public AssetBakeConfig assetBake;
        public PackManifest packManifest;
        public EncounterDefinition[] encounters = new EncounterDefinition[0];
    }

    [Serializable]
    public sealed class AssetBakeConfig
    {
        public string mode;
        public string identity;
        public string basePath;
        public string assetPathPattern;
        public bool prohibitDeleteRecreate;
    }

    [Serializable]
    public sealed class PackManifest
    {
        public ExpectedFullCounts expectedFullCounts;
    }

    [Serializable]
    public sealed class ExpectedFullCounts
    {
        public int NpcRest;
        public int NpcMemoryFragment;
        public int GeneralEncounter;
        public int MoralChoice;
        public int MetaText;
    }

    [Serializable]
    public sealed class EncounterDefinition
    {
        public string kind;
        public string stableId;
        public string assetPath;
        public string seedKey;
        public string sourceHash;
        public int floor;
        public string encounterType;
        public string contentCategory;
        public string[] tags = new string[0];
        public string npcStage;
        public MemoryFragmentRef memoryFragment;
        public int glitchLevel;
        public NodeRules nodeRules;
        public string bodyTextKey;
        public string writerStatus;
        public ChoiceDefinition[] choices = new ChoiceDefinition[0];
    }

    [Serializable]
    public sealed class MemoryFragmentRef
    {
        public string stableId;
        public string stage;
        public string textKey;
    }

    [Serializable]
    public sealed class NodeRules
    {
        public int choiceCountMin;
        public int choiceCountMax;
        public int timeLimitSeconds;
    }

    [Serializable]
    public sealed class ChoiceDefinition
    {
        public string kind;
        public string stableId;
        public string textKey;
        public string requirementMode;
        public RequirementDefinition[] requirements = new RequirementDefinition[0];
        public EffectDefinition[] effects = new EffectDefinition[0];
        public UnavailablePolicy unavailablePolicy;
        public NpcImmediateReaction npcImmediateReaction;
    }

    [Serializable]
    public sealed class RequirementDefinition
    {
        public string kind;
        public string flag;
        public bool expected;
        public string op;
        public int value;
        public string itemRef;
        public int minCount;
        public string abilityRef;
        public string stat;
        public string npcStage;
        public int min;
        public int max;
        public string memoryFragmentId;
        public int minFloor;
        public int maxFloor;
    }

    [Serializable]
    public sealed class EffectDefinition
    {
        public string kind;
        public int amount;
        public string clamp;
        public bool clampToMax;
        public string flag;
        public bool value;
        public string itemRef;
        public int count;
        public string abilityRef;
        public string statusRef;
        public int duration;
        public MemoryFragmentRef memoryFragment;
        public string npcStage;
        public string targetEncounterId;
        public string targetNodeId;
        public string rewardBundleRef;
        public CombatHandoff combatHandoff;
    }

    [Serializable]
    public sealed class UnavailablePolicy
    {
        public string mode;
        public string reasonTextKey;
    }

    [Serializable]
    public sealed class NpcImmediateReaction
    {
        public string reactionKey;
        public string stage;
        public string toneHint;
        public string textKey;
    }

    [Serializable]
    public sealed class CombatHandoff
    {
        public string kind;
        public string stableId;
        public string sourceEncounterId;
        public string sourceChoiceId;
        public string seedKey;
        public string sourceHash;
        public int floor;
        public string[] enemyRefs = new string[0];
        public PlayerSnapshotCaptureSpec playerSnapshot;
        public string npcStage;
        public EffectDefinition[] onVictoryEffects = new EffectDefinition[0];
        public EffectDefinition[] onDefeatEffects = new EffectDefinition[0];
        public DeathPolicy deathPolicy;
        public ReturnNodePolicy returnNode;
        public PostCombatText postCombatText;
        public NpcImmediateReaction npcImmediateReaction;
        public string promptHash;
        public string cacheKey;
    }

    [Serializable]
    public sealed class PlayerSnapshotCaptureSpec
    {
        public string kind;
        public string captureMode;
        public string[] include = new string[0];
    }

    [Serializable]
    public sealed class DeathPolicy
    {
        public string mode;
        public string memoryRetention;
    }

    [Serializable]
    public sealed class ReturnNodePolicy
    {
        public string onVictory;
        public string onDefeat;
    }

    [Serializable]
    public sealed class PostCombatText
    {
        public string victoryTextKey;
        public string defeatTextKey;
        public string deathTextKey;
    }
}
