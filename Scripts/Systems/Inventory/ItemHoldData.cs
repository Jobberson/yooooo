using UnityEngine;

[CreateAssetMenu(fileName = "NewItemHoldData", menuName = "Inventory/Item Hold Data")]
public class ItemHoldData : ScriptableObject
{
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scale = Vector3.one;
}
