using UnityEngine;

public class RadioFlashingLight : MonoBehaviour
{  
    public Light radioLight;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float maxIntensity = 2f;

    private void Update()
    {
        float intensity = Mathf.PingPong(Time.time * pulseSpeed, maxIntensity);
        radioLight.intensity = intensity;

    }

}
