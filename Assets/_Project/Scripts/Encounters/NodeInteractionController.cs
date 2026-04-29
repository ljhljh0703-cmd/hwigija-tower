using HwigiTower.Core;
using HwigiTower.UI;
using UnityEngine;

namespace HwigiTower.Encounters
{
    [RequireComponent(typeof(PlayerMovementController))]
    public sealed class NodeInteractionController : MonoBehaviour
    {
        [SerializeField] private PrototypeHud hud;

        private PlayerMovementController _movement;
        private InteractableNode _currentNode;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovementController>();
        }

        public void Configure(PrototypeHud prototypeHud)
        {
            hud = prototypeHud;
        }

        private void Update()
        {
            if (_currentNode != null && _movement.LatestInput.InteractPressed)
            {
                _currentNode.Interact();
                hud?.ShowInteraction(_currentNode);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var node = other.GetComponent<InteractableNode>();
            if (node == null)
            {
                return;
            }

            SetCurrentNode(node);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var node = other.GetComponent<InteractableNode>();
            if (node != null && node == _currentNode)
            {
                SetCurrentNode(null);
            }
        }

        private void SetCurrentNode(InteractableNode node)
        {
            if (_currentNode != null)
            {
                _currentNode.SetHighlighted(false);
            }

            _currentNode = node;

            if (_currentNode != null)
            {
                _currentNode.SetHighlighted(true);
            }

            hud?.ShowFocus(_currentNode);
        }
    }
}
