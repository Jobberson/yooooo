using UnityEngine;
using System.Collections.Generic;

public class SnowController : MonoBehaviour
{
    [Header("Snow Profile Blending")]
    public SnowProfile fromProfile;
    public SnowProfile toProfile;
    [Range(0f, 1f)] public float blendFactor = 0f;

    [Tooltip("Update snow every frame (for dynamic blending)")]
    public bool continuousUpdate = false;

    [Tooltip("Tag used to identify NatureManufacture objects")]
    public string targetTag = "NatureObject";

    private List<Renderer> targetRenderers = new List<Renderer>();
    private MaterialPropertyBlock propertyBlock;

    // Shader property names
    private const string SNOW_AMOUNT = "_Snow_Amount";
    private const string SNOW_COLOR = "_SnowColor";
    private const string BRIGHTNESS_REDUCTION = "_SnowBrightnessReduction";
    private const string MASK_THRESHOLD = "_SnowMaskTreshold";
    private const string ANGLE_OVERLAY = "_SnowAngleOverlay";

    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        CacheRenderers();
        ApplyBlendedProfile();
    }

    void Update()
    {
        if (continuousUpdate)
        {
            ApplyBlendedProfile();
        }
    }

    void CacheRenderers()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        targetRenderers.Clear();

        foreach (GameObject obj in objects)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            targetRenderers.AddRange(renderers);
        }
    }

    void ApplyBlendedProfile()
    {
        if (fromProfile == null || toProfile == null) return;

        // Blend values
        float snowAmount = Mathf.Lerp(fromProfile.snowAmount, toProfile.snowAmount, blendFactor);
        Color snowColor = Color.Lerp(fromProfile.snowColor, toProfile.snowColor, blendFactor);
        float brightnessReduction = Mathf.Lerp(fromProfile.brightnessReduction, toProfile.brightnessReduction, blendFactor);
        float maskThreshold = Mathf.Lerp(fromProfile.maskThreshold, toProfile.maskThreshold, blendFactor);
        float angleOverlay = Mathf.Lerp(fromProfile.angleOverlay, toProfile.angleOverlay, blendFactor);

        foreach (Renderer renderer in targetRenderers)
        {
            int materialCount = renderer.sharedMaterials.Length;

            for (int i = 0; i < materialCount; i++)
            {
                Material mat = renderer.sharedMaterials[i];
                if (mat == null) continue;

                renderer.GetPropertyBlock(propertyBlock, i);

                if (mat.HasProperty(SNOW_AMOUNT))
                    propertyBlock.SetFloat(SNOW_AMOUNT, snowAmount);

                if (mat.HasProperty(SNOW_COLOR))
                    propertyBlock.SetColor(SNOW_COLOR, snowColor);

                if (mat.HasProperty(BRIGHTNESS_REDUCTION))
                    propertyBlock.SetFloat(BRIGHTNESS_REDUCTION, brightnessReduction);

                if (mat.HasProperty(MASK_THRESHOLD))
                    propertyBlock.SetFloat(MASK_THRESHOLD, maskThreshold);

                if (mat.HasProperty(ANGLE_OVERLAY))
                    propertyBlock.SetFloat(ANGLE_OVERLAY, angleOverlay);

                renderer.SetPropertyBlock(propertyBlock, i);
            }
        }
    }
}
