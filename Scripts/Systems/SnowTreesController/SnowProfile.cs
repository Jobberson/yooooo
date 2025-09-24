using UnityEngine;

[CreateAssetMenu(fileName = "SnowProfile", menuName = "Environment/Snow Profile")]
public class SnowProfile : ScriptableObject
{
    [Header("Snow Settings")]
    [Range(0f, 2f)] public float snowAmount = 0.0f;
    public Color snowColor = Color.white;
    [Range(0f, 0.5f)] public float brightnessReduction = 0.2f;
    [Range(0.1f, 6f)] public float maskThreshold = 4f;
    [Range(0f, 1f)] public float angleOverlay = 0.5f;
}
