
public class DefaultInteraction : IInteractionBehavior
{
    public void Execute(GameObject interactable)
    {
        Debug.Log("Interacting with Default");
        // Start dialog, change state, etc.
    }
}
