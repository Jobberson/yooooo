using UnityEngine;

public class AdvancedHeadbob : MonoBehaviour
{
    public PlayerCharacter playerCharacter;

    [Header("Walk Settings")]
    public float walkFrequency = 10f;
    public float walkHorizontalAmplitude = 0f;
    public float walkVerticalAmplitude = 0.008f;
    
    [Header("Run Settings")]
    public float runFrequency = 13f;
    public float runHorizontalAmplitude = 0.01f;
    public float runVerticalAmplitude = 0.012f;

    [Header("Crouch Settings")]
    public float crouchFrequency = 6f;
    public float crouchHorizontalAmplitude = 0f;
    public float crouchVerticalAmplitude = 0.004f;

    private float bobTimer = 0f;

    void LateUpdate()
    {
        var state = playerCharacter.GetState();
        float speed = state.Velocity.magnitude;

        Vector3 basePos = playerCharacter.GetCameraTargetLocalPosition();

        if (speed > 0.1f)
        {
            float frequency = 1f;
            float verticalAmp = 0f;
            float horizontalAmp = 0f;

            // Determine bob settings based on stance
            switch (state.Stance)
            {
                case Stance.Stand:
                    frequency = walkFrequency;
                    verticalAmp = walkVerticalAmplitude;
                    horizontalAmp = walkHorizontalAmplitude;
                    break;
                case Stance.Crouch:
                    frequency = crouchFrequency;
                    verticalAmp = crouchVerticalAmplitude;
                    horizontalAmp = crouchHorizontalAmplitude;
                    break;
                case Stance.Sprint:
                    frequency = runFrequency;
                    verticalAmp = runVerticalAmplitude;
                    horizontalAmp = runHorizontalAmplitude;
                    break;
            }

            bobTimer += Time.deltaTime * frequency;

            float bobOffset = Mathf.Sin(bobTimer) * verticalAmp;
            float lateralOffset = Mathf.Cos(bobTimer * 2f) * horizontalAmp;

            basePos.y += bobOffset;
            basePos.x += lateralOffset;
        }

        transform.localPosition = basePos;
    }
}
