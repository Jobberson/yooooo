using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private bool canLook = true;
    [SerializeField] private float sensitivity = 0.1f;
    private Vector3 _eulerAngles;

    public void Initialize(Transform target)
    {
        //transform.SetPositionAndRotation(target.position, target.rotation);
        transform.eulerAngles = _eulerAngles = target.eulerAngles;
    }

    public void UpdateRotation(CameraInput input)
    {
        if (canLook)
        {
            _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
            _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -88f, 88f);

            transform.eulerAngles = _eulerAngles;
        }
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
}
