
public class TakePrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to take";
}
