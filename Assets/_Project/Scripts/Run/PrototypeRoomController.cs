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
        [SerializeField] private PrototypeHud hud;
        [SerializeField] private bool autoResolveCombat;
        [SerializeField] private bool showDemoNodeDebugLabels;

        public DeterministicRunContext RunContext { get; private set; }
        public GameFlowEventBus EventBus { get; } = new GameFlowEventBus();
        public PrototypeRunState RunState { get; private set; }
        public EncounterRuntimeCatalogData EncounterRuntimeCatalog => encounterRuntimeCatalog;
        private INPCMemoryRepo _persistentMemoryRepo;
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
            _restartIndex = 0;
            StartRun(RunContext.RunId);
        }

        public PrototypeNodeResolution RestartRun()
        {
            if (RunState == null || !RunState.RestartReady)
            {
                return new PrototypeNodeResolution("run.restart", string.Empty, "restart unavailable", false);
            }

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
            return resolution;
        }

        public PrototypeNodeResolution ResolveCombatAction(CombatAction action)
        {
            if (RunState == null)
            {
                return new PrototypeNodeResolution(string.Empty, string.Empty, "combat unavailable", false);
            }

            var hpBefore = RunState.ActiveCombatPlayer?.Hp ?? RunState.PlayerHp;
            var round = RunState.ResolveCombatRoundInteractive(action);
            var snapshot = RunState.CreateSnapshot();
            var message =
                "combat " + snapshot.LastCombatResultId +
                " | round " + snapshot.CombatRound +
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

            NotifyNodeResolved(node, resolution.PayloadId);
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
