using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HwigiTower.Core
{
    public sealed class PointerMovementInputSource : IMovementInputSource
    {
        private readonly Camera _camera;
        private readonly Transform _player;
        private readonly float _deadZoneWorldUnits;

        public PointerMovementInputSource(Camera camera, Transform player, float deadZoneWorldUnits)
        {
            _camera = camera;
            _player = player;
            _deadZoneWorldUnits = Mathf.Max(0.01f, deadZoneWorldUnits);
        }

        public MovementInputState Read()
        {
            if (_camera == null || _player == null)
            {
                return new MovementInputState(Vector2.zero, false);
            }

#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;
                if (touch.press.isPressed)
                {
                    return ReadFromScreenPosition(touch.position.ReadValue(), touch.press.wasPressedThisFrame);
                }
            }

            if (Pointer.current != null && Pointer.current.press.isPressed)
            {
                return ReadFromScreenPosition(Pointer.current.position.ReadValue(), Pointer.current.press.wasPressedThisFrame);
            }
#else
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                return ReadFromScreenPosition(touch.position, touch.phase == TouchPhase.Began);
            }

            if (Input.GetMouseButton(0))
            {
                return ReadFromScreenPosition(Input.mousePosition, Input.GetMouseButtonDown(0));
            }
#endif

            return new MovementInputState(Vector2.zero, false);
        }

        private MovementInputState ReadFromScreenPosition(Vector2 screenPosition, bool interactPressed)
        {
            var world = _camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -_camera.transform.position.z));
            var delta = (Vector2)(world - _player.position);
            var move = delta.magnitude <= _deadZoneWorldUnits ? Vector2.zero : delta.normalized;
            return new MovementInputState(move, interactPressed);
        }
    }
}
