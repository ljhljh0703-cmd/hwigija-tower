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
        [SerializeField] private Text runStateText;

        private void Awake()
        {
            ShowFocus(null);
            if (interactionText != null)
            {
                interactionText.text = "방";
            }

            ShowRunState(default);
        }

        public void Configure(Text focus, Text interaction, Text runState = null)
        {
            focusText = focus;
            interactionText = interaction;
            runStateText = runState;
            ShowFocus(null);
            if (interactionText != null)
            {
                interactionText.text = "방";
            }

            ShowRunState(default);
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

        public void ShowRunState(PrototypeRunSnapshot snapshot)
        {
            if (runStateText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(snapshot.RunId))
            {
                runStateText.text = "run: -";
                return;
            }

            var state = snapshot.RunCompleted ? "complete" : "active";
            runStateText.text = $"run {state} | HP {snapshot.PlayerHp}/{snapshot.PlayerMaxHp} | ATK {snapshot.PlayerAttack} | gold {snapshot.Gold} | mental {snapshot.Mental} | glitch {snapshot.GlitchLevel} | affinity {snapshot.Affinity} | abilities {snapshot.AbilityCount}";
        }
    }
}
