using HwigiTower.Core;
using HwigiTower.Run;
using HwigiTower.UI;
using UnityEngine;

namespace HwigiTower.Encounters
{
    [RequireComponent(typeof(PlayerMovementController))]
    public sealed class NodeInteractionController : MonoBehaviour
    {
        [SerializeField] private PrototypeHud hud;
        [SerializeField] private PrototypeRoomController roomController;

        private PlayerMovementController _movement;
        private InteractableNode _currentNode;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovementController>();
        }

        public void Configure(PrototypeHud prototypeHud, PrototypeRoomController roomController)
        {
            hud = prototypeHud;
            this.roomController = roomController;
            hud?.BindRoomController(roomController);
        }

        private void Update()
        {
            if (_currentNode != null && _movement.LatestInput.InteractPressed)
            {
                _currentNode.Interact();
                if (roomController != null && roomController.GetSnapshot().RunCompleted)
                {
                    hud?.ClearChoices();
                    hud?.ShowRunState(roomController.GetSnapshot());
                    return;
                }

                var selection = roomController == null
                    ? new EncounterSelection(_currentNode.Definition, null)
                    : roomController.SelectEncounter(_currentNode);
                var encounterId = selection.EncounterId;
                PrototypeNodeResolution resolution = default;
                if (roomController != null && _currentNode.Definition != null)
                {
                    roomController.EventBus.Raise(new GameFlowEvent(GameFlowEventType.EncounterSelected, roomController.RunContext.RunId, _currentNode.Definition.NodeId, encounterId));
                    if (roomController.HasEncounterChoices(selection))
                    {
                        ShowEncounterChoices(_currentNode, selection);
                        return;
                    }

                    resolution = roomController.ResolveNode(_currentNode, selection);
                }

                hud?.ShowInteraction(_currentNode, selection, resolution);
                if (roomController != null)
                {
                    hud?.ShowRunState(roomController.GetSnapshot());
                }
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
            var previousNode = _currentNode;
            if (_currentNode != null)
            {
                _currentNode.SetHighlighted(false);
            }

            _currentNode = node;

            if (_currentNode != null)
            {
                _currentNode.SetHighlighted(true);
                roomController?.NotifyNodeEntered(_currentNode);
            }
            else
            {
                roomController?.NotifyNodeExited(previousNode);
            }

            hud?.ShowFocus(_currentNode);
            if (_currentNode == null)
            {
                hud?.ClearChoices();
            }
        }

        private void ShowEncounterChoices(InteractableNode node, EncounterSelection selection)
        {
            if (roomController.GetSnapshot().RunCompleted)
            {
                hud?.ClearChoices();
                hud?.ShowRunState(roomController.GetSnapshot());
                return;
            }

            if (roomController.TryGetResolvedEncounterChoice(selection, out var resolvedChoiceStableId))
            {
                hud?.ClearChoices();
                var resolution = new PrototypeNodeResolution(
                    node == null || node.Definition == null ? string.Empty : node.Definition.NodeId,
                    resolvedChoiceStableId,
                    $"already resolved: {resolvedChoiceStableId}",
                    false);
                hud?.ShowInteraction(node, selection, resolution);
                hud?.ShowRunState(roomController.GetSnapshot());
                return;
            }

            var views = roomController.BuildEncounterChoiceViews(selection);
            hud?.ShowChoices(selection.Encounter, views, choiceStableId =>
            {
                var resolution = roomController.ResolveEncounterChoice(node, selection.Encounter, choiceStableId);
                hud?.ShowInteraction(node, selection, resolution);
                hud?.ShowRunState(roomController.GetSnapshot());
            });
            hud?.ShowRunState(roomController.GetSnapshot());
        }
    }
}
