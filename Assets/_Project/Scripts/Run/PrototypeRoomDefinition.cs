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

    [System.Serializable]
    public sealed class PrototypeFloorRunPath
    {
        [SerializeField, Min(1)] private int floor = 1;
        [SerializeField] private PrototypeDemoRunStep[] steps = new PrototypeDemoRunStep[0];

        public PrototypeFloorRunPath()
        {
        }

        public PrototypeFloorRunPath(int floor, params PrototypeDemoRunStep[] steps)
        {
            this.floor = floor < 1 ? 1 : floor;
            this.steps = steps ?? new PrototypeDemoRunStep[0];
        }

        public int Floor => floor;
        public IReadOnlyList<PrototypeDemoRunStep> Steps => steps;
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Room Definition", fileName = "SO_Room_Prototype")]
    public sealed class PrototypeRoomDefinition : ScriptableObject
    {
        [SerializeField] private string roomId = "room.prototype";
        [SerializeField] private int deterministicSeed = 1001;
        [SerializeField] private Vector2 roomSize = new Vector2(6f, 10f);
        [SerializeField] private PrototypeNodeDefinition[] availableNodes;
        [SerializeField] private PrototypeDemoRunStep[] demoRunPath = new PrototypeDemoRunStep[0];
        [SerializeField] private PrototypeFloorRunPath[] floorRunPaths = new PrototypeFloorRunPath[0];

        public string RoomId => roomId;
        public int DeterministicSeed => deterministicSeed;
        public Vector2 RoomSize => roomSize;
        public IReadOnlyList<PrototypeNodeDefinition> AvailableNodes => availableNodes;
        public IReadOnlyList<PrototypeDemoRunStep> DemoRunPath => demoRunPath;
        public IReadOnlyList<PrototypeFloorRunPath> FloorRunPaths => floorRunPaths;

        public IReadOnlyList<PrototypeDemoRunStep> GetRunPathForFloor(int floor)
        {
            if (floorRunPaths != null)
            {
                for (var i = 0; i < floorRunPaths.Length; i++)
                {
                    var path = floorRunPaths[i];
                    if (path != null && path.Floor == floor && path.Steps != null && path.Steps.Count > 0)
                    {
                        return path.Steps;
                    }
                }
            }

            return floor == 1 ? demoRunPath : new PrototypeDemoRunStep[0];
        }
    }
}
