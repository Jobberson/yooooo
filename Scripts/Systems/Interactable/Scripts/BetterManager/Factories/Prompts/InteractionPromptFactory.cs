public static class InteractionPromptFactory
{
    public static IInteractionPrompt GetPrompt(InteractableType type)
    {
        return type switch
        {
            InteractableType.Generator or
            InteractableType.Radio or
            InteractableType.Heater or
            InteractableType.LightSwitch => new UsePrompt(),

            InteractableType.SupplyCabinet or
            InteractableType.FuseBox or
            InteractableType.Door or
            InteractableType.Locker => new OpenPrompt(),

            InteractableType.Map => new ViewPrompt(),
            InteractableType.Toolbox => new TakePrompt(),
            InteractableType.Flashlight => new EquipPrompt(),
            InteractableType.Note => new ReadPrompt(),
            InteractableType.Bed => new SleepPrompt(),

            _ => new DefaultPrompt()
        };
    }
}
