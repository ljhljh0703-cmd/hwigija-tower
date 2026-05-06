using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.UI;
using UnityEngine;

namespace HwigiTower.Run
{
    public sealed class PrototypeRoomController : MonoBehaviour
    {
        [SerializeField] private PrototypeRoomDefinition roomDefinition;
        [SerializeField] private EncounterRuntimeCatalogData encounterRuntimeCatalog;
        [SerializeField] private PrototypeHud hud;

        public DeterministicRunContext RunContext { get; private set; }
        public GameFlowEventBus EventBus { get; } = new GameFlowEventBus();
        public PrototypeRunState RunState { get; private set; }
        public EncounterRuntimeCatalogData EncounterRuntimeCatalog => encounterRuntimeCatalog;

        private void Awake()
        {
            RebuildContext();
            ResolveHudReference();
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
            RunState = new PrototypeRunState(RunContext.RunId, EventBus) { AutoResolveCombat = true };
            RunState.AttachEncounterCatalog(encounterRuntimeCatalog);
            RunState.AttachDemoRunPath(roomDefinition == null ? null : roomDefinition.DemoRunPath);
            ConfigureHudDemoRoute();
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.RunStarted, RunContext.RunId, string.Empty, string.Empty));
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.RoomEntered, RunContext.RunId, RunContext.RunId, string.Empty));
        }

        public void NotifyNodeEntered(InteractableNode node)
        {
            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.NodeEntered, RunContext.RunId, nodeId, string.Empty));
        }

        public void NotifyNodeExited(InteractableNode node)
        {
            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.NodeExited, RunContext.RunId, nodeId, string.Empty));
        }

        public void NotifyNodeResolved(InteractableNode node, string payloadId)
        {
            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            EventBus.Raise(new GameFlowEvent(GameFlowEventType.NodeResolved, RunContext.RunId, nodeId, payloadId));
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

            var nodeId = node == null || node.Definition == null ? string.Empty : node.Definition.NodeId;
            var resolution = RunState.ResolveEncounterChoice(RunContext, nodeId, encounter, choiceStableId);
            NotifyNodeResolved(node, resolution.PayloadId);
            return resolution;
        }

        public PrototypeNodeResolution ResolveNode(InteractableNode node, EncounterSelection selection)
        {
            if (RunState == null)
            {
                BeginRun();
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
                    resolution = RunState.ResolveBattle(RunContext, definition.NodeId, encounterId, enemy, definition.TrackedSynergies);
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

        private void ConfigureHudDemoRoute()
        {
            ResolveHudReference();
            if (hud != null)
            {
                hud.ConfigureDemoRoute(roomDefinition == null ? null : roomDefinition.DemoRunPath);
            }
        }

        private void ResolveHudReference()
        {
            if (hud == null)
            {
                hud = FindFirstObjectByType<PrototypeHud>();
            }
        }
    }
}
