using System.Collections.Generic;
using HwigiTower.Audio;
using HwigiTower.Core;
using HwigiTower.Combat;
using HwigiTower.Encounters;
using HwigiTower.NPC;
using HwigiTower.UI;
using UnityEngine;

namespace HwigiTower.Run
{
    public sealed class PrototypeRoomController : MonoBehaviour
    {
        [SerializeField] private PrototypeRoomDefinition roomDefinition;
        [SerializeField] private EncounterRuntimeCatalogData encounterRuntimeCatalog;
        [SerializeField] private DemoPresentationData demoPresentationData;
        [SerializeField] private PrototypeAudioService audioService;
        [SerializeField] private PrototypeHud hud;
        [SerializeField] private bool autoResolveCombat;
        [SerializeField] private bool showDemoNodeDebugLabels;

        public DeterministicRunContext RunContext { get; private set; }
        public GameFlowEventBus EventBus { get; } = new GameFlowEventBus();
        public PrototypeRunState RunState { get; private set; }
        public EncounterRuntimeCatalogData EncounterRuntimeCatalog => encounterRuntimeCatalog;
        private INPCMemoryRepo _persistentMemoryRepo;
        private readonly HashSet<string> _persistentMemoryFragmentRefs = new HashSet<string>();
        private int _restartIndex;
        private string ActiveRunId => RunState == null ? RunContext.RunId : RunState.RunId;
        public bool AutoResolveCombat
        {
            get => autoResolveCombat;
            set
            {
                autoResolveCombat = value;
                if (RunState != null)
                {
                    RunState.AutoResolveCombat = value;
                }
            }
        }

        private void Awake()
        {
            RebuildContext();
            ResolveHudReference();
            ApplyDemoNodeDebugLabelVisibility();
        }

        private void Start()
        {
            if (RunState == null)
            {
                if (PrototypeRunSaveRequest.ConsumeContinueRequested() && PrototypeRunSaveStore.TryLoad(out var saveData))
                {
                    StartRunFromSave(saveData);
                }
                else
                {
                    BeginRun();
                }
            }

            hud?.ShowRunState(GetSnapshot());
        }

        public void SetDemoNodeDebugLabelsVisible(bool visible)
        {
            showDemoNodeDebugLabels = visible;
            ApplyDemoNodeDebugLabelVisibility();
        }

        public void SetDemoPresentationData(DemoPresentationData data)
        {
            demoPresentationData = data;
            ConfigureHudDemoRoute();
        }

        public void Configure(PrototypeRoomDefinition definition)
        {
            Configure(definition, encounterRuntimeCatalog);
        }

        public void Configure(PrototypeRoomDefinition definition, EncounterRuntimeCatalogData runtimeCatalog)
        {
            roomDefinition = definition;
            encounterRuntimeCatalog = runtimeCatalog;
            RebuildContext();
        }

        public void BeginRun()
        {
            PrototypeRunSaveRequest.RequestNewGame();
            _restartIndex = 0;
            StartRun(RunContext.RunId);
        }

        public PrototypeNodeResolution RestartRun()
        {
            if (RunState == null || !RunState.RestartReady)
            {
                return new PrototypeNodeResolution("run.restart", string.Empty, "restart unavailable", false);
            }

            PreserveCurrentMemoryFragments();
            _restartIndex++;
            var nextRunId = RunContext.RunId + ".restart." + _restartIndex;
            StartRun(nextRunId);
            return new PrototypeNodeResolution("run.restart", nextRunId, "run restarted: " + nextRunId, false);
        }

