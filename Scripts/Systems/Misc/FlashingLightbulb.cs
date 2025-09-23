using UnityEngine;
using System.Collections;

public class FlashingLightbulb : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light bulbLight;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float flickerSpeed = 0.1f;
    [SerializeField] private float flickerChance = 0.3f;

    [Header("Spark Sound")]
    [SerializeField] private AudioSource sparkAudioSource;
    [SerializeField] private AudioClip sparkClip;
    [SerializeField] private float sparkThreshold = 0.8f;

    [Header("Material Flicker")]
    [SerializeField] private Renderer bulbRenderer;
    [SerializeField] private string emissionProperty = "_EmissionColor";
    [SerializeField] private Color emissionColor = Color.white;

    [Header("Trigger Zone")]
    [SerializeField] private Collider triggerZone;
    [SerializeField] private string playerTag = "Player";

    private float targetIntensity;
    private float currentVelocity;
    private Material bulbMaterial;
    private float lastIntensity;
    private Coroutine flickerCoroutine;

    private void Start()
    {
        if (bulbRenderer != null)
            bulbMaterial = bulbRenderer.material;

        targetIntensity = maxIntensity;
        lastIntensity = bulbLight.intensity;

        // Ensure light starts stable
        bulbLight.intensity = maxIntensity;
        if (bulbMaterial != null)
            bulbMaterial.SetColor(emissionProperty, emissionColor * maxIntensity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && flickerCoroutine == null)
        {
            flickerCoroutine = StartCoroutine(FlickerRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;

            // Reset to stable state
            bulbLight.intensity = maxIntensity;
            if (bulbMaterial != null)
                bulbMaterial.SetColor(emissionProperty, emissionColor * maxIntensity);
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (Random.value < flickerChance)
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            else
                targetIntensity = maxIntensity;

            float newIntensity = Mathf.SmoothDamp(bulbLight.intensity, targetIntensity, ref currentVelocity, flickerSpeed);
            bulbLight.intensity = newIntensity;

            if (bulbMaterial != null)
                bulbMaterial.SetColor(emissionProperty, emissionColor * newIntensity);

            if (lastIntensity - newIntensity > sparkThreshold && sparkClip != null && sparkAudioSource != null)
                sparkAudioSource.PlayOneShot(sparkClip);

            lastIntensity = newIntensity;

            yield return new WaitForSeconds(Random.Range(0.05f, 1.5f));
        }
    }
}
