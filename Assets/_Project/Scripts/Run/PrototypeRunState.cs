using System.Collections.Generic;
using HwigiTower.Abilities;
using HwigiTower.Combat;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.LLM;
using HwigiTower.NPC;

namespace HwigiTower.Run
{
    public readonly struct PrototypeCombatHandoffResolution
    {
        public PrototypeCombatHandoffResolution(bool applied, string resultId)
        {
            Applied = applied;
            ResultId = resultId ?? string.Empty;
        }

        public bool Applied { get; }
        public string ResultId { get; }
    }

    public sealed class PrototypeRunState
    {
        private const int BasePlayerMaxHp = 24;
        private const int BasePlayerAttack = 5;
        private const int FallbackEnemyHp = 12;
        private const int FallbackEnemyAttack = 3;
        private const int MinMental = -100;
        private const int MaxMental = 100;
        private const int MinGold = 0;
        private const int MaxGold = 999;
        private const int MinGlitchLevel = 0;
        private const int MaxGlitchLevel = 100;
        private const int MinAffinity = -100;
        private const int MaxAffinity = 100;
        private const int PrototypeShopAbilityCost = 10;

        private readonly GameFlowEventBus _eventBus;
        private readonly ReflectionPipeline _reflectionPipeline;
        private readonly SynergyDetector _synergyDetector;
        private readonly List<SynergyState> _activeSynergies = new List<SynergyState>();
        private readonly HashSet<string> _flags = new HashSet<string>();
        private readonly Dictionary<string, int> _items = new Dictionary<string, int>();
        private readonly HashSet<string> _abilityRefs = new HashSet<string>();
        private readonly HashSet<string> _rewardBundleRefs = new HashSet<string>();
        private readonly HashSet<string> _memoryFragmentRefs = new HashSet<string>();
        private readonly Dictionary<string, string> _resolvedEncounterChoices = new Dictionary<string, string>();
        private bool _runCompleted;
        private int _playerHp = BasePlayerMaxHp;
        private int _playerMaxHp = BasePlayerMaxHp;
        private int _playerAttack = BasePlayerAttack;
        private int _mental;
        private int _gold;
        private int _glitchLevel;
        private int _affinity;

        public PrototypeRunState(string runId, GameFlowEventBus eventBus)
        {
            RunId = runId ?? string.Empty;
            _eventBus = eventBus;
            MemoryRepo = new InMemoryNpcMemoryRepo();
            LLMProvider = new CachedLLMProvider(MemoryRepo, new OnDeviceLLMProvider(100, new DeterministicFakeLLMProvider()));
            _reflectionPipeline = new ReflectionPipeline(MemoryRepo, LLMProvider, eventBus);
            _synergyDetector = new SynergyDetector(eventBus, RunId);
            Abilities = new AbilityInventory(eventBus, RunId);
        }

        public string RunId { get; }
        public AbilityInventory Abilities { get; }
        public INPCMemoryRepo MemoryRepo { get; }
        public ILLMProvider LLMProvider { get; }
        public NpcStateMachine NpcStateMachine { get; private set; }
        public EncounterRuntimeCatalogData EncounterCatalog { get; private set; }
        public int NodesResolved { get; private set; }
        public int BattlesWon { get; private set; }
        public bool RunCompleted => _runCompleted;
        public int PlayerHp => _playerHp;
        public int PlayerMaxHp => _playerMaxHp;
        public int PlayerAttack => _playerAttack;
        public int Mental => _mental;
        public int Gold => _gold;
        public int GlitchLevel => _glitchLevel;
        public int Affinity => _affinity;
        public IReadOnlyCollection<string> AbilityRefs => _abilityRefs;
        public IReadOnlyCollection<string> RewardBundleRefs => _rewardBundleRefs;
        public IReadOnlyCollection<string> MemoryFragmentRefs => _memoryFragmentRefs;

        public PrototypeRunSnapshot CreateSnapshot()
        {
            return new PrototypeRunSnapshot(
                RunId,
                _playerHp,
                _playerMaxHp,
                _playerAttack,
                _mental,
                _gold,
                _glitchLevel,
                _affinity,
                NodesResolved,
                BattlesWon,
                CountGrantedAbilities(),
                _runCompleted);
        }

