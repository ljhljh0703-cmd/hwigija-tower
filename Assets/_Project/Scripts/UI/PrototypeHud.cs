using HwigiTower.Encounters;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        [SerializeField] private Text focusText;
        [SerializeField] private Text interactionText;

        private void Awake()
        {
            ShowFocus(null);
            if (interactionText != null)
            {
                interactionText.text = "방";
            }
        }

        public void Configure(Text focus, Text interaction)
        {
            focusText = focus;
            interactionText = interaction;
            ShowFocus(null);
            if (interactionText != null)
            {
                interactionText.text = "방";
            }
        }

        public void ShowFocus(InteractableNode node)
        {
            if (focusText == null)
            {
                return;
            }

            focusText.text = node == null ? "노드 없음" : $"노드: {node.DisplayName}";
        }

        public void ShowInteraction(InteractableNode node, EncounterSelection selection, PrototypeNodeResolution resolution)
        {
            if (interactionText == null || node == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(resolution.Message))
            {
                interactionText.text = resolution.Message;
                return;
            }

            if (selection.HasEncounter)
            {
                interactionText.text = $"조우: {selection.EncounterId}";
                return;
            }

            interactionText.text = node.Definition == null ? "진입" : node.Definition.PlaceholderOutcome;
        }
    }
}
