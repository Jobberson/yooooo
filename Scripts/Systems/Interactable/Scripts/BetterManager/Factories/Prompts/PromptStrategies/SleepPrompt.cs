
public class SleepPrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to sleep";
}
