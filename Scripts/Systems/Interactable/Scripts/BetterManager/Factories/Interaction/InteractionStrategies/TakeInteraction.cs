using UnityEngine;

public class PickupInteraction : IInteractionBehavior
{
    public void Execute(GameObject interactable)
    {
        var inventory = Object.FindAnyObjectByType<PlayerInventory>();
        if (inventory == null) return;

        if (!inventory.IsHoldingItem)
            inventory.PickUp(interactable);
        else
            Debug.Log("Player is already holding an item.");
    }
}
