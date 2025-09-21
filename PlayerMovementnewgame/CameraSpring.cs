using UnityEngine;

namespace Snog.Player.PlayerMovement
{
    public class CameraSpring : MonoBehaviour
    {
        [SerializeField] private bool isSpringOn = true;
        [SerializeField][Min(0.01f)] private float halfLife = 0.075f;
        [SerializeField] private float frequency = 18f;
        [Space]
        [SerializeField] private float angularDisplacement = 1f;
        [SerializeField] private float linearDisplacement = 0.05f;

        private Vector3 _springPosition;
        private Vector3 _springVelocity;

        public void Initialize()
        {
            _springPosition = transform.position;
            _springVelocity = Vector3.zero;
        }

        public void UpdateSpring(float deltaTime, Vector3 up)
        {
            if (isSpringOn)
            {
                transform.localPosition = Vector3.zero;

                Spring(ref _springPosition, ref _springVelocity, transform.position, halfLife, frequency, deltaTime);

                var localSpringPosition = _springPosition - transform.position;
                var springHeight = Vector3.Dot(localSpringPosition, up);

                transform.localEulerAngles = new Vector3(-springHeight * angularDisplacement, 0f, 0f);
                transform.localPosition = localSpringPosition * linearDisplacement;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _springPosition);
            Gizmos.DrawSphere(_springPosition, 0.1f);
        }

        // https://allenchou.net/2015/04/game-math-more-on-numeric-springing/
        private static void Spring(ref Vector3 current, ref Vector3 velocity, Vector3 target, float halfLife, float frequency, float timeStep)
        {
            var dampingRatio = -Mathf.Log(0.5f) / (frequency * halfLife);
            var f = 1.0f + 2.0f * timeStep * dampingRatio * frequency;
            var oo = frequency * frequency;
            var hoo = timeStep * oo;
            var hhoo = timeStep * hoo;
            var detInv = 1.0f / (f + hhoo);
            var detX = f * current + timeStep * velocity + hhoo * target;
            var detV = velocity + hoo * (target - current);
            current = detX * detInv;
            velocity = detV * detInv;
        }
    }
}