        private void StartRun(string runId)
        {
            if (_persistentMemoryRepo == null)
            {
                _persistentMemoryRepo = new InMemoryNpcMemoryRepo();
            }

            RunState = new PrototypeRunState(runId, EventBus, null, _persistentMemoryRepo) { AutoResolveCombat = autoResolveCombat };
            RunState.AttachEncounterCatalog(encounterRuntimeCatalog);
            RestorePersistentMemoryFragments();
            if (roomDefinition == null)
            {
                RunState.AttachDemoRunPath(null);
            }
            else
            {
                RunState.AttachFloorRunPaths(roomDefinition.FloorRunPaths, roomDefinition.DemoRunPath);
            }
            ConfigureHudDemoRoute();
            RunState.ModifyGold(6);
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.RunStarted, runId, string.Empty, string.Empty));
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.RoomEntered, runId, RunContext.RunId, string.Empty));
            PlayAudioContext(PrototypeAudioContext.Exploration);
            SaveCurrentRun();
        }

        private void StartRunFromSave(PrototypeRunSaveData saveData)
        {
            if (saveData == null || string.IsNullOrEmpty(saveData.runId))
            {
                BeginRun();
                return;
            }

            if (_persistentMemoryRepo == null)
            {
                _persistentMemoryRepo = new InMemoryNpcMemoryRepo();
            }

            RunState = new PrototypeRunState(saveData.runId, EventBus, null, _persistentMemoryRepo) { AutoResolveCombat = autoResolveCombat };
            RunState.AttachEncounterCatalog(encounterRuntimeCatalog);
            RunState.RestoreFromSaveData(saveData, roomDefinition == null ? null : roomDefinition.FloorRunPaths, roomDefinition == null ? null : roomDefinition.DemoRunPath);
            PreserveCurrentMemoryFragments();
            ConfigureHudDemoRoute();
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.RunStarted, saveData.runId, "save.loaded", string.Empty));
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.RoomEntered, saveData.runId, "save.loaded", string.Empty));
            PlayAudioContext(PrototypeAudioContext.Exploration);
        }

        private void PreserveCurrentMemoryFragments()
        {
            if (RunState == null)
            {
                return;
            }

            foreach (var memoryFragmentRef in RunState.MemoryFragmentRefs)
            {
                if (!string.IsNullOrEmpty(memoryFragmentRef))
                {
                    _persistentMemoryFragmentRefs.Add(memoryFragmentRef);
                }
            }
        }

        private void RestorePersistentMemoryFragments()
        {
            if (RunState == null || _persistentMemoryFragmentRefs.Count == 0)
            {
                return;
            }

            foreach (var memoryFragmentRef in _persistentMemoryFragmentRefs)
            {
                RunState.UnlockMemoryFragmentRef(memoryFragmentRef);
            }
        }

        public void NotifyNodeEntered(InteractableNode node)
        {
            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.NodeEntered, ActiveRunId, nodeId, string.Empty));
        }

        public void NotifyNodeExited(InteractableNode node)
        {
            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.NodeExited, ActiveRunId, nodeId, string.Empty));
        }

        public void NotifyNodeResolved(InteractableNode node, string payloadId)
        {
            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.NodeResolved, ActiveRunId, nodeId, payloadId));
        }

        public bool HasEncounterChoices(EncounterSelection selection)
        {
            return selection.HasEncounter
                && selection.Encounter.Choices != null
                && selection.Encounter.Choices.Length > 0;
        }

        public EncounterSelection SelectEncounter(InteractableNode node)
        {
            if (node == null || node.Definition == null)
            {
                return new EncounterSelection(null, null);
            }

            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState != null && RunState.RunCompleted)
            {
                return new EncounterSelection(node.Definition, null);
            }

            if (RunState != null &&
                RunState.TryGetNextDemoStep(out var step) &&
                step.Node == node.Definition)
            {
                return new EncounterSelection(node.Definition, step.Encounter);
            }

            return new EncounterSelector().Select(RunContext, node.Definition);
        }

        public EncounterSelection SelectCurrentRouteEncounter()
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState == null || RunState.RunCompleted || !RunState.TryGetNextDemoStep(out var step))
            {
                return new EncounterSelection(null, null);
            }

            var selectable = RunState.GetSelectableMapNodeViews();
            if (selectable.Length == 1 && RunState.TrySelectMapNode(selectable[0].MapNodeId, out var selectedStep))
            {
                step = selectedStep;
            }

            PlayAudioContextForEncounter(step.Encounter);
            return new EncounterSelection(step.Node, step.Encounter);
        }

        public PrototypeFloorMapNodeView[] GetSelectableMapNodes()
        {
            if (RunState == null)
            {
                BeginRun();
            }

            return RunState == null ? new PrototypeFloorMapNodeView[0] : RunState.GetSelectableMapNodeViews();
        }

        public PrototypeFloorMapNodeView[] GetFloorMapNodes()
        {
            if (RunState == null)
            {
                BeginRun();
            }

            return RunState == null ? new PrototypeFloorMapNodeView[0] : RunState.GetFloorMapNodeViews();
        }

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public PrototypeFloorMapNodeView[] GetQaFloorMapNodes()
        {
            if (RunState == null)
            {
                BeginRun();
            }

            return RunState == null ? new PrototypeFloorMapNodeView[0] : RunState.GetQaFloorMapNodeViews();
        }

        public EncounterSelection CreateQaEncounterSelection(string encounterId)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (TryFindQaStep(encounterId, out var step))
            {
                PlayAudioContextForEncounter(step.Encounter);
                return new EncounterSelection(step.Node, step.Encounter);
            }

            return new EncounterSelection(null, null);
        }

        public void OpenQaEndingChoice()
        {
            if (RunState == null)
            {
                BeginRun();
            }

            RunState?.OpenQaEndingChoice();
            PlayAudioContext(PrototypeAudioContext.Ending);
        }

        public bool OpenQaFloor(int floor)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            var opened = RunState != null && RunState.OpenQaFloor(floor);
            if (opened)
            {
                hud?.ShowRunState(GetSnapshot());
            }

            return opened;
        }

        private bool TryFindQaStep(string encounterId, out PrototypeDemoRunStep step)
        {
            step = null;
            if (string.IsNullOrEmpty(encounterId) || roomDefinition == null)
            {
                return false;
            }

            if (TryFindQaStepInPath(roomDefinition.DemoRunPath, encounterId, out step))
            {
                return true;
            }

            var paths = roomDefinition.FloorRunPaths;
            if (paths != null)
            {
                for (var i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];
                    if (path != null && TryFindQaStepInPath(path.Steps, encounterId, out step))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryFindQaStepInPath(IReadOnlyList<PrototypeDemoRunStep> steps, string encounterId, out PrototypeDemoRunStep step)
        {
            step = null;
            if (steps == null)
            {
                return false;
            }

            for (var i = 0; i < steps.Count; i++)
            {
                var candidate = steps[i];
                if (candidate != null && candidate.IsValid && candidate.EncounterId == encounterId)
                {
                    step = candidate;
                    return true;
                }
            }

            return false;
        }
#endif

        public EncounterSelection SelectMapNode(string mapNodeId)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState == null || RunState.RunCompleted || !RunState.TrySelectMapNode(mapNodeId, out var step))
            {
                return new EncounterSelection(null, null);
            }

            PlayAudioContextForEncounter(step.Encounter);
            return new EncounterSelection(step.Node, step.Encounter);
        }

        public PrototypeEncounterChoiceView[] BuildEncounterChoiceViews(EncounterSelection selection)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState != null && RunState.RunCompleted)
            {
                return new PrototypeEncounterChoiceView[0];
            }

            if (TryGetResolvedEncounterChoice(selection, out _))
            {
                return new PrototypeEncounterChoiceView[0];
            }

            return HasEncounterChoices(selection)
                ? PrototypeEncounterRuntimeResolver.BuildChoiceViews(RunState, selection.Encounter)
                : new PrototypeEncounterChoiceView[0];
        }

        public bool TryGetResolvedEncounterChoice(EncounterSelection selection, out string choiceStableId)
        {
            choiceStableId = string.Empty;
            if (RunState == null || !selection.HasEncounter || selection.Node == null)
            {
                return false;
            }

            return RunState.TryGetResolvedEncounterChoice(selection.Node.NodeId, selection.EncounterId, out choiceStableId);
        }

        public PrototypeNodeResolution ResolveCurrentRouteChoice(EncounterSelection selection, string choiceStableId)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState == null || RunState.RunCompleted)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "run already completed", true);
            }

            if (!selection.HasEncounter || selection.Node == null)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "route unavailable", false);
            }

            var resolution = RunState.ResolveEncounterChoice(CreateActiveContext(), selection.Node.NodeId, selection.Encounter, choiceStableId);
            PlayAudioContextForSnapshot(RunState.CreateSnapshot(), selection.Encounter);
            SaveCurrentRun();
            return resolution;
        }

        public PrototypeNodeResolution ResolveCurrentRouteRestInteraction(EncounterSelection selection, string actionId, string utterance)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState == null || RunState.RunCompleted)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "run already completed", true);
            }

            if (!selection.HasEncounter || selection.Node == null)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "route unavailable", false);
            }

            var resolution = RunState.ResolveRestInteraction(selection.Node.NodeId, selection.EncounterId, actionId, utterance);
            PlayAudioContext(PrototypeAudioContext.Rest);
            SaveCurrentRun();
            return resolution;
        }

        public PrototypeNodeResolution ResolveCurrentRouteNode(EncounterSelection selection)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState == null || RunState.RunCompleted)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "run already completed", true);
            }

            if (selection.Node == null)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "route unavailable", false);
            }

            var resolution = ResolveNodeDefinition(selection.Node, selection);
            SaveCurrentRun();
            return resolution;
        }

        public PrototypeNodeResolution ResolveEncounterChoice(InteractableNode node, EncounterData encounter, string choiceStableId)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState != null && RunState.RunCompleted)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "run already completed", true);
            }

            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            var resolution = RunState.ResolveEncounterChoice(CreateActiveContext(), nodeId, encounter, choiceStableId);
            NotifyNodeResolved(node, resolution.PayloadId);
            SaveCurrentRun();
            return resolution;
        }

        public PrototypeNodeResolution ResolveNextFloor()
        {
            if (RunState == null)
            {
                BeginRun();
            }

            var resolution = RunState.ResolveNextFloor();
            ConfigureHudDemoRoute();
            PlayAudioContext(PrototypeAudioContext.Exploration);
            SaveCurrentRun();
            return resolution;
        }

        public PrototypeNodeResolution ResolveEndingChoice(string choiceStableId)
        {
            if (RunState == null)
            {
                return new PrototypeNodeResolution("ending.choice", string.Empty, "ending unavailable", false);
            }

            var resolution = RunState.ResolveEndingChoice(choiceStableId);
            PlayAudioContext(PrototypeAudioContext.Ending);
            SaveCurrentRun();
            return resolution;
        }

        public PrototypeNodeResolution ResolveCombatAction(CombatAction action)
        {
            if (RunState == null)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "combat unavailable", false);
            }

            if (RunState.RunCompleted && !RunState.IsInCombat)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "run already completed", true);
            }

            var hpBefore = RunState.ActiveCombatPlayer?.Hp ?? RunState.PlayerHp;
            var round = RunState.ResolveCombatRoundInteractive(action);
            var snapshot = RunState.CreateSnapshot();
            PlayAudioContextForSnapshot(snapshot, null);
            var message =
                "combat " + snapshot.LastCombatResultId +
                " | round " + snapshot.CombatRound +
                " | action " + action +
                " | player HP " + hpBefore + " -> " + snapshot.PlayerHp +
                " | enemy " + snapshot.LastCombatEnemyId + " " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp +
                " | playerDamage " + round.PlayerDamage +
                " | enemyDamage " + round.EnemyDamage;
            if (round.ComboDamage > 0)
            {
                message += " | combo " + round.ComboDamage;
            }

            if (!snapshot.IsInCombat)
            {
                message +=
                    " | enemyDefeated " + snapshot.LastCombatEnemyDefeated +
                    " | gold reward " + snapshot.LastCombatGoldReward +
                    " | glitch " + FormatDelta(snapshot.LastCombatGlitchDelta) +
                    " | affinity " + FormatDelta(snapshot.LastCombatAffinityDelta);
                if (snapshot.RunClear)
                {
                    message += " | run.clear";
                }
                else if (snapshot.RunFailed)
                {
                    message += " | run.failed";
                }
                else if (snapshot.StairUnlocked)
                {
                    message += " | stair unlocked";
                }

                if (snapshot.RestartReady)
                {
                    message += " | run.restartReady";
                }
            }

            SaveCurrentRun();
            return new PrototypeNodeResolution(snapshot.LastCombatId, snapshot.LastCombatResultId, message, snapshot.RunCompleted);
        }

        public PrototypeNodeResolution ResolveNode(InteractableNode node, EncounterSelection selection)
        {
            if (RunState == null)
            {
                BeginRun();
            }

            if (RunState.RunCompleted)
            {
                return new PrototypeNodeResolution(
                    node == null || node.Definition == null ? string.Empty : node.Definition.NodeId,
                    string.Empty,
                    "run already completed",
                    true);
            }

            if (node == null || node.Definition == null)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "no node", false);
            }

            var definition = node.Definition;
            var encounterId = selection.EncounterId;
            var enemy = selection.HasEncounter && selection.Encounter.Enemy != null
                ? selection.Encounter.Enemy
                : definition.FallbackEnemy;

            var resolution = ResolveNodeDefinition(definition, selection, enemy);
            NotifyNodeResolved(node, resolution.PayloadId);
            SaveCurrentRun();
            return resolution;
        }

        public void SaveCurrentRun()
        {
            if (RunState == null)
            {
                return;
            }

            PrototypeRunSaveStore.Save(RunState.CreateSaveData());
        }

        private PrototypeNodeResolution ResolveNodeDefinition(PrototypeNodeDefinition definition, EncounterSelection selection)
        {
            var enemy = selection.HasEncounter && selection.Encounter.Enemy != null
                ? selection.Encounter.Enemy
                : definition.FallbackEnemy;
            return ResolveNodeDefinition(definition, selection, enemy);
        }

        private PrototypeNodeResolution ResolveNodeDefinition(PrototypeNodeDefinition definition, EncounterSelection selection, EnemyData enemy)
        {
            var encounterId = selection.EncounterId;
            PrototypeNodeResolution resolution;
            if (HasEncounterChoices(selection))
            {
                if (RunState.TryGetResolvedEncounterChoice(definition.NodeId, encounterId, out var resolvedChoiceStableId))
                {
                    return new PrototypeNodeResolution(definition.NodeId, resolvedChoiceStableId, $"already resolved: {resolvedChoiceStableId}", false);
                }

                return new PrototypeNodeResolution(definition.NodeId, encounterId, $"choices pending: {encounterId}", false);
            }

            switch (definition.Kind)
            {
                case NodeKind.Battle:
                    resolution = RunState.ResolveBattle(CreateActiveContext(), definition.NodeId, encounterId, enemy, definition.TrackedSynergies);
                    break;
                case NodeKind.Rest:
                    resolution = RunState.ResolveRest(definition.NodeId);
                    break;
                case NodeKind.Shop:
                    resolution = RunState.ResolveShop(definition.NodeId, definition.GrantedAbility, definition.TrackedSynergies);
                    break;
                case NodeKind.Remnant:
                    resolution = RunState.ResolveRemnant(definition.NodeId);
                    break;
                default:
                    resolution = RunState.ResolveGeneric(definition.NodeId, encounterId, definition.PlaceholderOutcome);
                    break;
            }

            return resolution;
        }

        public PrototypeRunSnapshot GetSnapshot()
        {
            if (RunState == null)
            {
                return new PrototypeRunSnapshot(RunContext.RunId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);
            }

            return RunState.CreateSnapshot();
        }

        private void PlayAudioContextForEncounter(EncounterData encounter)
        {
            if (encounter == null)
            {
                PlayAudioContext(PrototypeAudioContext.Exploration);
                return;
            }

            var context = encounter.Type switch
            {
                EncounterType.Shop => PrototypeAudioContext.Shop,
                EncounterType.Rest => PrototypeAudioContext.Rest,
                EncounterType.Battle => encounter.Id == "ENC_COMBAT_GATE_03" ? PrototypeAudioContext.Boss : PrototypeAudioContext.Combat,
                EncounterType.Remnant => PrototypeAudioContext.Ending,
                _ => PrototypeAudioContext.Event
            };
            PlayAudioContext(context);
        }

        private void PlayAudioContextForSnapshot(PrototypeRunSnapshot snapshot, EncounterData encounter)
        {
            if (snapshot.EndingChoicePending || snapshot.EndingRest || snapshot.EndingContinue)
            {
                PlayAudioContext(PrototypeAudioContext.Ending);
                return;
            }

            if (snapshot.IsInCombat)
            {
                PlayAudioContext(snapshot.LastCombatEnemyId == "BOSS_APEX_02" ? PrototypeAudioContext.Boss : PrototypeAudioContext.Combat);
                return;
            }

            PlayAudioContextForEncounter(encounter);
        }

        private void PlayAudioContext(PrototypeAudioContext context)
        {
            if (audioService == null)
            {
                audioService = PrototypeAudioService.GetOrCreate();
            }

            audioService.PlayContext(context);
            var floor = RunState == null ? 1 : RunState.CurrentFloor;
            var ambience = floor switch
            {
                2 => PrototypeAudioContext.Floor02Ambience,
                3 => PrototypeAudioContext.Floor03Ambience,
                4 => PrototypeAudioContext.Floor04Ambience,
                5 => PrototypeAudioContext.Floor05Ambience,
                _ => PrototypeAudioContext.Floor01Ambience
            };
            audioService.PlayContext(ambience);
        }

        private void RebuildContext()
        {
            var seed = roomDefinition == null ? 1001 : roomDefinition.DeterministicSeed;
            var roomId = roomDefinition == null ? "room.prototype" : roomDefinition.RoomId;
            RunContext = new DeterministicRunContext(roomId, seed);
        }

        private DeterministicRunContext CreateActiveContext()
        {
            return new DeterministicRunContext(ActiveRunId, RunContext.Seed);
        }

        private void ConfigureHudDemoRoute()
        {
            ResolveHudReference();
            if (hud != null)
            {
                hud.BindRoomController(this);
                hud.SetPresentationData(demoPresentationData);
                var floor = RunState == null ? 1 : RunState.CurrentFloor;
                hud.ConfigureDemoRoute(roomDefinition == null ? null : roomDefinition.GetRunPathForFloor(floor));
            }
        }

        private void ResolveHudReference()
        {
            if (hud == null)
            {
                hud = FindFirstObjectByType<PrototypeHud>();
            }
        }

        private void ApplyDemoNodeDebugLabelVisibility()
        {
            var labels = Object.FindObjectsByType<TextMesh>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (label == null || label.gameObject.name != "Label" || label.transform.parent == null)
                {
                    continue;
                }

                if (label.transform.parent.GetComponent<InteractableNode>() != null)
                {
                    label.gameObject.SetActive(showDemoNodeDebugLabels);
                }
            }
        }

        private static string FormatDelta(int amount)
        {
            return amount > 0 ? "+" + amount : amount.ToString();
        }
    }
}
