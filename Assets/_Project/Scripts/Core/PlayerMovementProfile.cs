using UnityEngine;

namespace HwigiTower.Core
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Prototype/Player Movement Profile", fileName = "SO_Prototype_PlayerMovement")]
    public sealed class PlayerMovementProfile : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 4f;
        [SerializeField, Min(0.01f)] private float touchDeadZoneWorldUnits = 0.35f;

        public float MoveSpeed => moveSpeed;
        public float TouchDeadZoneWorldUnits => touchDeadZoneWorldUnits;
    }
}
