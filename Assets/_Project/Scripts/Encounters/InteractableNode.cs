using System;
using UnityEngine;

namespace HwigiTower.Encounters
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class InteractableNode : MonoBehaviour
    {
        [SerializeField] private PrototypeNodeDefinition definition;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Color idleColor = new Color(0.22f, 0.24f, 0.28f, 1f);
        [SerializeField] private Color highlightedColor = new Color(0.72f, 0.78f, 0.82f, 1f);

        public event Action<InteractableNode> OnInteracted;

        public PrototypeNodeDefinition Definition => definition;
        public string DisplayName => definition == null ? name : definition.DisplayName;

        public void Configure(PrototypeNodeDefinition nodeDefinition, SpriteRenderer nodeVisual)
        {
            definition = nodeDefinition;
            visual = nodeVisual;
            SetHighlighted(false);
        }

        private void Awake()
        {
            if (visual == null)
            {
                visual = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void OnValidate()
        {
            if (visual == null)
            {
                visual = GetComponentInChildren<SpriteRenderer>();
            }
        }

        public void SetHighlighted(bool highlighted)
        {
            if (visual != null)
            {
                visual.color = highlighted ? highlightedColor : idleColor;
            }
        }

        public void Interact()
        {
            OnInteracted?.Invoke(this);
        }
    }
}
