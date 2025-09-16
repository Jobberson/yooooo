using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    private GameObject heldItem;
    public bool IsHoldingItem => heldItem != null;

    public void PickUp(GameObject item)
    {
        if (heldItem != null)
        {
            Debug.LogWarning("Already holding an item.");
            return;
        }

        heldItem = item;

        // Parent to hand and reset local transform
        heldItem.transform.SetParent(handTransform);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        // Optional: disable physics
        var rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        var collider = heldItem.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        Debug.Log($"Picked up {item.name}");
    }

    public void Drop()
    {
        if (heldItem == null) return;

        // Unparent and re-enable physics
        heldItem.transform.SetParent(null);
        heldItem.transform.position = handTransform.position + handTransform.forward;

        var rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        var collider = heldItem.GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        Debug.Log($"Dropped {heldItem.name}");
        heldItem = null;
    }

    public GameObject GetHeldItem() => heldItem;
}
