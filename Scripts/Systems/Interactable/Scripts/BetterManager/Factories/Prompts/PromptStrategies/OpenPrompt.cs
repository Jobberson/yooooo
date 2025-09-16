
public class OpenPrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to open";
}
