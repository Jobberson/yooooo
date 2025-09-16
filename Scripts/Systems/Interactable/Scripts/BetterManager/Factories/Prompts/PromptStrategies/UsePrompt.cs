
public class UsePrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to use";
}
