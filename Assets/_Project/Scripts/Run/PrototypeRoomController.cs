using HwigiTower.Core;
using HwigiTower.Encounters;
using UnityEngine;

namespace HwigiTower.Run
{
    public sealed class PrototypeRoomController : MonoBehaviour
    {
        [SerializeField] private PrototypeRoomDefinition roomDefinition;

        public DeterministicRunContext RunContext { get; private set; }
        public GameFlowEventBus EventBus { get; } = new GameFlowEventBus();

        private void Awake()
        {
            RebuildContext();
        }

        public void Configure(PrototypeRoomDefinition definition)
        {
            roomDefinition = definition;
            RebuildContext();
        }

        public void BeginRun()
        {
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

        private void RebuildContext()
        {
            var seed = roomDefinition == null ? 1001 : roomDefinition.DeterministicSeed;
            var roomId = roomDefinition == null ? "room.prototype" : roomDefinition.RoomId;
            RunContext = new DeterministicRunContext(roomId, seed);
        }
    }
}
