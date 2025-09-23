using UnityEngine;
using UnityEngine.InputSystem;

namespace Snog.Player.PlayerMovement
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter playerCharacter;
        [SerializeField] private PlayerCamera playerCamera;
        [Space]
        [SerializeField] private CameraSpring cameraSpring;
        [SerializeField] private CameraLean cameraLean;

        private InputActions _inputActions;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            _inputActions = new InputActions();
            _inputActions.Enable();

            playerCharacter.Initialize();
            playerCamera.Initialize(playerCharacter.GetCameraTarget());

            cameraSpring.Initialize();
            cameraLean.Initialize();
        }

        private void OnDestroy()
        {
            _inputActions.Dispose();
        }

        private void Update()
        {
            var input = _inputActions.Player;
            var deltaTime = Time.deltaTime;

            // get camera input and update its rotation
            var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
            playerCamera.UpdateRotation(cameraInput);

            // get the info if crouch is toggle of hold
            var isCrouchToggle = playerCharacter.GetIsCrouchToggle();

            if (isCrouchToggle)
            {

                // get the character input and update it
                var characterInput = new CharacterInput
                {
                    Rotation    = cameraFollow.transform.rotation,
                    Move        = input.Move.ReadValue<Vector2>(),
                    Jump        = input.Jump.WasPerformedThisFrame(),
                    JumpSustain = input.Jump.IsPressed(),
                    Sprint      = input.Sprint.IsPressed(),
                    Crouch      = input.Crouch.WasPerformedThisFrame()
                        ? CrouchInput.Toggle
                        : CrouchInput.None
                };
                playerCharacter.UpdateInput(characterInput);
            }
            else
            {
                // get the character input and update it
                var characterInput = new CharacterInput
                {
                    Rotation    = cameraFollow.transform.rotation,
                    Move        = input.Move.ReadValue<Vector2>(),
                    Jump        = input.Jump.WasPerformedThisFrame(),
                    JumpSustain = input.Jump.IsPressed(),
                    Sprint      = input.Sprint.IsPressed(),
                    Crouch      = input.Crouch.IsPressed()
                        ? CrouchInput.Crouch
                        : CrouchInput.Uncrouch
                };
                playerCharacter.UpdateInput(characterInput);
            }
            playerCharacter.UpdateBody(deltaTime);

#if UNITY_EDITOR
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                if (Physics.Raycast(ray, out var hit))
                {
                    EditorTeleport(hit.point);
                }
            }
#endif
        }

        private void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            var cameraTarget = playerCharacter.GetCameraTarget();
            var state = playerCharacter.GetState();

            playerCamera.UpdatePosition(cameraTarget);
            cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
            cameraLean.UpdateLean
            (
                deltaTime,
                state.Stance is Stance.Slide,
                state.Acceleration,
                cameraTarget.up
            );
        }

        private void EditorTeleport(Vector3 position)
        {
            playerCharacter.SetPosition(position);
        }
    }
}