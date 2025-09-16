
public class DefaultPrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to interact";
}
