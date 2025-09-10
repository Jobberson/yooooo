using UnityEngine;

namespace Snog.PlayerMovement
{
    public struct CameraInput
    {
        public Vector2 Look;
    }

    public class PlayerCamera : MonoBehaviour
    {
        public bool canLook = true;
        [SerializeField] private float sensitivity = 0.1f;
        private Vector3 _eulerAngles;

        public void Initialize(Transform target)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
            transform.eulerAngles = _eulerAngles = target.eulerAngles;
        }

        public void UpdateRotation(CameraInput input)
        {
            if (canLook)
            {
                _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
                transform.eulerAngles = _eulerAngles;
            }
        }

        public void UpdatePosition(Transform target)
        {
            transform.position = target.position;
        }
    }
}