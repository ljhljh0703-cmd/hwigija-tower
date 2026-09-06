using UnityEngine;
using System.Collections.Generic;
using HwigiTower.Abilities;
using HwigiTower.Combat;

namespace HwigiTower.Encounters
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Node Definition", fileName = "SO_Node_Prototype")]
    public sealed class PrototypeNodeDefinition : ScriptableObject
    {
        [SerializeField] private string nodeId = "node.prototype";
        [SerializeField] private NodeKind kind;
        [SerializeField] private string displayName = "전투";
        [SerializeField, TextArea(1, 2)] private string placeholderOutcome = "placeholder";
        [SerializeField] private EncounterData[] possibleEncounters = new EncounterData[0];
        [SerializeField] private EnemyData fallbackEnemy;
        [SerializeField] private AbilityData grantedAbility;
        [SerializeField] private SynergyData[] trackedSynergies = new SynergyData[0];

        public string NodeId => nodeId;
        public NodeKind Kind => kind;
        public string DisplayName => displayName;
        public string PlaceholderOutcome => placeholderOutcome;
        public IReadOnlyList<EncounterData> PossibleEncounters => possibleEncounters;
        public EnemyData FallbackEnemy => fallbackEnemy;
        public AbilityData GrantedAbility => grantedAbility;
        public IReadOnlyList<SynergyData> TrackedSynergies => trackedSynergies;
    }
}
