using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }

    public void UpdateRotation(Transform flashlight)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, flashlight.rotation, speed * Time.deltaTime);
    }
}
