using UnityEngine;

namespace HwigiTower.Core
{
    public readonly struct MovementInputState
    {
        public MovementInputState(Vector2 move, bool interactPressed)
        {
            Move = Vector2.ClampMagnitude(move, 1f);
            InteractPressed = interactPressed;
        }

        public Vector2 Move { get; }
        public bool InteractPressed { get; }
    }

    public interface IMovementInputSource
    {
        MovementInputState Read();
    }
}
