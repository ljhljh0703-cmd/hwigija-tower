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
        private readonly List<PrototypeFloorMapNode> _floorMapNodes = new List<PrototypeFloorMapNode>();
        private readonly HashSet<string> _resolvedDemoSteps = new HashSet<string>();
        private bool _runCompleted;
        private bool _runClear;
        private bool _runFailed;
        private bool _restartReady;
        private bool _endingRest;
        private bool _endingContinue;
        private string _endingChoiceId = string.Empty;
        private string _selectedMapNodeId = string.Empty;
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
        private int _crackedJarBuffCombats;
        private bool _trainingBuffActive;
        private string _pendingRestNodeId = string.Empty;
        private string _selectedRestActionId = string.Empty;
        private string _lastRestUtterance = string.Empty;
        private string _lastMataiosResponse = string.Empty;
        private string _lastNpcReactionKey = string.Empty;

        public PrototypeRunState(string runId, GameFlowEventBus eventBus)
            : this(runId, eventBus, null)
        {
        }

        public PrototypeRunState(string runId, GameFlowEventBus eventBus, LLMRuntimeSettings llmRuntimeSettings)
            : this(runId, eventBus, llmRuntimeSettings, null)
        {
        }

        public PrototypeRunState(string runId, GameFlowEventBus eventBus, LLMRuntimeSettings llmRuntimeSettings, INPCMemoryRepo memoryRepo)
        {
            RunId = runId ?? string.Empty;
            _eventBus = eventBus;
            MemoryRepo = memoryRepo ?? new InMemoryNpcMemoryRepo();
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
        public bool FloorComplete => _floorMapNodes.Count > 0 ? IsFloorMapComplete() : _demoRunPath.Count > 0 && _resolvedDemoSteps.Count >= _demoRunPath.Count;
        public bool StairUnlocked => _stairUnlocked;
        public bool RunClear => _runClear;
        public bool RunFailed => _runFailed;
        public bool RestartReady => _restartReady;
        public bool EndingChoicePending => _runClear && !_endingRest && !_endingContinue;
        public bool EndingRest => _endingRest;
        public bool EndingContinue => _endingContinue;
        public string EndingChoiceId => _endingChoiceId;
        public bool BossGateUnlocked => IsCurrentBossGateUnlocked();
        public string RunStatus => _endingRest ? "ending.rest" : _endingContinue ? "ending.continue" : _runFailed ? "run.failed" : _runClear ? "run.clear" : _restartReady ? "run.restartReady" : "run.active";
        public string DemoStatus => _endingRest ? "ending.rest" : _endingContinue ? "ending.continue" : _runFailed ? "run.failed" : _runClear ? "run.clear" : _stairUnlocked ? "stair.unlocked" : _demoRunPath.Count == 0 ? "demo.unconfigured" : "demo.active";
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
        public bool TrainingBuffActive => _trainingBuffActive;
        public string PendingRestNodeId => _pendingRestNodeId;
        public string SelectedRestActionId => _selectedRestActionId;
        public string LastRestUtterance => _lastRestUtterance;
        public string LastMataiosResponse => _lastMataiosResponse;

        public PrototypeRunSaveData CreateSaveData()
        {
            var completedMapNodeIds = new List<string>();
            var skippedMapNodeIds = new List<string>();
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (node == null)
                {
                    continue;
                }

                if (node.Completed)
                {
                    completedMapNodeIds.Add(node.MapNodeId);
                }
                else if (node.Skipped)
                {
                    skippedMapNodeIds.Add(node.MapNodeId);
                }
            }

            var items = new List<PrototypeRunSaveItemEntry>();
            foreach (var pair in _items)
            {
                if (!string.IsNullOrEmpty(pair.Key) && pair.Value > 0)
                {
                    items.Add(new PrototypeRunSaveItemEntry { itemRef = pair.Key, count = pair.Value });
                }
            }

            var resolvedChoices = new List<PrototypeRunSaveResolvedChoice>();
            foreach (var pair in _resolvedEncounterChoices)
            {
                resolvedChoices.Add(new PrototypeRunSaveResolvedChoice { key = pair.Key, choiceStableId = pair.Value });
            }

            return new PrototypeRunSaveData
            {
                runId = RunId,
                currentFloor = _currentFloor,
                playerHp = _activeCombatPlayer?.Hp ?? _playerHp,
                playerMaxHp = _playerMaxHp,
                mental = _mental,
                gold = _gold,
                glitchLevel = _glitchLevel,
                affinity = _affinity,
                nodesResolved = NodesResolved,
                battlesWon = BattlesWon,
                runCompleted = _runCompleted,
                runClear = _runClear,
                runFailed = _runFailed,
                restartReady = _restartReady,
                endingRest = _endingRest,
                endingContinue = _endingContinue,
                endingChoiceId = _endingChoiceId,
                stairUnlocked = _stairUnlocked,
                trainingBuffActive = _trainingBuffActive,
                selectedMapNodeId = _selectedMapNodeId,
                flags = ToArray(_flags),
                items = items.ToArray(),
                abilityRefs = ToArray(_abilityRefs),
                rewardBundleRefs = ToArray(_rewardBundleRefs),
                memoryFragmentRefs = ToArray(_memoryFragmentRefs),
                resolvedChoices = resolvedChoices.ToArray(),
                resolvedDemoStepKeys = ToArray(_resolvedDemoSteps),
                completedMapNodeIds = completedMapNodeIds.ToArray(),
                skippedMapNodeIds = skippedMapNodeIds.ToArray()
            };
        }

        public void RestoreFromSaveData(
            PrototypeRunSaveData data,
            IReadOnlyList<PrototypeFloorRunPath> floorRunPaths,
            IReadOnlyList<PrototypeDemoRunStep> fallbackFloorOnePath)
        {
            if (data == null || string.IsNullOrEmpty(data.runId))
            {
                return;
            }

            _floorRunPaths.Clear();
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

            _currentFloor = data.currentFloor < 1 ? 1 : data.currentFloor;
            AttachDemoRunPath(GetFloorRunPath(_currentFloor, fallbackFloorOnePath));

            _flags.Clear();
            AddRange(_flags, data.flags);
            _items.Clear();
            if (data.items != null)
            {
                for (var i = 0; i < data.items.Length; i++)
                {
                    var item = data.items[i];
                    if (item != null && !string.IsNullOrEmpty(item.itemRef) && item.count > 0)
                    {
                        _items[item.itemRef] = item.count;
                    }
                }
            }

            _abilityRefs.Clear();
            Abilities.Clear();
            AddRange(_abilityRefs, data.abilityRefs);
            if (data.abilityRefs != null && EncounterCatalog != null)
            {
                for (var i = 0; i < data.abilityRefs.Length; i++)
                {
                    if (EncounterCatalog.TryGetAbility(data.abilityRefs[i], out var ability) && ability != null)
                    {
                        Abilities.Add(ability);
                    }
                }
            }

            _rewardBundleRefs.Clear();
            AddRange(_rewardBundleRefs, data.rewardBundleRefs);
            _memoryFragmentRefs.Clear();
            AddRange(_memoryFragmentRefs, data.memoryFragmentRefs);
            _lastMemoryFragmentId = data.memoryFragmentRefs != null && data.memoryFragmentRefs.Length > 0 ? data.memoryFragmentRefs[data.memoryFragmentRefs.Length - 1] : string.Empty;
            if (!string.IsNullOrEmpty(_lastMemoryFragmentId) && EncounterCatalog != null && EncounterCatalog.TryGetMemoryFragment(_lastMemoryFragmentId, out var memoryFragment))
            {
                _lastMemoryFragmentTitleKey = memoryFragment.TitleKey;
                _lastMemoryFragmentBodyKey = memoryFragment.BodyKey;
            }
            else
            {
                _lastMemoryFragmentTitleKey = string.Empty;
                _lastMemoryFragmentBodyKey = string.Empty;
            }
            _resolvedEncounterChoices.Clear();
            if (data.resolvedChoices != null)
            {
                for (var i = 0; i < data.resolvedChoices.Length; i++)
                {
                    var entry = data.resolvedChoices[i];
                    if (entry != null && !string.IsNullOrEmpty(entry.key))
                    {
                        _resolvedEncounterChoices[entry.key] = entry.choiceStableId ?? string.Empty;
                    }
                }
            }

            _resolvedDemoSteps.Clear();
            AddRange(_resolvedDemoSteps, data.resolvedDemoStepKeys);
            ApplySavedMapNodeState(data.completedMapNodeIds, data.skippedMapNodeIds);

            _stairUnlocked = data.stairUnlocked;
            _runCompleted = data.runCompleted;
            _runClear = data.runClear;
            _runFailed = data.runFailed;
            _restartReady = data.restartReady;
            _endingRest = data.endingRest;
            _endingContinue = data.endingContinue;
            _endingChoiceId = data.endingChoiceId ?? string.Empty;
            _selectedMapNodeId = IsSavedMapNodeSelectable(data.selectedMapNodeId) ? data.selectedMapNodeId : string.Empty;
            NodesResolved = data.nodesResolved < 0 ? 0 : data.nodesResolved;
            BattlesWon = data.battlesWon < 0 ? 0 : data.battlesWon;
            _mental = Clamp(data.mental, MinMental, MaxMental);
            _gold = Clamp(data.gold, MinGold, MaxGold);
            _glitchLevel = Clamp(data.glitchLevel, MinGlitchLevel, MaxGlitchLevel);
            _affinity = Clamp(data.affinity, MinAffinity, MaxAffinity);
            _trainingBuffActive = data.trainingBuffActive;
            if (_trainingBuffActive)
            {
                _flags.Add("MATAIOS_TRAINING_BUFF_ACTIVE");
            }

            RecalculatePlayerStats();
            _playerMaxHp = data.playerMaxHp <= 0 ? _playerMaxHp : data.playerMaxHp;
            _playerHp = Clamp(data.playerHp, 0, _playerMaxHp);
            _lastCombatResultId = string.Empty;
            _lastCombatRoundResult = "loaded save";
            _lastNpcReactionKey = "NPC_REACT_SAVE_LOADED";
        }

        private void ApplySavedMapNodeState(string[] completedMapNodeIds, string[] skippedMapNodeIds)
        {
            if (completedMapNodeIds != null)
            {
                for (var i = 0; i < completedMapNodeIds.Length; i++)
                {
                    var node = FindMapNode(completedMapNodeIds[i]);
                    if (node != null)
                    {
                        node.Completed = true;
                    }
                }
            }

            if (skippedMapNodeIds != null)
            {
                for (var i = 0; i < skippedMapNodeIds.Length; i++)
                {
                    var node = FindMapNode(skippedMapNodeIds[i]);
                    if (node != null && !node.Completed)
                    {
                        node.Skipped = true;
                    }
                }
            }
        }

        private bool IsSavedMapNodeSelectable(string mapNodeId)
        {
            var node = FindMapNode(mapNodeId);
            return node != null && !node.Completed && !node.Skipped;
        }

        private PrototypeFloorMapNode FindMapNode(string mapNodeId)
        {
            if (string.IsNullOrEmpty(mapNodeId))
            {
                return null;
            }

            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (node != null && node.MapNodeId == mapNodeId)
                {
                    return node;
                }
            }

            return null;
        }

        private static void AddRange(HashSet<string> destination, string[] source)
        {
            if (destination == null || source == null)
            {
                return;
            }

            for (var i = 0; i < source.Length; i++)
            {
                if (!string.IsNullOrEmpty(source[i]))
                {
                    destination.Add(source[i]);
                }
            }
        }

        private static string[] ToArray(HashSet<string> values)
        {
            if (values == null || values.Count == 0)
            {
                return System.Array.Empty<string>();
            }

            var result = new string[values.Count];
            values.CopyTo(result);
            System.Array.Sort(result, System.StringComparer.Ordinal);
            return result;
        }

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
                _runFailed,
                _restartReady,
                BossGateUnlocked,
                RunStatus,
                _lastNpcReactionKey,
                CountOwnedItems(),
                EndingChoicePending,
                _endingRest,
                _endingContinue,
                _endingChoiceId,
                BuildMapNodeViews(),
                _selectedMapNodeId);
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
            if (string.IsNullOrEmpty(abilityRef) || _abilityRefs.Contains(abilityRef))
            {
                return false;
            }

            if (EncounterCatalog != null)
            {
                if (!EncounterCatalog.TryGetAbility(abilityRef, out var ability) || ability == null)
                {
                    return false;
                }

                _abilityRefs.Add(abilityRef);
                Abilities.Add(ability);
            }
            else
            {
                _abilityRefs.Add(abilityRef);
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
            _floorMapNodes.Clear();
            _selectedMapNodeId = string.Empty;

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

            PrototypeFloorMapBuilder.Build(_currentFloor, _demoRunPath, _floorMapNodes);
        }

        public void AttachFloorRunPaths(IReadOnlyList<PrototypeFloorRunPath> floorRunPaths, IReadOnlyList<PrototypeDemoRunStep> fallbackFloorOnePath)
        {
            _floorRunPaths.Clear();
            _currentFloor = 1;
            _stairUnlocked = false;
            _runClear = false;
            _runFailed = false;
            _restartReady = false;

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
            if (_floorMapNodes.Count > 0)
            {
                if (TryGetSelectedMapNode(out var selectedNode))
                {
                    step = selectedNode.Step;
                    return step != null && step.IsValid;
                }

                if (TryGetFirstSelectableMapNode(out var selectableNode))
                {
                    step = selectableNode.Step;
                    return step != null && step.IsValid;
                }
            }

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

        public PrototypeFloorMapNodeView[] GetSelectableMapNodeViews()
        {
            var views = new List<PrototypeFloorMapNodeView>();
            var activeLayer = GetActiveMapLayer();
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (IsMapNodeSelectable(node, activeLayer))
                {
                    views.Add(ToMapNodeView(node, true, false));
                }
            }

            return views.ToArray();
        }

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public PrototypeFloorMapNodeView[] GetQaFloorMapNodeViews()
        {
            var views = new List<PrototypeFloorMapNodeView>();
            var activeLayer = GetActiveMapLayer();
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (node == null)
                {
                    continue;
                }

                var selectable = IsMapNodeSelectable(node, activeLayer);
                var locked = !node.Completed && !selectable;
                views.Add(ToMapNodeView(node, selectable, locked));
            }

            return views.ToArray();
        }
