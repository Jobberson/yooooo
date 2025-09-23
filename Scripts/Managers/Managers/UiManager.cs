using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance { get; private set; }

    [Header("Save UI")]
    [SerializeField] private CanvasGroup saveIndicatorGroup;
    [SerializeField] private TextMeshProUGUI saveIndicatorText = "Saving...";
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float displayDuration = 2f;

    // Add more as needed

    void Awake() 
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowSaveIndicator() 
    {
        StartCoroutine(FadeSaveIndicator());
    }
    
    private IEnumerator FadeSaveIndicator() 
    {
        saveIndicatorGroup.gameObject.SetActive(true);

        // Fade in
        float t = 0;
        while (t < fadeDuration) 
        {
            t += Time.deltaTime;
            saveIndicatorGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        // Fade out
        t = 0;
        while (t < fadeDuration) 
        {
            t += Time.deltaTime;
            saveIndicatorGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        saveIndicatorGroup.gameObject.SetActive(false);
    }
}
