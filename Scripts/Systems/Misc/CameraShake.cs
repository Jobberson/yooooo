using UnityEngine;

public class CameraShake : MonoBehaviour 
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake() 
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        originalPosition = transform.localPosition;
    }

    public void Shake(float duration = 0.5f, float magnitude = 0.1f, float damping = 1f) 
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude, damping));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude, float damping) 
    {
        float elapsed = 0f;

        while (elapsed < duration) 
        {
            float damper = Mathf.Lerp(1f, 0f, elapsed / duration) * damping;
            Vector3 offset = Random.insideUnitSphere * magnitude * damper;
            transform.localPosition = originalPosition + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