        public int ModifyMental(int amount)
        {
            _mental = Clamp(_mental + amount, MinMental, MaxMental);
            return _mental;
        }

        public int ModifyGold(int amount)
        {
            _gold = Clamp(_gold + amount, MinGold, MaxGold);
            return _gold;
        }

        public int ModifyGlitchLevel(int amount)
        {
            _glitchLevel = Clamp(_glitchLevel + amount, MinGlitchLevel, MaxGlitchLevel);
            return _glitchLevel;
        }

        public int ModifyAffinity(int amount)
        {
            _affinity = Clamp(_affinity + amount, MinAffinity, MaxAffinity);
            return _affinity;
        }

        public int ModifyPlayerHp(int amount)
        {
            _playerHp = Clamp(_playerHp + amount, 0, _playerMaxHp);
            return _playerHp;
        }

        public void SetFlag(string flag, bool value)
        {
            if (string.IsNullOrEmpty(flag))
            {
                return;
            }

            if (value)
            {
                _flags.Add(flag);
            }
            else
            {
                _flags.Remove(flag);
            }
        }

        public bool HasFlag(string flag)
        {
            return !string.IsNullOrEmpty(flag) && _flags.Contains(flag);
        }

        public int AddItemRef(string itemRef, int count)
        {
            if (string.IsNullOrEmpty(itemRef) || count == 0)
            {
                return GetItemCount(itemRef);
            }

            if (count > 0 && EncounterCatalog != null && !EncounterCatalog.TryGetItem(itemRef, out _))
            {
                return GetItemCount(itemRef);
            }

            var next = Clamp(GetItemCount(itemRef) + count, 0, 999);
            if (next == 0)
            {
                _items.Remove(itemRef);
            }
            else
            {
                _items[itemRef] = next;
            }

            return next;
        }

        public int GetItemCount(string itemRef)
        {
            return !string.IsNullOrEmpty(itemRef) && _items.TryGetValue(itemRef, out var count) ? count : 0;
        }

        public bool AddAbilityRef(string abilityRef)
        {
            if (string.IsNullOrEmpty(abilityRef) || !_abilityRefs.Add(abilityRef))
            {
                return false;
            }

            if (EncounterCatalog != null && EncounterCatalog.TryGetAbility(abilityRef, out var ability))
            {
                Abilities.Add(ability);
            }

            return true;
        }

        public bool HasAbilityRef(string abilityRef)
        {
            if (string.IsNullOrEmpty(abilityRef) || _abilityRefs.Contains(abilityRef))
            {
                return !string.IsNullOrEmpty(abilityRef) && _abilityRefs.Contains(abilityRef);
            }

            for (var i = 0; i < Abilities.Abilities.Count; i++)
            {
                if (Abilities.Abilities[i] != null && Abilities.Abilities[i].Id == abilityRef)
                {
                    return true;
                }
            }

            return false;
        }

        public bool GrantRewardBundleRef(string rewardBundleRef)
        {
            if (string.IsNullOrEmpty(rewardBundleRef) || !_rewardBundleRefs.Add(rewardBundleRef))
            {
                return false;
            }

            if (EncounterCatalog != null && EncounterCatalog.TryGetRewardBundle(rewardBundleRef, out var rewardBundle))
            {
                ApplyRewardBundle(rewardBundle);
            }

            return true;
        }

        public bool HasRewardBundleRef(string rewardBundleRef)
        {
            return !string.IsNullOrEmpty(rewardBundleRef) && _rewardBundleRefs.Contains(rewardBundleRef);
        }

        public bool UnlockMemoryFragmentRef(string memoryFragmentRef)
        {
            if (string.IsNullOrEmpty(memoryFragmentRef))
            {
                return false;
            }

            if (EncounterCatalog != null && !EncounterCatalog.TryGetMemoryFragment(memoryFragmentRef, out _))
            {
                return false;
            }

            return _memoryFragmentRefs.Add(memoryFragmentRef);
        }

        public bool HasMemoryFragmentRef(string memoryFragmentRef)
        {
            return !string.IsNullOrEmpty(memoryFragmentRef) && _memoryFragmentRefs.Contains(memoryFragmentRef);
        }

