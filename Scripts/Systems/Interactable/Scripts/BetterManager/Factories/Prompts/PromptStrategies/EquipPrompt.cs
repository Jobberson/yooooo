
public class EquipPrompt : IInteractionPrompt
{
    public string GetPrompt(KeyCode key) => $"Press {key} to equip";
}
