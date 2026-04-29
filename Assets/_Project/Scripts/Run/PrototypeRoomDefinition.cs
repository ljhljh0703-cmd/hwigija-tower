using System.Collections.Generic;
using HwigiTower.Encounters;
using UnityEngine;

namespace HwigiTower.Run
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Room Definition", fileName = "SO_Room_Prototype")]
    public sealed class PrototypeRoomDefinition : ScriptableObject
    {
        [SerializeField] private string roomId = "room.prototype";
        [SerializeField] private int deterministicSeed = 1001;
        [SerializeField] private Vector2 roomSize = new Vector2(6f, 10f);
        [SerializeField] private PrototypeNodeDefinition[] availableNodes;

        public string RoomId => roomId;
        public int DeterministicSeed => deterministicSeed;
        public Vector2 RoomSize => roomSize;
        public IReadOnlyList<PrototypeNodeDefinition> AvailableNodes => availableNodes;
    }
}
