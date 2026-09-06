using System.Collections.Generic;
using UnityEngine;

namespace HwigiTower.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private PlayerMovementProfile movementProfile;
        [SerializeField] private Camera inputCamera;

        private Rigidbody2D _rigidbody;
        private IMovementInputSource _inputSource;
        private MovementInputState _latestInput;

        public MovementInputState LatestInput => _latestInput;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            RebuildInputSource();
        }

        public void Configure(PlayerMovementProfile profile, Camera cameraOverride)
        {
            movementProfile = profile;
            inputCamera = cameraOverride;
            RebuildInputSource();
        }

        private void RebuildInputSource()
        {
            if (inputCamera == null)
            {
                inputCamera = Camera.main;
            }

            var deadZone = movementProfile == null ? 0.35f : movementProfile.TouchDeadZoneWorldUnits;
            _inputSource = new CompositeMovementInputSource(new List<IMovementInputSource>
            {
                new KeyboardMovementInputSource(),
                new PointerMovementInputSource(inputCamera, transform, deadZone)
            });
        }

        private void Update()
        {
            if (_inputSource == null)
            {
                RebuildInputSource();
            }

            _latestInput = _inputSource.Read();
        }

        private void FixedUpdate()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody2D>();
            }

            var speed = movementProfile == null ? 4f : movementProfile.MoveSpeed;
            _rigidbody.linearVelocity = _latestInput.Move * speed;
        }
    }
}