        public bool HasResolvedEncounterChoice(string nodeId, string encounterId)
        {
            return TryGetResolvedEncounterChoice(nodeId, encounterId, out _);
        }

        public bool TryGetResolvedEncounterChoice(string nodeId, string encounterId, out string choiceStableId)
        {
            return _resolvedEncounterChoices.TryGetValue(BuildResolvedEncounterKey(nodeId, encounterId), out choiceStableId);
        }

        public void MarkEncounterChoiceResolved(string nodeId, string encounterId, string choiceStableId)
        {
            if (string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(encounterId) || string.IsNullOrEmpty(choiceStableId))
            {
                return;
            }

            _resolvedEncounterChoices[BuildResolvedEncounterKey(nodeId, encounterId)] = choiceStableId;
        }

        public void AttachNpcStateMachine(NpcStateMachine stateMachine)
        {
            NpcStateMachine = stateMachine;
        }

        public void AttachEncounterCatalog(EncounterRuntimeCatalogData catalog)
        {
            EncounterCatalog = catalog;
        }

        public PrototypeNodeResolution ResolveBattle(DeterministicRunContext context, string nodeId, string encounterId, EnemyData enemyData, IReadOnlyList<SynergyData> trackedSynergies)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, encounterId, "run already completed", true);
            }

            EvaluateSynergies(trackedSynergies);
            RecalculatePlayerStats();
            var player = new CombatantState("player", _playerMaxHp, _playerAttack, _playerHp);
            var enemy = CreateEnemyState(enemyData);
            var combat = new CombatController(context, string.IsNullOrEmpty(encounterId) ? nodeId : encounterId);

            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatStarted, RunId, nodeId, enemy.Id));

            var rounds = 0;
            CombatRoundResult round;
            do
            {
                round = combat.ResolveRound(player, enemy);
                rounds++;
            }
            while (!round.IsComplete && rounds < 12);

            NodesResolved++;
            if (enemy.IsDefeated)
            {
                BattlesWon++;
            }

            _playerHp = player.Hp;
            var resultId = enemy.IsDefeated ? "victory" : "defeat";
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatCompleted, RunId, nodeId, resultId));
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, resultId));

            if (player.IsDefeated)
            {
                ApplyNpcTrigger("battle.defeat");
                CompleteRun("defeat");
            }
            else if (enemy.IsDefeated)
            {
                ApplyNpcTrigger("battle.victory");
            }

            var message = $"{resultId} | player {player.Hp}/{player.MaxHp} | enemy {enemy.Hp}/{enemy.MaxHp} | rounds {rounds}";
            return new PrototypeNodeResolution(nodeId, encounterId, message, _runCompleted);
        }

        public PrototypeNodeResolution ResolveRest(string nodeId)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, string.Empty, "run already completed", true);
            }

            NodesResolved++;
            _playerHp = _playerMaxHp;
            var recall = _reflectionPipeline.LoadRecallPrompt(RunId, 3);
            ApplyNpcTrigger("rest.recall");
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "rest"));
            return new PrototypeNodeResolution(nodeId, "recall", recall, false);
        }

        public PrototypeNodeResolution ResolveShop(string nodeId, AbilityData grantedAbility, IReadOnlyList<SynergyData> trackedSynergies)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, string.Empty, "run already completed", true);
            }

            NodesResolved++;
            if (grantedAbility == null)
            {
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-empty"));
                return new PrototypeNodeResolution(nodeId, string.Empty, "shop unavailable", false);
            }

            if (_gold < PrototypeShopAbilityCost)
            {
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-insufficient-gold"));
                return new PrototypeNodeResolution(nodeId, string.Empty, $"purchase failed: gold {_gold}/{PrototypeShopAbilityCost}", false);
            }

            var added = Abilities.Add(grantedAbility);
            if (!added)
            {
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-empty"));
                return new PrototypeNodeResolution(nodeId, string.Empty, "purchase failed: ability unavailable", false);
            }

            ModifyGold(-PrototypeShopAbilityCost);
            EvaluateSynergies(trackedSynergies);
            RecalculatePlayerStats();
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "shop-purchase-success"));
            return new PrototypeNodeResolution(nodeId, grantedAbility.Id, $"purchase success: {grantedAbility.Id}", false);
        }

        public PrototypeNodeResolution ResolveRemnant(string nodeId)
        {
            if (!_runCompleted)
            {
                NodesResolved++;
                CompleteRun("remnant");
            }

            return new PrototypeNodeResolution(nodeId, "run.completed", "run completed: reflection saved", true);
        }

        public PrototypeNodeResolution ResolveGeneric(string nodeId, string payloadId, string message)
        {
            if (!_runCompleted)
            {
                NodesResolved++;
                _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, payloadId));
            }

            return new PrototypeNodeResolution(nodeId, payloadId, message, _runCompleted);
        }

        public PrototypeNodeResolution ResolveEncounterChoice(string nodeId, EncounterData encounter, string choiceStableId)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, encounter == null ? string.Empty : encounter.Id, "run already completed", true);
            }

            var encounterId = encounter == null ? string.Empty : encounter.Id;
            if (TryGetResolvedEncounterChoice(nodeId, encounterId, out var resolvedChoiceStableId))
            {
                return new PrototypeNodeResolution(nodeId, resolvedChoiceStableId, $"already resolved: {resolvedChoiceStableId}", false);
            }

            var resolution = PrototypeEncounterRuntimeResolver.Resolve(this, encounter, choiceStableId, new DeterministicRunContext(RunId, 0), nodeId);
            if (resolution.Applied)
            {
                MarkEncounterChoiceResolved(nodeId, encounterId, resolution.ChoiceStableId);
            }

            NodesResolved++;
            var payloadId = resolution.ChoiceStableId;
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, payloadId));
            return new PrototypeNodeResolution(nodeId, payloadId, resolution.Message, false);
        }

        public PrototypeNodeResolution ResolveEncounterChoice(DeterministicRunContext context, string nodeId, EncounterData encounter, string choiceStableId)
        {
            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, encounter == null ? string.Empty : encounter.Id, "run already completed", true);
            }

            var encounterId = encounter == null ? string.Empty : encounter.Id;
            if (TryGetResolvedEncounterChoice(nodeId, encounterId, out var resolvedChoiceStableId))
            {
                return new PrototypeNodeResolution(nodeId, resolvedChoiceStableId, $"already resolved: {resolvedChoiceStableId}", false);
            }

            var resolution = PrototypeEncounterRuntimeResolver.Resolve(this, encounter, choiceStableId, context, nodeId);
            if (resolution.Applied)
            {
                MarkEncounterChoiceResolved(nodeId, encounterId, resolution.ChoiceStableId);
            }

            NodesResolved++;
            var payloadId = resolution.ChoiceStableId;
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, payloadId));
            return new PrototypeNodeResolution(nodeId, payloadId, resolution.Message, false);
        }

        public PrototypeCombatHandoffResolution ResolveCombatHandoff(
            DeterministicRunContext context,
            string nodeId,
            EncounterData encounter,
            EncounterCombatHandoffRuntimeData handoff,
            System.Action<PrototypeRunState, EncounterPostCombatEffectRuntimeData[]> applyEffects)
        {
            if (_runCompleted || handoff == null)
            {
                return new PrototypeCombatHandoffResolution(false, string.Empty);
            }

            if (string.IsNullOrEmpty(context.RunId))
            {
                context = new DeterministicRunContext(RunId, 0);
            }

            RecalculatePlayerStats();
            var player = new CombatantState("player", _playerMaxHp, _playerAttack, _playerHp);
            var enemyId = handoff.enemyRefs != null && handoff.enemyRefs.Length > 0 && !string.IsNullOrEmpty(handoff.enemyRefs[0])
                ? handoff.enemyRefs[0]
                : "enemy.placeholder";
            var enemy = EncounterCatalog != null && EncounterCatalog.TryGetEnemy(enemyId, out var enemyData)
                ? CreateEnemyState(enemyData)
                : new CombatantState(enemyId, FallbackEnemyHp, FallbackEnemyAttack);
            var combatId = string.IsNullOrEmpty(handoff.stableId) ? encounter == null ? nodeId : encounter.Id : handoff.stableId;
            var combat = new CombatController(context, string.IsNullOrEmpty(handoff.seedKey) ? combatId : handoff.seedKey);

            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatStarted, RunId, nodeId, enemy.Id));

            var rounds = 0;
            CombatRoundResult round;
            do
            {
                round = combat.ResolveRound(player, enemy);
                rounds++;
            }
            while (!round.IsComplete && rounds < 12);

            if (enemy.IsDefeated)
            {
                BattlesWon++;
            }

            _playerHp = player.Hp;
            var resultId = enemy.IsDefeated ? "victory" : "defeat";
            applyEffects?.Invoke(this, enemy.IsDefeated ? handoff.onVictoryEffects : handoff.onDefeatEffects);
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatCompleted, RunId, nodeId, resultId));

            if (player.IsDefeated)
            {
                ApplyNpcTrigger("battle.defeat");
                CompleteRun("defeat");
            }
            else if (enemy.IsDefeated)
            {
                ApplyNpcTrigger("battle.victory");
            }

            return new PrototypeCombatHandoffResolution(true, resultId);
        }

        private int CountGrantedAbilities()
        {
            var count = Abilities.Abilities.Count;
            foreach (var abilityRef in _abilityRefs)
            {
                var hasRuntimeAbility = false;
                for (var i = 0; i < Abilities.Abilities.Count; i++)
                {
                    if (Abilities.Abilities[i] != null && Abilities.Abilities[i].Id == abilityRef)
                    {
                        hasRuntimeAbility = true;
                        break;
                    }
                }

                if (!hasRuntimeAbility)
                {
                    count++;
                }
            }

            return count;
        }

        private void ApplyRewardBundle(HwigiTower.Rewards.RewardBundleData rewardBundle)
        {
            if (rewardBundle == null || rewardBundle.Entries == null)
            {
                return;
            }

            for (var i = 0; i < rewardBundle.Entries.Length; i++)
            {
                var entry = rewardBundle.Entries[i];
                if (entry == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(entry.ItemRef))
                {
                    AddItemRef(entry.ItemRef, System.Math.Max(1, entry.ItemCount));
                }

                if (!string.IsNullOrEmpty(entry.AbilityRef))
                {
                    AddAbilityRef(entry.AbilityRef);
                }
            }
        }

        private void EvaluateSynergies(IReadOnlyList<SynergyData> trackedSynergies)
        {
            _activeSynergies.Clear();
            _activeSynergies.AddRange(_synergyDetector.Evaluate(Abilities.Abilities, trackedSynergies));
        }

        private void RecalculatePlayerStats()
        {
            var previousMaxHp = _playerMaxHp;
            var modifiers = CombatAbilityModifiers.From(Abilities.Abilities, _activeSynergies);
            _playerMaxHp = System.Math.Max(1, BasePlayerMaxHp + modifiers.PlayerMaxHpBonus);
            _playerAttack = System.Math.Max(0, BasePlayerAttack + modifiers.PlayerAttackBonus);

            if (_playerMaxHp > previousMaxHp)
            {
                _playerHp += _playerMaxHp - previousMaxHp;
            }

            _playerHp = System.Math.Max(0, System.Math.Min(_playerHp, _playerMaxHp));
        }

        private void CompleteRun(string cause)
        {
            if (_runCompleted)
            {
                return;
            }

            _runCompleted = true;
            var summary = $"cause={cause};nodes={NodesResolved};battles={BattlesWon}";
            _reflectionPipeline.TrySaveReflection(RunId, summary, out _);
            ApplyNpcTrigger("run.completed");
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.RunCompleted, RunId, RunId, cause));
        }

        private void ApplyNpcTrigger(string triggerId)
        {
            NpcStateMachine?.TryApply(triggerId);
        }

        private static int Clamp(int value, int min, int max)
        {
            return System.Math.Max(min, System.Math.Min(value, max));
        }

        private static string BuildResolvedEncounterKey(string nodeId, string encounterId)
        {
            return (nodeId ?? string.Empty) + "::" + (encounterId ?? string.Empty);
        }

        private static CombatantState CreateEnemyState(EnemyData enemyData)
        {
            if (enemyData == null)
            {
                return new CombatantState("enemy.placeholder", FallbackEnemyHp, FallbackEnemyAttack);
            }

            return new CombatantState(enemyData.Id, enemyData.Hp, enemyData.Attack);
        }
    }
}
