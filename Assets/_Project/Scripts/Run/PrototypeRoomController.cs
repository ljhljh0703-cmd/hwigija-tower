using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Run
{
    public sealed class PrototypeRoomController : MonoBehaviour
    {
        [SerializeField] private PrototypeRoomDefinition roomDefinition;

        public DeterministicRunContext RunContext { get; private set; }

        private void Awake()
        {
            RebuildContext();
        }

        public void Configure(PrototypeRoomDefinition definition)
        {
            roomDefinition = definition;
            RebuildContext();
        }

        private void RebuildContext()
        {
            var seed = roomDefinition == null ? 1001 : roomDefinition.DeterministicSeed;
            var roomId = roomDefinition == null ? "room.prototype" : roomDefinition.RoomId;
            RunContext = new DeterministicRunContext(roomId, seed);
        }
    }
}
