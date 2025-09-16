public static class InteractionBehaviorFactory
{
    public static IInteractionBehavior GetBehavior(InteractableType type)
    {
        return type switch
        {
            InteractableType.Bed => new BedInteraction(),
            InteractableType.Door => new DoorInteraction(),
            InteractableType.Flashlight => new FlashlightInteraction(),
            InteractableType.FuseBox => new FuseBoxInteraction(),
            InteractableType.Generator => new GeneratorInteraction(),
            InteractableType.Heater => new HeaterInteraction(),
            InteractableType.LightSwitch => new LightSwitchInteraction(),
            InteractableType.Locker => new LockerInteraction(),
            InteractableType.Map => new MapInteraction(),
            InteractableType.Note => new NoteInteraction(),
            InteractableType.Radio => new RadioInteraction(),
            InteractableType.SupplyCabinet => new SupplyCabinetInteraction(),
            InteractableType.Toolbox => new ToolboxInteraction(),
            InteractableType.Take => new TakeInteraction(),
            _ => new DefaultInteraction()
        };
    }
}
