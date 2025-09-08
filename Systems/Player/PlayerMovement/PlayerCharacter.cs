using AwesomeAttributes;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.UI;

public enum CrouchInput
{
    None, Toggle, Crouch, Uncrouch
}

public enum Stance
{
    Stand, Crouch, Slide
}

public struct CharacterState
{
    public bool Grounded;
    public Stance Stance;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public CrouchInput Crouch;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [Title("Refs", null, true, true)]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    [Space]

    [Title("Movement", "Toggles", true, true)]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool isCrouchToggle = true;
    [Space]

    [Title(null, "Walk", true, true)]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float walkResponse = 25f;
    [Space]

    [Title(null, "Crouch", true, true)]
    [SerializeField] private float crouchSpeed = 7f;
    [SerializeField] private float crouchResponse = 20f;
    [Space]

    [Title("Air Movement", null, true, true)]
    [SerializeField] private float airSpeed = 15f;
    [SerializeField] private float airAcceleration = 70f;
    [Space]

    [Title("Jumping", "Toggle", true, true)]
    [SerializeField] private bool canJump = true;
    [Space]

    [Title(null, "Other Variables", true, true)]
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField][Range (0f, 1f)] private float jumpSustainGravity = 0.4f;
    [SerializeField] private float gravity = -90f;
    [Space]

    [Title("Sliding", "Toggle", true, true)]
    [SerializeField] private bool canSlide = true;
    [Space]

    [Title(null, "Other Variables", true, true)]
    [SerializeField] private float slideStartSpeed = 25f;
    [SerializeField] private float slideEndSpeed = 15f;
    [SerializeField] private float slideFriction = 0.8f;
    [SerializeField] private float slideSteerAcceleration = 5f;
    [SerializeField] private float slideGravity = -90f;
    [Space]

    [Title("Camera Height")]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchHeightResponse = 15f;
    [Range(0f, 1f)]
    [SerializeField] private float standCameraTargetHeight = 0.9f;
    [Range(0f, 1f)]
    [SerializeField] private float crouchCameraTargetHeight = 0.7f;

    // private variables
    private CharacterState _state;
    private CharacterState _lastState;
    private CharacterState _tempState;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedSustainedJump;
    private bool _requestedCrouch;
    private bool _requestedCrouchInAir;
    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;
    private Collider[] _uncrouchOverlapResults;

    public void Initialize()
    {
        _state.Stance = Stance.Stand;
        _lastState = _state;

        _uncrouchOverlapResults = new Collider[8];

        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
         _requestedRotation = input.Rotation;

        if(canMove)
        {
            // take the 2d inpUt and create a 3D movement vector on the xyz plane
            _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);

            // clamp the length to 1 to avoid moving faster diagonally in WASD input
            _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1);

            // orient the input so it's relative to the direction the player is facing
            _requestedMovement = input.Rotation * _requestedMovement;
        }

        // jump input
        if(canJump)
        {
            var wasRequestingJump = _requestedJump;
            _requestedJump = _requestedJump || input.Jump;
            if(_requestedJump && !wasRequestingJump)
                _timeSinceJumpRequest = 0f;

            _requestedSustainedJump = input.JumpSustain;
        }

        // crouch input
        if(canCrouch)
        {
            var wasRequestingCrouch = _requestedCrouch;
            _requestedCrouch = input.Crouch switch
            {
                // this is for toggle crouch
                CrouchInput.Toggle => !_requestedCrouch,
                CrouchInput.None => _requestedCrouch,

                // this is for holding crouch
                CrouchInput.Crouch => true, 
                CrouchInput.Uncrouch => false,

                _ => _requestedCrouch
            };

            if(_requestedCrouch && !wasRequestingCrouch)
                _requestedCrouchInAir = !_state.Grounded;
            else if(!_requestedCrouch && wasRequestingCrouch)
                _requestedCrouchInAir = false;
        }
    }

    public void UpdateBody(float deltaTime)
    {
        if(canCrouch)
        {
            var currentHeight = motor.Capsule.height;
            var normalizedHeight = currentHeight / standHeight;

            var cameraTargetHeight = currentHeight *
            (
                _state.Stance is Stance.Stand
                    ? standCameraTargetHeight
                    : crouchCameraTargetHeight
            );  
            var rootTargetScale = new Vector3(1f, normalizedHeight, 1f);

            cameraTarget.localPosition = Vector3.Lerp
            (
                a: cameraTarget.localPosition,
                b: new Vector3(0f, cameraTargetHeight, 0f),
                t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
            );
            
            root.localScale = Vector3.Lerp
            (
                a: root.localScale,
                b: rootTargetScale,
                t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
            );
        }
    }
    
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _state.Acceleration = Vector3.zero;

        // if on ground...
        if(motor.GroundingStatus.IsStableOnGround)
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;

            // ...snap the requested movement direction to the angle of the surface
            // the character is currently walking on.
            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            // ...start sliding
            if(canSlide)
            {
                var moving = groundedMovement.sqrMagnitude > 0f;
                var crouching = _state.Stance is Stance.Crouch;
                var wasStanding = _lastState.Stance is Stance.Stand;
                var wasInAir = !_lastState.Grounded;

                if(moving && crouching && (wasStanding || wasInAir))
                {
                    _state.Stance = Stance.Slide;

                    // when landing on stable ground the character motor projects the velocity onto a flat ground plane
                    // See: KinematicCharacterMotor.HandleVelocityProjection()
                    // this is normally good, because under normal circumstances the player shouldn't slide when landing on the ground
                    // in this case, we *want* the player to slide
                    // Reproject the last frames (falling) velocity onto the ground normal to slide

                    if(wasInAir)
                    {
                        currentVelocity = Vector3.ProjectOnPlane
                        (
                            vector: _lastState.Velocity,
                            planeNormal: motor.GroundingStatus.GroundNormal
                        );
                    } 
                    
                    var effectiveSlideStartSpeed = slideStartSpeed;
                    if(!_lastState.Grounded && !_requestedCrouchInAir)
                    {
                        effectiveSlideStartSpeed = 0f;
                        _requestedCrouchInAir = false;
                    }

                    var slideSpeed = Mathf.Max(effectiveSlideStartSpeed, currentVelocity.magnitude);
                    currentVelocity = motor.GetDirectionTangentToSurface
                    (
                        direction: currentVelocity,
                        surfaceNormal: motor.GroundingStatus.GroundNormal
                    ) * slideSpeed;
                }
            }

            // ...walk
            if(_state.Stance is Stance.Stand or Stance.Crouch)
            {
                // calculate the speed and responsiveness of movement based
                // on the character's stance
                var speed = _state.Stance is Stance.Stand
                    ? walkSpeed
                    : crouchSpeed;
                var response = _state.Stance is Stance.Stand
                    ? walkResponse
                    : crouchResponse;

                // and smoothly move along the ground in that direction
                var targetVelocity = groundedMovement * speed;
                var moveVelocity = Vector3.Lerp
                (
                    a: currentVelocity,
                    b: targetVelocity,
                    t: 1f - Mathf.Exp(-response * deltaTime)
                );
                _state.Acceleration = (moveVelocity - currentVelocity) / deltaTime;
                currentVelocity = moveVelocity;
            }
            // ...continue sliding
            else 
            if(canSlide)
            {
                // friction 
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);

                // slope 
                {
                    var force = Vector3.ProjectOnPlane
                    (
                        vector: -motor.CharacterUp,
                        planeNormal: motor.GroundingStatus.GroundNormal
                    ) * slideGravity;

                    currentVelocity -= force * deltaTime;
                }

                // steer
                {
                    // target velocity is the player's movement direction, at the current speed
                    var currentSpeed = currentVelocity.magnitude;
                    var targetVelocity = groundedMovement * currentSpeed;
                    var steerVelocity = currentVelocity;
                    var steerForce = (targetVelocity - steerVelocity) * slideSteerAcceleration * deltaTime;
                    
                    // add steer force, but clamp velocity so the slide speed doesn't increase due 
                    // to the direct movement input
                    steerVelocity += steerForce;
                    steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);

                    _state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;
                    currentVelocity = steerVelocity;
                }

                // stop
                if(currentVelocity.magnitude < slideEndSpeed)
                    _state.Stance = Stance.Crouch;
            }
        }
        // or else, in the air...
        else
        {
            _timeSinceUngrounded += deltaTime;

            // ...air move
            if(_requestedMovement.sqrMagnitude > 0f)
            {
                // Requested movement project onto movement place (magnitude preserved)
                var planarMovement = Vector3.ProjectOnPlane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp.normalized
                ) * _requestedMovement.magnitude;

                // Current velocity on movement plane
                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                // Calculate movement force
                var movementForce = planarMovement * airAcceleration * deltaTime;

                // if moving slower than the max air speed, treat movementForce as a simple steering force
                if(currentPlanarVelocity.magnitude < airSpeed)
                {
                    // Add it to the current planar velocity for a target velocity
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                    // Limit target velocity to air speed
                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                } 
                // otherwise, nerf the movement force when it is in the direction of the current planar velocity
                // to prevent accelerating further beyond the max air speed
                else if(Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
                {
                    // project movement force onto the plane whose normal is the current planar velocity
                    var constrainedMovementForce = Vector3.ProjectOnPlane
                    (
                        vector: movementForce,
                        planeNormal: currentPlanarVelocity.normalized
                    );

                    movementForce = constrainedMovementForce;
                }

                // prevent air-climbning steep slopes
                if(motor.GroundingStatus.FoundAnyGround)
                {
                    // if moving in the same direction as the resultant velocity...
                    if(Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                    {
                        // calculate obstruction normal
                        var obstructionNormal = Vector3.Cross
                        (
                            motor.CharacterUp,
                            Vector3.Cross
                            (
                                motor.CharacterUp,
                                motor.GroundingStatus.GroundNormal
                            )
                        ).normalized;

                        // project movement force onto obstruction plane
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);   
                    }
                }
                currentVelocity += movementForce;
            }

            // gravity
            var effectiveGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if(_requestedSustainedJump && verticalSpeed > 0f)
                effectiveGravity *= jumpSustainGravity;

            currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
        }

        // jumping
        if(_requestedJump && canJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if(grounded || canCoyoteJump) // only jump if on ground
            {
                _requestedJump = false;   // unset jump request
                _requestedCrouch = false; // and request the character uncrouches
                _requestedCrouchInAir = false;

                // unstick the player from the ground
                motor.ForceUnground(time: 0f);
                _ungroundedDueToJump = true;

                // set minimum velocity speed to the jump speed
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);

                // add the difference in current and target vertical speed to the character's velocity
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;
                
                // defer the jump request until coyote time has passed
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                _requestedJump = canJumpLater;
            }
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane
        (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );
        
        if(forward != Vector3.zero)
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }
    
    public void BeforeCharacterUpdate(float deltaTime)
    {
        _tempState = _state;

        // crouch
        if(_requestedCrouch && _state.Stance is Stance.Stand)
        {
            _state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
            );
        }
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        if(!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Slide)
            _state.Stance = Stance.Crouch;
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        // uncrouch
        if(!_requestedCrouch && _state.Stance is not Stance.Stand)
        {
            // tentatively "standup" the character capsule
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f
            );

            // then see if the capsule overlaps any colliders before atcually
            // allowing the character to standup.
            var pos = motor.TransientPosition;
            var rot = motor.TransientRotation;
            var mask = motor.CollidableLayers;
            if(motor.CharacterOverlap(pos, rot, _uncrouchOverlapResults, mask, QueryTriggerInteraction.Ignore) > 0)
            {
                _requestedCrouch = true;
                motor.SetCapsuleDimensions
                (
                    radius: motor.Capsule.radius,
                    height: standHeight,
                    yOffset: standHeight * 0.5f
                );
            }
            else
            {
                _state.Stance = Stance.Stand;
            }
        }

        // Update state to reflect relevant motor properties
        _state.Grounded = motor.GroundingStatus.IsStableOnGround;
        _state.Velocity = motor.Velocity;

        // And update the _lastState to store the characyer state snapshot taken at
        // the beginning of this character update
        _lastState = _tempState;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
    public bool IsColliderValidForCollisions(Collider coll) => true;
    public void OnDiscreteCollisionDetected(Collider hitCollider){}
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport){}
    public Transform GetCameraTarget() => cameraTarget;
    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _lastState;
    public bool GetIsCrouchToggle() => isCrouchToggle;
    public void SetPosition(Vector3 position, bool killVelocity = true)
    {
        motor.SetPosition(position);
        if(killVelocity)
            motor.BaseVelocity = Vector3.zero; 
    }
}
