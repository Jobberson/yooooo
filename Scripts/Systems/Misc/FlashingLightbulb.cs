using UnityEngine;

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
    [SerializeField] private float sparkThreshold = 0.8f; // sudden drop threshold
    [Header("Material Flicker")]
    [SerializeField] private Renderer bulbRenderer;
    [SerializeField] private string emissionProperty = "_EmissionColor";
    [SerializeField] private Color emissionColor = Color.white;

    [Header("Trigger Conditions")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool requireStoryEvent = false;
    [SerializeField] private StoryState requiredState = StoryState.afterGenerator;

    private float targetIntensity;
    private float currentVelocity;
    private bool isPlayerNearby = false;
    private bool isFlickering = false;
    private Material bulbMaterial;
    private float lastIntensity;

    private void Start()
    {
        bulbMaterial = bulbRenderer.material;
        targetIntensity = maxIntensity;
        lastIntensity = bulbLight.intensity;
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (CanFlicker())
            {
                isFlickering = true;

                // Random flicker
                if (Random.value < flickerChance)
                {
                    targetIntensity = Random.Range(minIntensity, maxIntensity);
                }
                else
                {
                    targetIntensity = maxIntensity;
                }

                // Smooth transition
                float newIntensity = Mathf.SmoothDamp(bulbLight.intensity, targetIntensity, ref currentVelocity, flickerSpeed);
                bulbLight.intensity = newIntensity;

                // Emission flicker
                bulbMaterial.SetColor(emissionProperty, emissionColor * newIntensity);

                // Spark sound on sudden drop
                if (lastIntensity - newIntensity > sparkThreshold && sparkClip != null)
                {
                    sparkAudioSource.PlayOneShot(sparkClip);
                }

                lastIntensity = newIntensity;
            }
            else
            {
                isFlickering = false;
                bulbLight.intensity = maxIntensity;
                bulbMaterial.SetColor(emissionProperty, emissionColor * maxIntensity);
            }

            yield return new WaitForSeconds(Random.Range(0.05f, 1.5f));
        }
    }

    private bool CanFlicker()
    {
        if (requireStoryEvent && !StoryManager.Instance.IsAtState(requiredState))
            return false;

        return isPlayerNearby || !requireStoryEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = false;
        }
    }



    /// Eye Tracking Portraits
    /// Shadow Entity Tracker
    /// Heartbeat Proximity System
    /// Ambient Audio Zones
    /// Compass/Map Distortion
    /// Wall Breathing Effect
    /// Sudden Silence Zones






//     [SerializeField] private Transform player;
// [SerializeField] private float activationDistance = 5f;

// private bool IsPlayerClose()
// {
//     float sqrDistance = (player.position - transform.position).sqrMagnitude;
//     return sqrDistance <= activationDistance * activationDistance;
// }

}
