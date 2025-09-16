
public class ReadPrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to read";
}
