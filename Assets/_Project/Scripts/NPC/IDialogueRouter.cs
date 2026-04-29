namespace HwigiTower.NPC
{
    public interface IDialogueRouter
    {
        bool TryRoute(DialogueRequest request, out DialogueResponse response);
    }
}