#endif

        public bool TrySelectMapNode(string mapNodeId, out PrototypeDemoRunStep step)
        {
            step = null;
            var activeLayer = GetActiveMapLayer();
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (node != null && node.MapNodeId == mapNodeId && IsMapNodeSelectable(node, activeLayer))
                {
                    _selectedMapNodeId = node.MapNodeId;
                    step = node.Step;
                    return step != null && step.IsValid;
                }
            }

            return false;
        }

        public bool CanAdvanceToNextFloor => _stairUnlocked && HasFloorRunPath(_currentFloor + 1);

        public PrototypeNodeResolution ResolveNextFloor()
        {
            if (_runCompleted || !CanAdvanceToNextFloor)
            {
                return new PrototypeNodeResolution("node.stair", string.Empty, "next floor unavailable", _runCompleted);
            }

            var completedFloor = _currentFloor;
            _currentFloor++;
            _stairUnlocked = false;
            _resolvedDemoSteps.Clear();
            _selectedMapNodeId = string.Empty;
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
                FailRun();
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
            ModifyGlitchLevel(-3);
            var recall = _reflectionPipeline.LoadRecallPrompt(RunId, 3);
            ApplyNpcTrigger("rest.recall");
            SetNpcReaction("NPC_REACT_REST");
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, "rest"));
            return new PrototypeNodeResolution(nodeId, "recall", "rest complete: HP restored | Glitch -3 | " + recall, false);
        }

        public PrototypeEncounterChoiceResolution ResolveJarRoomChoice(DeterministicRunContext context, string nodeId, string choiceStableId)
        {
            if (_runCompleted)
            {
                return new PrototypeEncounterChoiceResolution(choiceStableId, false, "run already completed");
            }

            if (choiceStableId == "CHOICE_EVT_F01_JAR_PATTERNED")
            {
                var random = context.CreateRandom("EVT_F01_JAR_ROOM.patterned");
                if (random.Range(0, 100) < 80)
                {
                    ModifyGold(8);
                    SetNpcReaction("NPC_REACT_EVENT_JAR_GOLD");
                    return new PrototypeEncounterChoiceResolution(choiceStableId, true, "jar outcome: Gold +8");
                }

                ResolveBattle(context, nodeId, "EVT_F01_JAR_ROOM_ELITE", null, null);
                SetNpcReaction("NPC_REACT_EVENT_JAR_ELITE");
                return new PrototypeEncounterChoiceResolution(choiceStableId, true, "jar outcome: elite combat");
            }

            if (choiceStableId == "CHOICE_EVT_F01_JAR_PLAIN")
            {
                _playerHp = System.Math.Min(_playerMaxHp, _playerHp + 5);
                ModifyMental(5);
                SetNpcReaction("NPC_REACT_EVENT_JAR_REST");
                return new PrototypeEncounterChoiceResolution(choiceStableId, true, "jar outcome: HP +5 | Mental +5");
            }

            if (choiceStableId == "CHOICE_EVT_F01_JAR_CRACKED")
            {
                _crackedJarBuffCombats = 3;
                SetFlag("FLAG_CRACKED_JAR_DAMAGE_BUFF", true);
                SetNpcReaction("NPC_REACT_EVENT_JAR_BUFF");
                return new PrototypeEncounterChoiceResolution(choiceStableId, true, "jar outcome: next 3 combat damage buff");
            }

            return new PrototypeEncounterChoiceResolution(choiceStableId, false, "jar outcome unavailable");
        }

        public PrototypeEncounterChoiceResolution ResolveRestChoice(string choiceStableId)
        {
            if (_runCompleted)
            {
                return new PrototypeEncounterChoiceResolution(choiceStableId, false, "run already completed");
            }

            _playerHp = System.Math.Min(_playerMaxHp, _playerHp + 6);
            ModifyGlitchLevel(-3);
            var recall = _reflectionPipeline.LoadRecallPrompt(RunId, 3);
            ApplyNpcTrigger("rest.recall");
            SetNpcReaction("NPC_REACT_REST");
            return new PrototypeEncounterChoiceResolution(choiceStableId, true, "rest complete: HP restored | Glitch -3 | " + recall);
        }

        public PrototypeNodeResolution ResolveRestInteraction(string nodeId, string encounterId, string actionId, string utterance)
        {
            var resolvedNodeId = GetResolvedChoiceNodeId(nodeId);
            if (TryGetResolvedEncounterChoice(resolvedNodeId, encounterId, out var resolvedActionId))
            {
                return new PrototypeNodeResolution(nodeId, resolvedActionId, "already resolved: " + resolvedActionId, _runCompleted);
            }

            if (_runCompleted)
            {
                return new PrototypeNodeResolution(nodeId, encounterId, "run already completed", true);
            }

            var normalizedActionId = NormalizeRestActionId(actionId);
            if (string.IsNullOrEmpty(normalizedActionId))
            {
                return new PrototypeNodeResolution(nodeId, encounterId, "rest action unavailable", false);
            }

            var normalizedUtterance = utterance == null ? string.Empty : utterance.Trim();
            if (RestActionRequiresInput(normalizedActionId) && string.IsNullOrEmpty(normalizedUtterance))
            {
                _pendingRestNodeId = nodeId ?? string.Empty;
                _selectedRestActionId = normalizedActionId;
                return new PrototypeNodeResolution(nodeId, normalizedActionId, "input required", false);
            }

            _pendingRestNodeId = nodeId ?? string.Empty;
            _selectedRestActionId = normalizedActionId;
            _lastRestUtterance = normalizedUtterance;
            var promptProfileId = normalizedActionId;
            var prompt = "profile=" + promptProfileId + "\naction=" + normalizedActionId + "\nplayer=" + normalizedUtterance;
            var request = new LLMRequest(RunId, prompt, promptProfileId);
            LLMProvider.TryComplete(request, out _);
            _lastMataiosResponse = BuildTemporaryRestResponse(normalizedActionId);

            var message = ApplyRestInteractionEffect(normalizedActionId);
            MarkEncounterChoiceResolved(resolvedNodeId, encounterId, normalizedActionId);
            NodesResolved++;
            UpdateDemoProgression(nodeId, encounterId, true);
            MemoryRepo.SaveReflection(new RunReflection(RunId + ".rest." + NodesResolved, request.CacheKey.PromptHash, BuildRestReflectionSummary(normalizedActionId, normalizedUtterance)));
            SetNpcReaction("NPC_REACT_REST");
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, normalizedActionId));
            return new PrototypeNodeResolution(nodeId, normalizedActionId, message + " | Mataios response ready", _runCompleted);
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
                CompleteRun("remnant", true);
            }

            return new PrototypeNodeResolution(nodeId, "run.completed", "run completed: reflection saved", true);
        }

        public PrototypeNodeResolution ResolveEndingChoice(string choiceStableId)
        {
            if (!_runClear)
            {
                return new PrototypeNodeResolution("ending.choice", string.Empty, "ending unavailable", _runCompleted);
            }

            if (!string.IsNullOrEmpty(_endingChoiceId))
            {
                return new PrototypeNodeResolution("ending.choice", _endingChoiceId, "ending already resolved: " + _endingChoiceId, true);
            }

            if (choiceStableId == "PLACEHOLDER_ENDING_REST")
            {
                _endingChoiceId = choiceStableId;
                _endingRest = true;
                _endingContinue = false;
                _restartReady = false;
                SetNpcReaction("NPC_REACT_ENDING_REST");
                return new PrototypeNodeResolution("ending.choice", choiceStableId, "ending.rest | PLACEHOLDER_ENDING_REST", true);
            }

            if (choiceStableId == "PLACEHOLDER_ENDING_CONTINUE")
            {
                _endingChoiceId = choiceStableId;
                _endingContinue = true;
                _endingRest = false;
                _restartReady = true;
                SetNpcReaction("NPC_REACT_ENDING_CONTINUE");
                return new PrototypeNodeResolution("ending.choice", choiceStableId, "ending.continue | PLACEHOLDER_ENDING_CONTINUE | run.restartReady", true);
            }

            return new PrototypeNodeResolution("ending.choice", string.Empty, "ending unavailable", _runCompleted);
        }

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public void OpenQaEndingChoice()
        {
            _runCompleted = true;
            _runClear = true;
            _runFailed = false;
            _restartReady = false;
            _endingRest = false;
            _endingContinue = false;
            _endingChoiceId = string.Empty;
            SetNpcReaction("NPC_REACT_RUN_CLEAR");
        }
