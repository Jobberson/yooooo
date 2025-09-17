using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    [Header("Lighting")]
    public Light mainLight;
    public Color defaultColor;
    public Color horrorColor;

    [Header("Fog Settings")]
    public bool enableFog;
    public Color fogColor;
    public float fogDensity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyEnvironmentForState(StoryState state)
    {
        switch (state)
        {
            case StoryState.doneWithFuseBox:
                SetLighting(horrorColor);
                SetFog(true, fogColor, fogDensity);
                break;

            case StoryState.Act1Intro:
                SetLighting(defaultColor);
                SetFog(false, Color.clear, 0f);
                break;

            // Add more cases as needed
        }

        Debug.Log($"Environment updated for state: {state}");
    }

    private void SetLighting(Color color)
    {
        if (mainLight != null)
            mainLight.color = color;
    }

    private void SetFog(bool enabled, Color color, float density)
    {
        RenderSettings.fog = enabled;
        RenderSettings.fogColor = color;
        RenderSettings.fogDensity = density;
    }
}
