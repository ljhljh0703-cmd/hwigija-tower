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
            : this(applied, resultId, string.Empty, string.Empty)
        {
        }

        public PrototypeCombatHandoffResolution(bool applied, string resultId, string combatStableId, string enemyId)
        {
            Applied = applied;
            ResultId = resultId ?? string.Empty;
            CombatStableId = combatStableId ?? string.Empty;
            EnemyId = enemyId ?? string.Empty;
        }

        public bool Applied { get; }
        public string ResultId { get; }
        public string CombatStableId { get; }
        public string EnemyId { get; }
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
        private readonly List<PrototypeDemoRunStep> _demoRunPath = new List<PrototypeDemoRunStep>();
        private readonly List<PrototypeFloorRunPath> _floorRunPaths = new List<PrototypeFloorRunPath>();
        private readonly HashSet<string> _resolvedDemoSteps = new HashSet<string>();
        private bool _runCompleted;
        private bool _runClear;
        private bool _stairUnlocked;
        private int _playerHp = BasePlayerMaxHp;
        private int _playerMaxHp = BasePlayerMaxHp;
        private int _playerAttack = BasePlayerAttack;
        private int _mental;
        private int _gold;
        private int _glitchLevel;
        private int _affinity;
        private int _currentFloor = 1;
        private string _lastMemoryFragmentId = string.Empty;
        private string _lastMemoryFragmentTitleKey = string.Empty;
        private string _lastMemoryFragmentBodyKey = string.Empty;
        private string _lastCombatId = string.Empty;
        private string _lastCombatEnemyId = string.Empty;
        private string _lastCombatResultId = string.Empty;
        private string _lastCombatRoundResult = string.Empty;
        private int _combatRound;
        private int _lastCombatGoldReward;
        private int _lastCombatGlitchDelta;
        private int _lastCombatAffinityDelta;
        private bool _lastCombatEnemyDefeated;
        private int _lastCombatComboDamage;
        private string _lastNpcReactionKey = string.Empty;

        public PrototypeRunState(string runId, GameFlowEventBus eventBus)
            : this(runId, eventBus, null)
        {
        }

        public PrototypeRunState(string runId, GameFlowEventBus eventBus, LLMRuntimeSettings llmRuntimeSettings)
        {
            RunId = runId ?? string.Empty;
            _eventBus = eventBus;
            MemoryRepo = new InMemoryNpcMemoryRepo();
            LLMProvider = LLMProviderFactory.Create(llmRuntimeSettings, MemoryRepo);
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
        public bool DemoComplete => _runClear;
        public bool FloorComplete => _demoRunPath.Count > 0 && _resolvedDemoSteps.Count >= _demoRunPath.Count;
        public bool StairUnlocked => _stairUnlocked;
        public bool RunClear => _runClear;
        public string DemoStatus => _runClear ? "run.clear" : _stairUnlocked ? "stair.unlocked" : _demoRunPath.Count == 0 ? "demo.unconfigured" : "demo.active";
        public int DemoStepCount => _demoRunPath.Count;
        public int DemoResolvedStepCount => _resolvedDemoSteps.Count;
        public int CurrentFloor => _currentFloor;
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
        public string LastMemoryFragmentId => _lastMemoryFragmentId;
        public string LastMemoryFragmentTitleKey => _lastMemoryFragmentTitleKey;
        public string LastMemoryFragmentBodyKey => _lastMemoryFragmentBodyKey;
        public string LastCombatId => _lastCombatId;
        public string LastCombatEnemyId => _lastCombatEnemyId;
        public string LastCombatResultId => _lastCombatResultId;
        public string LastNpcReactionKey => _lastNpcReactionKey;

        public PrototypeRunSnapshot CreateSnapshot()
        {
            return new PrototypeRunSnapshot(
                RunId,
                _activeCombatPlayer?.Hp ?? _playerHp,
                _activeCombatPlayer?.MaxHp ?? _playerMaxHp,
                _playerAttack,
                _mental,
                _gold,
                _glitchLevel,
                _affinity,
                NodesResolved,
                BattlesWon,
                CountGrantedAbilities(),
                _runCompleted,
                DemoStatus,
                NextDemoNodeId,
                NextDemoEncounterId,
                DemoStepCount,
                DemoResolvedStepCount,
                _memoryFragmentRefs.Count,
                LastMemoryFragmentId,
                LastMemoryFragmentTitleKey,
                LastMemoryFragmentBodyKey,
                LastCombatId,
                LastCombatEnemyId,
                LastCombatResultId,
                _lastCombatRoundResult,
                IsInCombat,
                _activeCombatEnemy?.Hp ?? 0,
                _activeCombatEnemy?.MaxHp ?? 0,
                _combatRound,
                _lastCombatGoldReward,
                _lastCombatGlitchDelta,
                _lastCombatAffinityDelta,
                _lastCombatEnemyDefeated,
                _lastCombatComboDamage,
                _currentFloor,
                _stairUnlocked,
                _runClear,
                _lastNpcReactionKey,
                CountOwnedItems());
        }

        public string NextDemoNodeId
        {
            get
            {
                return TryGetNextDemoStep(out var step) ? step.NodeId : string.Empty;
            }
        }

        public string NextDemoEncounterId
        {
            get
            {
                return TryGetNextDemoStep(out var step) ? step.EncounterId : string.Empty;
            }
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

            RecalculatePlayerStats();
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

            RecalculatePlayerStats();
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

            var added = _memoryFragmentRefs.Add(memoryFragmentRef);
            if (added)
            {
                _lastMemoryFragmentId = memoryFragmentRef;
                if (EncounterCatalog != null && EncounterCatalog.TryGetMemoryFragment(memoryFragmentRef, out var memoryFragment))
                {
                    _lastMemoryFragmentTitleKey = memoryFragment.TitleKey;
                    _lastMemoryFragmentBodyKey = memoryFragment.BodyKey;
                }
                else
                {
                    _lastMemoryFragmentTitleKey = string.Empty;
                    _lastMemoryFragmentBodyKey = string.Empty;
                }
            }

            return added;
        }

        public void RecordNpcReaction(string reactionKey)
        {
            SetNpcReaction(string.IsNullOrEmpty(reactionKey) ? "NPC_REACT_FALLBACK" : reactionKey);
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

        public void AttachDemoRunPath(IReadOnlyList<PrototypeDemoRunStep> demoRunPath)
        {
            _demoRunPath.Clear();
            _resolvedDemoSteps.Clear();

            if (demoRunPath == null)
            {
                return;
            }

            for (var i = 0; i < demoRunPath.Count; i++)
            {
                var step = demoRunPath[i];
                if (step != null && step.IsValid)
                {
                    _demoRunPath.Add(step);
                }
            }
        }

        public void AttachFloorRunPaths(IReadOnlyList<PrototypeFloorRunPath> floorRunPaths, IReadOnlyList<PrototypeDemoRunStep> fallbackFloorOnePath)
        {
            _floorRunPaths.Clear();
            _currentFloor = 1;
            _stairUnlocked = false;
            _runClear = false;

            if (floorRunPaths != null)
            {
                for (var i = 0; i < floorRunPaths.Count; i++)
                {
                    var path = floorRunPaths[i];
                    if (path != null && path.Floor > 0 && path.Steps != null && path.Steps.Count > 0)
                    {
                        _floorRunPaths.Add(path);
                    }
                }
            }

            AttachDemoRunPath(GetFloorRunPath(1, fallbackFloorOnePath));
        }

        public bool TryGetNextDemoStep(out PrototypeDemoRunStep step)
        {
            for (var i = 0; i < _demoRunPath.Count; i++)
            {
                var candidate = _demoRunPath[i];
                if (!_resolvedDemoSteps.Contains(BuildResolvedEncounterKey(candidate.NodeId, candidate.EncounterId)))
                {
                    step = candidate;
                    return true;
                }
            }

            step = null;
            return false;
        }

        public bool CanAdvanceToNextFloor => _stairUnlocked && HasFloorRunPath(_currentFloor + 1);

        public PrototypeNodeResolution ResolveNextFloor()
        {
            if (!CanAdvanceToNextFloor)
            {
                return new PrototypeNodeResolution("node.stair", string.Empty, "next floor unavailable", _runCompleted);
            }

            var completedFloor = _currentFloor;
            _currentFloor++;
            _stairUnlocked = false;
            _resolvedDemoSteps.Clear();
            AttachDemoRunPath(GetFloorRunPath(_currentFloor, null));
            SaveFloorReflection(completedFloor);
            SetNpcReaction("NPC_REACT_FLOOR_" + _currentFloor);
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.RoomEntered, RunId, "floor." + _currentFloor, string.Empty));
            return new PrototypeNodeResolution("node.stair", "floor." + _currentFloor, "floor " + _currentFloor + " entered", false);
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
                round = combat.ResolveRound(player, enemy, CombatAction.Attack);
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
            var encounterId = encounter == null ? string.Empty : encounter.Id;
            if (TryGetResolvedEncounterChoice(nodeId, encounterId, out var resolvedChoiceStableId))
            {
                return new PrototypeNodeResolution(nodeId, resolvedChoiceStableId, $"already resolved: {resolvedChoiceStableId}", _runCompleted);
            }

            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, encounterId, "run already completed", true);
            }

            var resolution = PrototypeEncounterRuntimeResolver.Resolve(this, encounter, choiceStableId, new DeterministicRunContext(RunId, 0), nodeId);
            if (resolution.Applied)
            {
                MarkEncounterChoiceResolved(nodeId, encounterId, resolution.ChoiceStableId);
            }

            NodesResolved++;
            UpdateDemoProgression(nodeId, encounterId, resolution.Applied);
            var payloadId = resolution.ChoiceStableId;
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, payloadId));
            return new PrototypeNodeResolution(nodeId, payloadId, BuildChoiceResolutionMessage(resolution.Message), _runCompleted);
        }

        public PrototypeNodeResolution ResolveEncounterChoice(DeterministicRunContext context, string nodeId, EncounterData encounter, string choiceStableId)
        {
            var encounterId = encounter == null ? string.Empty : encounter.Id;
            if (TryGetResolvedEncounterChoice(nodeId, encounterId, out var resolvedChoiceStableId))
            {
                return new PrototypeNodeResolution(nodeId, resolvedChoiceStableId, $"already resolved: {resolvedChoiceStableId}", _runCompleted);
            }

            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, encounterId, "run already completed", true);
            }

            var resolution = PrototypeEncounterRuntimeResolver.Resolve(this, encounter, choiceStableId, context, nodeId);
            if (resolution.Applied)
            {
                MarkEncounterChoiceResolved(nodeId, encounterId, resolution.ChoiceStableId);
            }

            NodesResolved++;
            UpdateDemoProgression(nodeId, encounterId, resolution.Applied);
            var payloadId = resolution.ChoiceStableId;
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, payloadId));
            return new PrototypeNodeResolution(nodeId, payloadId, BuildChoiceResolutionMessage(resolution.Message), _runCompleted);
        }

        private CombatantState _activeCombatPlayer;
        private CombatantState _activeCombatEnemy;
        private CombatController _activeCombatController;
        private EncounterCombatHandoffRuntimeData _activeCombatHandoff;
        private string _activeCombatNodeId;
        private string _activeCombatEncounterId;
        private System.Action<PrototypeRunState, EncounterPostCombatEffectRuntimeData[]> _activeCombatEffectApplier;

        public bool IsInCombat => _activeCombatPlayer != null && _activeCombatEnemy != null && !_activeCombatPlayer.IsDefeated && !_activeCombatEnemy.IsDefeated;
        public CombatantState ActiveCombatPlayer => _activeCombatPlayer;
        public CombatantState ActiveCombatEnemy => _activeCombatEnemy;

        public CombatRoundResult ResolveCombatRoundInteractive(CombatAction action)
        {
            if (!IsInCombat)
            {
                return new CombatRoundResult(0, 0, _activeCombatPlayer?.IsDefeated ?? true, _activeCombatEnemy?.IsDefeated ?? true);
            }

            var secondAction = action == CombatAction.Skill && HasPlayableSkill() ? (CombatAction?)CombatAction.Attack : null;
            var result = _activeCombatController.ResolveRound(_activeCombatPlayer, _activeCombatEnemy, action, secondAction);
            _combatRound++;
            _lastCombatComboDamage = result.ComboDamage;
            _lastCombatRoundResult =
                "round " + _combatRound +
                " | action " + action +
                " | playerDamage " + result.PlayerDamage +
                " | enemyDamage " + result.EnemyDamage +
                (result.ComboDamage > 0 ? " | combo " + result.ComboDamage : string.Empty);

            if (action == CombatAction.Defend)
            {
                var defendReduce = CombatAbilityModifiers.From(Abilities.Abilities, _activeSynergies, BuildOwnedItemData()).DefendDamageReduce;
                if (defendReduce > 0 && result.EnemyDamage > 0)
                {
                    _activeCombatPlayer.RestoreHp(defendReduce);
                    _lastCombatRoundResult += " | item guard " + defendReduce;
                }
            }

            if (result.IsComplete)
            {
                FinalizeCombat();
            }

            return result;
        }

        private void FinalizeCombat()
        {
            if (_activeCombatEnemy.IsDefeated)
            {
                BattlesWon++;
            }

            _playerHp = _activeCombatPlayer.Hp;
            var resultId = _activeCombatEnemy.IsDefeated ? "victory" : "defeat";
            _lastCombatResultId = resultId;
            _lastCombatEnemyDefeated = _activeCombatEnemy.IsDefeated;
            CapturePostCombatDeltas(_activeCombatEnemy.IsDefeated ? _activeCombatHandoff.onVictoryEffects : _activeCombatHandoff.onDefeatEffects);
            
            _activeCombatEffectApplier?.Invoke(this, _activeCombatEnemy.IsDefeated ? _activeCombatHandoff.onVictoryEffects : _activeCombatHandoff.onDefeatEffects);
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatCompleted, RunId, _activeCombatNodeId, resultId));

            if (_activeCombatPlayer.IsDefeated)
            {
                if (TryApplyRecallAnchor())
                {
                    _lastCombatResultId = "recall";
                    _lastCombatRoundResult += " | recall anchor";
                    SetNpcReaction("NPC_REACT_RECALL_ANCHOR");
                    return;
                }

                ApplyNpcTrigger("battle.defeat");
                SetNpcReaction("NPC_REACT_COMBAT_DEFEAT");
                CompleteRun("defeat");
            }
            else if (_activeCombatEnemy.IsDefeated)
            {
                ApplyNpcTrigger("battle.victory");
                SetNpcReaction("NPC_REACT_COMBAT_VICTORY");
            }

            // Clear active combat if complete
            if (resultId != "started")
            {
                // We keep the states for one turn so UI can show the final result, 
                // but IsInCombat will return false because someone is defeated.
            }

            if (!AutoResolveCombat)
            {
                UpdateDemoProgression(_activeCombatNodeId, _activeCombatEncounterId, true);
            }
        }

        public bool AutoResolveCombat { get; set; }

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
            var combatStartRestore = CombatAbilityModifiers.From(Abilities.Abilities, _activeSynergies, BuildOwnedItemData()).CombatStartHpRestore;
            if (combatStartRestore > 0)
            {
                player.RestoreHp(combatStartRestore);
            }

            var enemyId = handoff.enemyRefs != null && handoff.enemyRefs.Length > 0 && !string.IsNullOrEmpty(handoff.enemyRefs[0])
                ? handoff.enemyRefs[0]
                : "enemy.placeholder";
            var enemy = EncounterCatalog != null && EncounterCatalog.TryGetEnemy(enemyId, out var enemyData)
                ? CreateEnemyState(enemyData)
                : new CombatantState(enemyId, FallbackEnemyHp, FallbackEnemyAttack);
            var combatId = string.IsNullOrEmpty(handoff.stableId) ? encounter == null ? nodeId : encounter.Id : handoff.stableId;
            var combat = new CombatController(context, string.IsNullOrEmpty(handoff.seedKey) ? combatId : handoff.seedKey);

            _lastCombatId = combatId;
            _lastCombatEnemyId = enemy.Id;
            _lastCombatResultId = "started";
            _lastCombatRoundResult = "round 0 | ready";
            _combatRound = 0;
            _lastCombatGoldReward = 0;
            _lastCombatGlitchDelta = 0;
            _lastCombatAffinityDelta = 0;
            _lastCombatEnemyDefeated = false;
            _lastCombatComboDamage = 0;
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatStarted, RunId, nodeId, enemy.Id));

            // Initialize interactive state
            _activeCombatPlayer = player;
            _activeCombatEnemy = enemy;
            _activeCombatController = combat;
            _activeCombatHandoff = handoff;
            _activeCombatNodeId = nodeId;
            _activeCombatEncounterId = encounter == null ? string.Empty : encounter.Id;
            _activeCombatEffectApplier = applyEffects;

            if (AutoResolveCombat)
            {
                var rounds = 0;
                while (IsInCombat && rounds < 12)
                {
                    ResolveCombatRoundInteractive(CombatAction.Attack);
                    rounds++;
                }
                return new PrototypeCombatHandoffResolution(true, _lastCombatResultId, combatId, enemy.Id);
            }

            return new PrototypeCombatHandoffResolution(true, "started", combatId, enemy.Id);
        }

        private void CapturePostCombatDeltas(EncounterPostCombatEffectRuntimeData[] effects)
        {
            _lastCombatGoldReward = 0;
            _lastCombatGlitchDelta = 0;
            _lastCombatAffinityDelta = 0;

            if (effects == null)
            {
                return;
            }

            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i];
                if (effect == null)
                {
                    continue;
                }

                switch (effect.kind)
                {
                    case "ModifyGold":
                        _lastCombatGoldReward += effect.amount;
                        break;
                    case "ModifyGlitchLevel":
                        _lastCombatGlitchDelta += effect.amount;
                        break;
                    case "ModifyAffinity":
                        _lastCombatAffinityDelta += effect.amount;
                        break;
                }
            }
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

        private int CountOwnedItems()
        {
            var total = 0;
            foreach (var pair in _items)
            {
                total += System.Math.Max(0, pair.Value);
            }

            return total;
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
            var modifiers = CombatAbilityModifiers.From(Abilities.Abilities, _activeSynergies, BuildOwnedItemData());
            _playerMaxHp = System.Math.Max(1, BasePlayerMaxHp + modifiers.PlayerMaxHpBonus);
            _playerAttack = System.Math.Max(0, BasePlayerAttack + modifiers.PlayerAttackBonus);

            if (_playerMaxHp > previousMaxHp)
            {
                _playerHp += _playerMaxHp - previousMaxHp;
            }

            _playerHp = System.Math.Max(0, System.Math.Min(_playerHp, _playerMaxHp));
        }

        private List<HwigiTower.Items.ItemData> BuildOwnedItemData()
        {
            var result = new List<HwigiTower.Items.ItemData>();
            if (EncounterCatalog == null)
            {
                return result;
            }

            foreach (var pair in _items)
            {
                if (pair.Value <= 0 || !EncounterCatalog.TryGetItem(pair.Key, out var item) || item == null)
                {
                    continue;
                }

                for (var i = 0; i < pair.Value; i++)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private bool HasPlayableSkill()
        {
            return Abilities.Abilities.Count > 0 || _abilityRefs.Count > 0;
        }

        private bool TryApplyRecallAnchor()
        {
            if (!HasAbilityRef("ABILITY_RECALL_ANCHOR") || HasFlag("FLAG_RECALL_ANCHOR_USED") || _activeCombatPlayer == null)
            {
                return false;
            }

            SetFlag("FLAG_RECALL_ANCHOR_USED", true);
            _activeCombatPlayer.RestoreHp(6);
            _playerHp = _activeCombatPlayer.Hp;
            ModifyGlitchLevel(3);
            ModifyAffinity(1);
            return true;
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

        private void UpdateDemoProgression(string nodeId, string encounterId, bool applied)
        {
            if (!applied || _demoRunPath.Count == 0)
            {
                return;
            }

            var key = BuildResolvedEncounterKey(nodeId, encounterId);
            for (var i = 0; i < _demoRunPath.Count; i++)
            {
                var step = _demoRunPath[i];
                if (BuildResolvedEncounterKey(step.NodeId, step.EncounterId) == key)
                {
                    _resolvedDemoSteps.Add(key);
                    break;
                }
            }

            if (FloorComplete)
            {
                if (HasFloorRunPath(_currentFloor + 1))
                {
                    _stairUnlocked = true;
                    SetNpcReaction("NPC_REACT_STAIR_UNLOCKED");
                }
                else
                {
                    _runClear = true;
                    CompleteRun("run.clear");
                }
            }
        }

        private string BuildChoiceResolutionMessage(string baseMessage)
        {
            if (_runClear)
            {
                return string.IsNullOrEmpty(baseMessage) ? "run.clear" : baseMessage + " | run.clear";
            }

            if (_stairUnlocked)
            {
                return string.IsNullOrEmpty(baseMessage) ? "stair unlocked" : baseMessage + " | stair unlocked";
            }

            if (TryGetNextDemoStep(out var next))
            {
                var nextMessage = "next: " + next.NodeId + "/" + next.EncounterId;
                return string.IsNullOrEmpty(baseMessage) ? nextMessage : baseMessage + " | " + nextMessage;
            }

            return baseMessage ?? string.Empty;
        }

        private void ApplyNpcTrigger(string triggerId)
        {
            NpcStateMachine?.TryApply(triggerId);
        }

        private void SetNpcReaction(string reactionKey)
        {
            _lastNpcReactionKey = reactionKey ?? string.Empty;
        }

        private IReadOnlyList<PrototypeDemoRunStep> GetFloorRunPath(int floor, IReadOnlyList<PrototypeDemoRunStep> fallbackFloorOnePath)
        {
            for (var i = 0; i < _floorRunPaths.Count; i++)
            {
                if (_floorRunPaths[i] != null && _floorRunPaths[i].Floor == floor)
                {
                    return _floorRunPaths[i].Steps;
                }
            }

            return floor == 1 ? fallbackFloorOnePath : null;
        }

        private bool HasFloorRunPath(int floor)
        {
            for (var i = 0; i < _floorRunPaths.Count; i++)
            {
                if (_floorRunPaths[i] != null && _floorRunPaths[i].Floor == floor && _floorRunPaths[i].Steps != null && _floorRunPaths[i].Steps.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void SaveFloorReflection(int completedFloor)
        {
            var summary = "floor=" + completedFloor + ";nodes=" + NodesResolved + ";battles=" + BattlesWon;
            _reflectionPipeline.TrySaveReflection(RunId + ".floor." + completedFloor, summary, out _);
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