#endif

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
            var resolvedNodeId = GetResolvedChoiceNodeId(nodeId);
            if (TryGetResolvedEncounterChoice(resolvedNodeId, encounterId, out var resolvedChoiceStableId))
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
                MarkEncounterChoiceResolved(resolvedNodeId, encounterId, resolution.ChoiceStableId);
            }

            NodesResolved++;
            UpdateDemoProgression(nodeId, encounterId, ShouldAdvanceDemoProgression(encounter, choiceStableId, resolution.Applied));
            var payloadId = resolution.ChoiceStableId;
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.EncounterCompleted, RunId, nodeId, payloadId));
            return new PrototypeNodeResolution(nodeId, payloadId, BuildChoiceResolutionMessage(resolution.Message), _runCompleted);
        }

        public PrototypeNodeResolution ResolveEncounterChoice(DeterministicRunContext context, string nodeId, EncounterData encounter, string choiceStableId)
        {
            var encounterId = encounter == null ? string.Empty : encounter.Id;
            var resolvedNodeId = GetResolvedChoiceNodeId(nodeId);
            if (TryGetResolvedEncounterChoice(resolvedNodeId, encounterId, out var resolvedChoiceStableId))
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
                MarkEncounterChoiceResolved(resolvedNodeId, encounterId, resolution.ChoiceStableId);
            }

            NodesResolved++;
            UpdateDemoProgression(nodeId, encounterId, ShouldAdvanceDemoProgression(encounter, choiceStableId, resolution.Applied));
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

            if (action == CombatAction.Skill && !HasPlayableSkill())
            {
                _lastCombatRoundResult = "round " + _combatRound + " | action Skill | skill unavailable | scout required";
                return new CombatRoundResult(0, 0, false, false);
            }

            var enemyHpBefore = _activeCombatEnemy.Hp;
            var playerHpBefore = _activeCombatPlayer.Hp;
            var secondAction = action == CombatAction.Skill && HasPlayableSkill() ? (CombatAction?)CombatAction.Attack : null;
            var result = _activeCombatController.ResolveRound(_activeCombatPlayer, _activeCombatEnemy, action, secondAction);
            _combatRound++;
            var crackedJarBonus = 0;
            if (_crackedJarBuffCombats > 0 && result.PlayerDamage > 0 && !_activeCombatEnemy.IsDefeated)
            {
                crackedJarBonus = 2;
                _activeCombatEnemy.ApplyDamage(crackedJarBonus);
                _crackedJarBuffCombats--;
                if (_crackedJarBuffCombats <= 0)
                {
                    SetFlag("FLAG_CRACKED_JAR_DAMAGE_BUFF", false);
                }
            }

            var trainingBonus = 0;
            if (_trainingBuffActive && result.PlayerDamage > 0)
            {
                trainingBonus = 1;
                _activeCombatEnemy.ApplyDamage(trainingBonus);
                _trainingBuffActive = false;
                SetFlag("MATAIOS_TRAINING_BUFF_ACTIVE", false);
            }

            _lastCombatComboDamage = result.ComboDamage;
            _lastCombatRoundResult =
                "round " + _combatRound +
                " | action " + action +
                " | playerDamage " + result.PlayerDamage +
                " | enemyDamage " + result.EnemyDamage +
                " | enemyHp " + enemyHpBefore + "->" + _activeCombatEnemy.Hp +
                " | playerHp " + playerHpBefore + "->" + _activeCombatPlayer.Hp +
                (crackedJarBonus > 0 ? " | cracked jar +" + crackedJarBonus : string.Empty) +
                (trainingBonus > 0 ? " | training +" + trainingBonus : string.Empty) +
                (result.ComboDamage > 0 ? " | combo " + result.ComboDamage : string.Empty);

            if (action == CombatAction.Skill)
            {
                _lastCombatRoundResult += " | scout skill";
            }

            if (action == CombatAction.Defend)
            {
                _lastCombatRoundResult += " | defended";
            }

            if (HasAbilityRef("ABILITY_RECALL_ANCHOR") && !HasFlag("FLAG_RECALL_ANCHOR_USED"))
            {
                _lastCombatRoundResult += " | recall ready";
            }

            if (action == CombatAction.Defend)
            {
                var defendReduce = CombatAbilityModifiers.From(Abilities.Abilities, _activeSynergies, BuildOwnedItemData()).DefendDamageReduce;
                if (defendReduce > 0 && result.EnemyDamage > 0)
                {
                    _activeCombatPlayer.RestoreHp(defendReduce);
                    _lastCombatRoundResult += " | item guard " + defendReduce;
                }
            }

            if (result.IsComplete || _activeCombatEnemy.IsDefeated || _activeCombatPlayer.IsDefeated)
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

            var playerDefeated = _activeCombatPlayer.IsDefeated;
            var enemyDefeated = _activeCombatEnemy.IsDefeated;
            if (playerDefeated && !enemyDefeated && TryApplyRecallAnchor())
            {
                _lastCombatResultId = "recall";
                _lastCombatRoundResult += " | recall anchor";
                _lastCombatEnemyDefeated = false;
                SetNpcReaction("NPC_REACT_RECALL_ANCHOR");
                return;
            }

            _playerHp = _activeCombatPlayer.Hp;
            var resultId = enemyDefeated ? "victory" : "defeat";
            _lastCombatResultId = resultId;
            _lastCombatEnemyDefeated = enemyDefeated;
            CapturePostCombatDeltas(enemyDefeated ? _activeCombatHandoff.onVictoryEffects : _activeCombatHandoff.onDefeatEffects);
            
            _activeCombatEffectApplier?.Invoke(this, enemyDefeated ? _activeCombatHandoff.onVictoryEffects : _activeCombatHandoff.onDefeatEffects);
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.CombatCompleted, RunId, _activeCombatNodeId, resultId));

            if (playerDefeated && !enemyDefeated)
            {
                ApplyNpcTrigger("battle.defeat");
                SetNpcReaction("NPC_REACT_COMBAT_DEFEAT");
                FailRun();
            }
            else if (enemyDefeated)
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
            var modifiers = CombatAbilityModifiers.From(Abilities.Abilities, _activeSynergies, BuildOwnedItemData());
            var player = new CombatantState("player", _playerMaxHp, _playerAttack, _playerHp);
            var combatStartRestore = modifiers.CombatStartHpRestore;
            if (combatStartRestore > 0)
            {
                player.RestoreHp(combatStartRestore);
            }

            var enemyId = handoff.enemyRefs != null && handoff.enemyRefs.Length > 0 && !string.IsNullOrEmpty(handoff.enemyRefs[0])
                ? handoff.enemyRefs[0]
                : "enemy.placeholder";
            var combatId = string.IsNullOrEmpty(handoff.stableId) ? encounter == null ? nodeId : encounter.Id : handoff.stableId;
            var poolRank = ResolveCombatPoolRank(encounter, combatId);
            var enemyData = ResolveCombatEnemyData(context, poolRank, string.IsNullOrEmpty(handoff.seedKey) ? combatId : handoff.seedKey, enemyId);
            var enemy = enemyData != null
                ? CreateEnemyState(enemyData)
                : new CombatantState(enemyId, FallbackEnemyHp, FallbackEnemyAttack);
            var combat = new CombatController(context, string.IsNullOrEmpty(handoff.seedKey) ? combatId : handoff.seedKey);

            _lastCombatId = combatId;
            _lastCombatEnemyId = enemy.Id;
            _lastCombatResultId = "started";
            _lastCombatRoundResult = "round 0 | ready" +
                (combatStartRestore > 0 ? " | bandage " + combatStartRestore : string.Empty) +
                (modifiers.PlayerMaxHpBonus > 0 ? " | maxHp +" + modifiers.PlayerMaxHpBonus : string.Empty) +
                (HasAbilityRef("ABILITY_SCOUT") ? " | scout +" + modifiers.PlayerAttackBonus : string.Empty) +
                (HasAbilityRef("ABILITY_RECALL_ANCHOR") && !HasFlag("FLAG_RECALL_ANCHOR_USED") ? " | recall ready" : string.Empty);
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
                    ResolveCombatRoundInteractive(rounds == 0 && HasPlayableSkill() ? CombatAction.Skill : CombatAction.Attack);
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

        private string ApplyRestInteractionEffect(string actionId)
        {
            if (actionId == "rest.ask_mood")
            {
                ModifyAffinity(2);
                SetFlag("MATAIOS_HINT_S1_01", true);
                ApplyNpcTrigger("rest.ask_mood");
                return "rest ask mood complete | Affinity +2";
            }

            if (actionId == "rest.train")
            {
                _trainingBuffActive = true;
                SetFlag("MATAIOS_TRAINING_BUFF_ACTIVE", true);
                ApplyNpcTrigger("rest.train");
                return "rest training complete | training buff +1 next combat";
            }

            _playerHp = _playerMaxHp;
            ModifyGlitchLevel(-3);
            ApplyNpcTrigger("rest.recover");
            return "rest recover complete | HP restored | Glitch -3";
        }

        private static string NormalizeRestActionId(string actionId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return string.Empty;
            }

            switch (actionId)
            {
                case "rest.ask_mood":
                case "rest.train":
                case "rest.recover":
                    return actionId;
                default:
                    return string.Empty;
            }
        }

        private static bool RestActionRequiresInput(string actionId)
        {
            return actionId == "rest.ask_mood" || actionId == "rest.train";
        }

        private static string BuildTemporaryRestResponse(string actionId)
        {
            switch (actionId)
            {
                case "rest.ask_mood":
                    return "임시 응답: 상태를 확인했다.";
                case "rest.train":
                    return "임시 응답: 다음 전투를 준비했다.";
                case "rest.recover":
                    return "임시 응답: 잠시 호흡을 고른다.";
                default:
                    return "임시 응답";
            }
        }

        private static string BuildRestReflectionSummary(string actionId, string utterance)
        {
            return "restAction=" + actionId + ";utterance=" + (utterance ?? string.Empty);
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
            return HasAbilityRef("ABILITY_SCOUT");
        }

        private EnemyData ResolveCombatEnemyData(DeterministicRunContext context, EnemyPoolRank rank, string seedKey, string fallbackEnemyId)
        {
            if (TryGetSelectedMapNode(out _) &&
                EncounterCatalog != null &&
                EncounterCatalog.TrySelectEnemyForFloor(_currentFloor, rank, context, seedKey, out var pooledEnemy))
            {
                return pooledEnemy;
            }

            return EncounterCatalog != null && EncounterCatalog.TryGetEnemy(fallbackEnemyId, out var fallbackEnemy)
                ? fallbackEnemy
                : null;
        }

        private EnemyPoolRank ResolveCombatPoolRank(EncounterData encounter, string combatId)
        {
            if (TryGetSelectedMapNode(out var selectedNode) && selectedNode.Type == PrototypeFloorMapNodeType.Boss)
            {
                return EnemyPoolRank.Boss;
            }

            var encounterId = encounter == null ? string.Empty : encounter.Id;
            if (encounterId.Contains("ELITE") || combatId.Contains("ELITE"))
            {
                return EnemyPoolRank.Elite;
            }

            return EnemyPoolRank.Normal;
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

        private void CompleteRun(string cause, bool restartReady)
        {
            if (_runCompleted)
            {
                return;
            }

            _runCompleted = true;
            _restartReady = restartReady;
            var summary = $"cause={cause};nodes={NodesResolved};battles={BattlesWon}";
            _reflectionPipeline.TrySaveReflection(RunId, summary, out _);
            ApplyNpcTrigger("run.completed");
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.RunCompleted, RunId, RunId, cause));
        }

        private void ClearRun()
        {
            if (_runCompleted)
            {
                return;
            }

            _runClear = true;
            _runFailed = false;
            _endingRest = false;
            _endingContinue = false;
            _endingChoiceId = string.Empty;
            SetNpcReaction("NPC_REACT_RUN_CLEAR");
            CompleteRun("run.clear", false);
        }

        private void FailRun()
        {
            if (_runCompleted)
            {
                return;
            }

            _runFailed = true;
            _runClear = false;
            _endingRest = false;
            _endingContinue = false;
            _endingChoiceId = string.Empty;
            SetNpcReaction("NPC_REACT_RUN_FAILED");
            CompleteRun("run.failed", true);
        }

        private void UpdateDemoProgression(string nodeId, string encounterId, bool applied)
        {
            if (!applied || _runCompleted || _demoRunPath.Count == 0)
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
                    MarkSelectedMapNodeCompleted(key);
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
                    ClearRun();
                }
            }
        }

        private bool ShouldAdvanceDemoProgression(EncounterData encounter, string choiceStableId, bool applied)
        {
            if (!applied)
            {
                return false;
            }

            return !IsInCombat || AutoResolveCombat || !ChoiceStartsCombat(encounter, choiceStableId);
        }

        private static bool ChoiceStartsCombat(EncounterData encounter, string choiceStableId)
        {
            if (encounter == null || encounter.Choices == null)
            {
                return false;
            }

            for (var i = 0; i < encounter.Choices.Length; i++)
            {
                var choice = encounter.Choices[i];
                if (choice == null || choice.stableId != choiceStableId || choice.effects == null)
                {
                    continue;
                }

                for (var j = 0; j < choice.effects.Length; j++)
                {
                    if (choice.effects[j] != null && choice.effects[j].kind == "StartCombat")
                    {
                        return true;
                    }
                }
            }

            return false;
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

        private bool IsCurrentBossGateUnlocked()
        {
            if (_runCompleted || _stairUnlocked || !TryGetNextDemoStep(out var next))
            {
                return false;
            }

            if (_floorMapNodes.Count > 0)
            {
                if (TryGetSelectedMapNode(out var selected))
                {
                    return selected.Type == PrototypeFloorMapNodeType.Boss;
                }

                return false;
            }

            return HasStartCombatEffect(next.Encounter);
        }

        private bool TryGetSelectedMapNode(out PrototypeFloorMapNode node)
        {
            node = null;
            if (string.IsNullOrEmpty(_selectedMapNodeId))
            {
                return false;
            }

            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                if (_floorMapNodes[i] != null && _floorMapNodes[i].MapNodeId == _selectedMapNodeId && !_floorMapNodes[i].Completed && !_floorMapNodes[i].Skipped)
                {
                    node = _floorMapNodes[i];
                    return true;
                }
            }

            _selectedMapNodeId = string.Empty;
            return false;
        }

        private string GetResolvedChoiceNodeId(string nodeId)
        {
            return _floorMapNodes.Count > 0 && !string.IsNullOrEmpty(_selectedMapNodeId)
                ? _selectedMapNodeId
                : nodeId;
        }

        private bool TryGetFirstSelectableMapNode(out PrototypeFloorMapNode node)
        {
            node = null;
            var activeLayer = GetActiveMapLayer();
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                if (IsMapNodeSelectable(_floorMapNodes[i], activeLayer))
                {
                    node = _floorMapNodes[i];
                    return true;
                }
            }

            return false;
        }

        private int GetActiveMapLayer()
        {
            var activeLayer = int.MaxValue;
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (node != null && !node.Completed && !node.Skipped && node.Layer < activeLayer)
                {
                    activeLayer = node.Layer;
                }
            }

            return activeLayer == int.MaxValue ? 0 : activeLayer;
        }

        private bool IsMapNodeSelectable(PrototypeFloorMapNode node, int activeLayer)
        {
            return node != null && node.IsValid && !node.Completed && !node.Skipped && node.Layer == activeLayer && string.IsNullOrEmpty(_selectedMapNodeId);
        }

        private void MarkSelectedMapNodeCompleted(string resolvedKey)
        {
            PrototypeFloorMapNode completedNode = null;
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (node == null)
                {
                    continue;
                }

                var selectedMatches = string.IsNullOrEmpty(_selectedMapNodeId) || node.MapNodeId == _selectedMapNodeId;
                if (selectedMatches && BuildResolvedEncounterKey(node.Step.NodeId, node.Step.EncounterId) == resolvedKey)
                {
                    node.Completed = true;
                    completedNode = node;
                    break;
                }
            }

            if (completedNode != null)
            {
                for (var i = 0; i < _floorMapNodes.Count; i++)
                {
                    var node = _floorMapNodes[i];
                    if (node != null && node.Layer == completedNode.Layer && node.MapNodeId != completedNode.MapNodeId && !node.Completed)
                    {
                        node.Skipped = true;
                    }
                }
            }

            _selectedMapNodeId = string.Empty;
        }

        private bool IsFloorMapComplete()
        {
            var hasBoss = false;
            var bossComplete = false;
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                if (node == null)
                {
                    continue;
                }

                if (!node.Completed && !node.Skipped)
                {
                    return false;
                }

                if (node.Type == PrototypeFloorMapNodeType.Boss)
                {
                    hasBoss = true;
                    bossComplete = node.Completed;
                }
            }

            return hasBoss && bossComplete;
        }

        private PrototypeFloorMapNodeView[] BuildMapNodeViews()
        {
            var views = new PrototypeFloorMapNodeView[_floorMapNodes.Count];
            var activeLayer = GetActiveMapLayer();
            for (var i = 0; i < _floorMapNodes.Count; i++)
            {
                var node = _floorMapNodes[i];
                var locked = node == null || node.Skipped || (activeLayer > 0 && node.Layer > activeLayer);
                views[i] = ToMapNodeView(node, IsMapNodeSelectable(node, activeLayer), locked);
            }

            return views;
        }

        private static PrototypeFloorMapNodeView ToMapNodeView(PrototypeFloorMapNode node, bool selectable, bool locked)
        {
            return node == null
                ? default
                : new PrototypeFloorMapNodeView(node.MapNodeId, node.Type, node.Floor, node.Layer, node.Index, selectable, node.Completed, locked);
        }

        private static bool HasStartCombatEffect(EncounterData encounter)
        {
            if (encounter == null || encounter.Choices == null)
            {
                return false;
            }

            for (var i = 0; i < encounter.Choices.Length; i++)
            {
                var choice = encounter.Choices[i];
                if (choice == null || choice.effects == null)
                {
                    continue;
                }

                for (var j = 0; j < choice.effects.Length; j++)
                {
                    if (choice.effects[j] != null && choice.effects[j].kind == "StartCombat")
                    {
                        return true;
                    }
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
