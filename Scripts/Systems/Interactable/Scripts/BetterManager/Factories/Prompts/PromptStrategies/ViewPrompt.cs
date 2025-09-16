
public class ViewPrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to view";
}
