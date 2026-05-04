using System.Collections.Generic;
using HwigiTower.Encounters;
using UnityEngine;

namespace HwigiTower.Run
{
    [System.Serializable]
    public sealed class PrototypeDemoRunStep
    {
        [SerializeField] private PrototypeNodeDefinition node;
        [SerializeField] private EncounterData encounter;

        public PrototypeDemoRunStep()
        {
        }

        public PrototypeDemoRunStep(PrototypeNodeDefinition node, EncounterData encounter)
        {
            this.node = node;
            this.encounter = encounter;
        }

        public PrototypeNodeDefinition Node => node;
        public EncounterData Encounter => encounter;
        public string NodeId => node == null ? string.Empty : node.NodeId;
        public string EncounterId => encounter == null ? string.Empty : encounter.Id;
        public bool IsValid => node != null && encounter != null;
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Room Definition", fileName = "SO_Room_Prototype")]
    public sealed class PrototypeRoomDefinition : ScriptableObject
    {
        [SerializeField] private string roomId = "room.prototype";
        [SerializeField] private int deterministicSeed = 1001;
        [SerializeField] private Vector2 roomSize = new Vector2(6f, 10f);
        [SerializeField] private PrototypeNodeDefinition[] availableNodes;
        [SerializeField] private PrototypeDemoRunStep[] demoRunPath = new PrototypeDemoRunStep[0];

        public string RoomId => roomId;
        public int DeterministicSeed => deterministicSeed;
        public Vector2 RoomSize => roomSize;
        public IReadOnlyList<PrototypeNodeDefinition> AvailableNodes => availableNodes;
        public IReadOnlyList<PrototypeDemoRunStep> DemoRunPath => demoRunPath;
    }
}
