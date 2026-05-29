namespace HwigiTower.Combat
{
    public readonly struct CombatActionPreview
    {
        public CombatActionPreview(CombatAction action, string label, string previewText, bool usable)
        {
            Action = action;
            Label = label ?? string.Empty;
            PreviewText = previewText ?? string.Empty;
            Usable = usable;
        }

        public CombatAction Action { get; }
        public string Label { get; }
        public string PreviewText { get; }
        public bool Usable { get; }
        public bool HasPreview => !string.IsNullOrEmpty(PreviewText);
    }
}
