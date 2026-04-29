using UnityEngine;

namespace HwigiTower.Core
{
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 minBounds = new Vector2(-3.5f, -6f);
        [SerializeField] private Vector2 maxBounds = new Vector2(3.5f, 6f);

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var position = target.position;
            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
            position.z = transform.position.z;
            transform.position = position;
        }

        public void SetTarget(Transform followTarget)
        {
            target = followTarget;
        }
    }
}
