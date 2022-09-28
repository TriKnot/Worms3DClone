using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private float transformCenterYOffset = 0.5f;

    public Transform CameraFollow { get; private set; }
    private CameraController _cameraController;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private InputHandler _input;
    
    private Vector3 _rigidbodyPosition;
    private float _colliderYBounds;
    
    [Header("Input")]
    [SerializeField] private Vector3 playerMoveInput = Vector3.zero;
    private Vector3 _playerLookInput = Vector3.zero;
    private Vector3 _previousLookInput = Vector3.zero;
    private float _cameraPitch;
    
    [Header("Camera")]
    [SerializeField] private float playerLookInputLerpTime = 0.35f;

    [Header("Movement")] 
    [SerializeField] private float moveSpeedMultiplier = 30.0f;
    [SerializeField] private float notGroundedMoveSpeedMultiplier = 0.5f;
    [SerializeField] private float rotationSpeedMultiplier = 180.0f;
    [SerializeField] private float pitchSpeedMultiplier = 180.0f;
    [SerializeField] private float runSpeedMultiplier = 2.0f;

    [Header("Ground Check")] 
    [SerializeField] private bool isGrounded = true;
    [SerializeField] [Range(0.0f, 1.8f)] private float groundCheckRadiusMultiplier = 0.9f;
    [SerializeField] [Range(-0.95f, 2.05f)] private float groundCheckDistanceTolerance = 0.05f;
    [SerializeField] private float playerCenterToGroundDistance = 0.0f;
    private RaycastHit _groundCheckHit;

    [Header("Gravity")] 
    [SerializeField] private float gravityFallCurrent = 0.0f;
    [SerializeField] private float gravityFallMin = 0.0f;
    [SerializeField] private float gravityFallIncrementTime = 0.1f;
    [SerializeField] private float playerFallTimer = 0.0f;
    [SerializeField] private float gravityGrounded = -1.0f;
    [SerializeField] private float maxSlopeAngle = 45.0f;
    [SerializeField] [Range(0f, -2f)] private float slipSpeedModifier = -0.2f;

    [Header("Stairs")] 
    [SerializeField] [Range(0.0f, 1.0f)] private float maxStepHeight = 0.5f;
    [SerializeField] [Range(0.0f, 1.0f)] private float minStepDepth = 0.3f;
    [SerializeField] private float stairHeightPaddingMultiplier = 1.5f;
    [SerializeField] private bool isFirstStep = true;
    [SerializeField] private float firstStepVelocityDistanceMultiplier = 0.1f;
    [SerializeField] private bool playerIsAscendingStairs = false;
    [SerializeField] private bool playerIsDescendingStairs = false;
    [SerializeField] private float ascendingStairsMoveSpeedMultiplier = 0.35f;
    [SerializeField] private float descendingStairsMoveSpeedMultiplier = 0.7f;
    [SerializeField] private float maximumAngleOfApproachToAscend = 45.0f;
    private float _playerHalfHeightToGround = 0.0f;
    private float _maxAscendRayDistance = 0.0f;
    private float _maxDescendRayDistance = 0.0f;
    private int _numberOfStepsDetectRays = 0;
    private float _rayIncrementAmount = 0.0f;
    
    [Header("Jump")] 
    [SerializeField] private float initialJumpForceMultiplier = 750.0f;
    [SerializeField] private float continualJumpForceMultiplier = 0.5f;
    [SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float jumpTimeCounter = 0.0f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimeCounter = 0.0f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpBufferTimeCounter = 0.0f;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool jumpWasPressedLastFrame = false;
    [SerializeField] private float maxSlopeAngleToJump = 45.0f;


    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _input = GameManager.Instance.GetComponent<InputHandler>();
        _collider = GetComponent<CapsuleCollider>();
        _colliderYBounds = _collider.bounds.extents.y;

        CameraFollow = transform.Find("CameraFollowTarget").transform;
        _cameraController = GameManager.Instance.GetComponent<CameraController>();
        
        _maxAscendRayDistance = maxStepHeight / Mathf.Cos(maximumAngleOfApproachToAscend * Mathf.Deg2Rad);
        _maxDescendRayDistance = maxStepHeight / Mathf.Cos(80.0f * Mathf.Deg2Rad);
        
        _numberOfStepsDetectRays = Mathf.RoundToInt(((maxStepHeight * 100.0f) * 0.5f) + 1.0f);
        _rayIncrementAmount = maxStepHeight / _numberOfStepsDetectRays;
    }

    private void FixedUpdate()
    {
        _rigidbodyPosition = _rigidbody.position + Vector3.up * transformCenterYOffset;
        playerMoveInput = GetMoveInput();

        if(!_cameraController.UsingOrbitalCamera)
        {
            _playerLookInput = GetLookInput();
            PlayerLook();
            PitchCamera();
        }
        else if(_cameraController.UsingOrbitalCamera && playerMoveInput != Vector3.zero)
        {
            PlayerLookOrbitalCamera();
        }
        
        isGrounded = GroundCheck();
        
        playerMoveInput = PlayerMove();
        playerMoveInput = PlayerStairs();
        playerMoveInput = PlayerSlope();
        playerMoveInput = PlayerRun();
        
        playerMoveInput.y = PlayerFallGravity();
        playerMoveInput.y = PlayerJump();

        if(debugMode)
        {
            Debug.DrawRay(_rigidbodyPosition, _rigidbody.transform.TransformDirection(playerMoveInput), Color.red,
                0.5f);
        }      
        playerMoveInput *= _rigidbody.mass; // for dev testing
        
        _rigidbody.AddRelativeForce(playerMoveInput, ForceMode.Force);
    }

    private Vector3 GetLookInput()
    {
        _previousLookInput = _playerLookInput;
        _playerLookInput = new Vector3(_input.LookInput.x, (_input.InvertMouseY ? -_input.LookInput.y : _input.LookInput.y), 0f);
        return Vector3.Lerp(_previousLookInput, _playerLookInput * Time.deltaTime, playerLookInputLerpTime);
    }

    private void PlayerLook()
    {
        _rigidbody.rotation = Quaternion.Euler(0.0f, _rigidbody.rotation.eulerAngles.y + (_playerLookInput.x * rotationSpeedMultiplier), 0.0f);
    }

    private void PlayerLookOrbitalCamera()
    {
        _rigidbody.rotation = Quaternion.Euler(0.0f, _cameraController._activeCamera.transform.rotation.eulerAngles.y, 0.0f);
    }

    private void PitchCamera()
    {
        Vector3 rotationValues = CameraFollow.rotation.eulerAngles;
        _cameraPitch += _playerLookInput.y * pitchSpeedMultiplier;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
        
        CameraFollow.rotation = Quaternion.Euler(_cameraPitch, rotationValues.y, rotationValues.z);
    }
    
    private Vector3 GetMoveInput()
    {
        return new Vector3(_input.MoveInput.x, 0, _input.MoveInput.y);
    }

    private Vector3 PlayerMove()
    {
        return ((isGrounded) ? (playerMoveInput * moveSpeedMultiplier) : (playerMoveInput * (moveSpeedMultiplier * notGroundedMoveSpeedMultiplier)));
    }

    private bool GroundCheck()
    {
        float radius = _collider.radius * groundCheckRadiusMultiplier;
        Physics.SphereCast(_rigidbodyPosition, radius, Vector3.down, out _groundCheckHit);
        playerCenterToGroundDistance = _groundCheckHit.distance + radius;
        return((playerCenterToGroundDistance >= _colliderYBounds - groundCheckDistanceTolerance) &&
               (playerCenterToGroundDistance <= _colliderYBounds + groundCheckDistanceTolerance));
    }

    private Vector3 PlayerSlope()
    {
        Vector3 calculatedPlayerMovement = playerMoveInput;

        if (isGrounded && !playerIsAscendingStairs && !playerIsDescendingStairs)
        {
            Vector3 rigidBodyTransformUp = _rigidbody.transform.up;
            Vector3 localGroundCheckHitNormal = _rigidbody.transform.InverseTransformDirection(_groundCheckHit.normal);

            float groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, rigidBodyTransformUp);
            
            if (groundSlopeAngle == 0f)
            {
                if (_input.MoveIsPressed)
                {
                    RaycastHit rayHit;
                    float rayCalculatedRayHeight = _rigidbodyPosition.y - playerCenterToGroundDistance + groundCheckDistanceTolerance;
                    Vector3 rayOrigin = new Vector3(_rigidbodyPosition.x, rayCalculatedRayHeight, _rigidbodyPosition.z);
                    if(Physics.Raycast(rayOrigin, _rigidbody.transform.TransformDirection(calculatedPlayerMovement), out rayHit, 0.75f))
                    {
                        if (Vector3.Angle(rayHit.normal, _rigidbody.transform.up) > maxSlopeAngle)
                        {
                            calculatedPlayerMovement.y = -moveSpeedMultiplier;
                            //TODO Try for good value
                        }
                    }
#if UNITY_EDITOR
                    if(debugMode)
                        Debug.DrawRay(rayOrigin, _rigidbody.transform.TransformDirection(calculatedPlayerMovement), Color.green, 1f);
#endif                    
                }
                if (calculatedPlayerMovement.y == 0f)
                {
                    calculatedPlayerMovement.y = gravityGrounded;
                }
            }else
            {
                Quaternion slopeAngleRotation = Quaternion.FromToRotation(rigidBodyTransformUp, localGroundCheckHitNormal);
                calculatedPlayerMovement = slopeAngleRotation * calculatedPlayerMovement;

                float relativeSlopeAngle = Vector3.Angle(calculatedPlayerMovement, rigidBodyTransformUp) - 90f;
                calculatedPlayerMovement += calculatedPlayerMovement * (relativeSlopeAngle / 90f);

                if (groundSlopeAngle < maxSlopeAngle)
                {
                    if (_input.MoveIsPressed)
                    {
                        calculatedPlayerMovement.y += gravityGrounded;
                    }
                }
                else
                {
                    float calculatedSlopeGravity = groundSlopeAngle * slipSpeedModifier;
                    if(calculatedSlopeGravity < calculatedPlayerMovement.y)
                    {
                        calculatedPlayerMovement.y = calculatedSlopeGravity;
                    }
                }
            }
        }

        return calculatedPlayerMovement;
    }

    private float PlayerFallGravity()
    {
        float gravity = playerMoveInput.y;
        if (isGrounded || playerIsAscendingStairs || playerIsDescendingStairs)
        {
            gravityFallCurrent = gravityFallMin;
        }
        else
        {
            playerFallTimer -= Time.fixedDeltaTime;
            if (playerFallTimer < 0.0f)
            {
                float gravityFallMax = moveSpeedMultiplier * runSpeedMultiplier * 5.0f;
                float gravityFallIncrementAmount = (gravityFallMax - gravityFallMin) * 0.1f;
                if (gravityFallCurrent < gravityFallMax)
                {
                    gravityFallCurrent += gravityFallIncrementAmount;
                }
                playerFallTimer = gravityFallIncrementTime;
            }
            gravity = -gravityFallCurrent;
        }
        return gravity;
    }

    private Vector3 PlayerRun()
    {
        Vector3 calculatedPlayerRunSpeed = playerMoveInput;
        if (_input.MoveIsPressed && _input.RunIsPressed)
        {
            calculatedPlayerRunSpeed *= runSpeedMultiplier;
        }

        return calculatedPlayerRunSpeed;
    }

    
    private float PlayerJump()
    {
        float calculatedJumpInput = playerMoveInput.y;

        SetJumpTimeCounter();
        SetCoyoteTimeCounter();
        SetJumpBufferTimeCounter();

        if (jumpBufferTimeCounter > 0f && !isJumping && coyoteTimeCounter > 0f)
        {
            if(Vector3.Angle(_rigidbody.transform.up, _groundCheckHit.normal) < maxSlopeAngleToJump)
            {
                calculatedJumpInput = initialJumpForceMultiplier;
                isJumping = true;
                coyoteTimeCounter = 0f;
                jumpBufferTimeCounter = 0f;
            }
        }
        else if(_input.JumpIsPressed && isJumping && !isGrounded && jumpTimeCounter > 0f)
        {
            calculatedJumpInput = initialJumpForceMultiplier * continualJumpForceMultiplier;
        }
        else if(isJumping && isGrounded)
        {
            isJumping = false;
        }

        return calculatedJumpInput;
    }

    private void SetJumpTimeCounter()
    {
        if(isJumping && !isGrounded)
        {
            jumpTimeCounter -= Time.fixedDeltaTime;
        }
        else
        {
            jumpTimeCounter = jumpTime;
        }
    }
    
    private void SetCoyoteTimeCounter()
    {
        if(isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }
    
    private void SetJumpBufferTimeCounter()
    {
        if(!jumpWasPressedLastFrame && _input.JumpIsPressed)
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }
        else if(jumpBufferTimeCounter > 0f)
        {
            jumpBufferTimeCounter -= Time.fixedDeltaTime;
        }
        jumpWasPressedLastFrame = _input.JumpIsPressed;
    }
    
    private Vector3 PlayerStairs()
    {
        Vector3 calculatedStepInput = playerMoveInput;

        _playerHalfHeightToGround = _colliderYBounds;
        if(playerCenterToGroundDistance < _colliderYBounds)
        {
            _playerHalfHeightToGround = playerCenterToGroundDistance;
        }
        
        calculatedStepInput = AscendStairs(calculatedStepInput);
        if (!playerIsAscendingStairs)
        {
            calculatedStepInput = DescendStairs(calculatedStepInput);
        }

        return calculatedStepInput;
    }

    private Vector3 AscendStairs(Vector3 calculatedStepInput)
    {
        if (_input.MoveIsPressed)
        {
            float calculatedVelDistance = isFirstStep
                ? (_rigidbody.velocity.magnitude * firstStepVelocityDistanceMultiplier) + _collider.radius
                : _collider.radius;
            //TODO Make calculation above better/more efficient

            float ray = 0.0f;
            List<RaycastHit> rayHits = new List<RaycastHit>();
            for (int i = 1; i <= _numberOfStepsDetectRays; i++, ray += _rayIncrementAmount)
            {
                Vector3 rayLowerOrigin = new Vector3(_rigidbodyPosition.x,
                    (_rigidbodyPosition.y - _playerHalfHeightToGround) + ray, _rigidbodyPosition.z);
                RaycastHit hitLower;
                if(Physics.Raycast(rayLowerOrigin, _rigidbody.transform.TransformDirection(playerMoveInput), out hitLower, calculatedVelDistance + _maxAscendRayDistance))
                {
                   float stairSlopeAngle = Vector3.Angle(hitLower.normal, _rigidbody.transform.up);
                   if (stairSlopeAngle == 90.0f)
                   {
                       rayHits.Add(hitLower);
                   }
                }
            }

            if (rayHits.Count > 0)
            {
                Vector3 rayUpperOrigin = new Vector3(_rigidbodyPosition.x,
                    ((_rigidbodyPosition.y - _playerHalfHeightToGround) + maxStepHeight) + _rayIncrementAmount, _rigidbodyPosition.z);
                RaycastHit hitUpper;
                Physics.Raycast(rayUpperOrigin, _rigidbody.transform.TransformDirection(playerMoveInput), out hitUpper, calculatedVelDistance +
                    (_maxAscendRayDistance * 2.0f));
                if (!hitUpper.collider || hitUpper.distance - rayHits[0].distance > minStepDepth)
                {
                    if (Vector3.Angle(rayHits[0].normal, _rigidbody.transform.TransformDirection(-playerMoveInput)) <=
                        maximumAngleOfApproachToAscend)
                    {
                        if(debugMode)
                            Debug.DrawRay(rayUpperOrigin, _rigidbody.transform.TransformDirection(playerMoveInput), Color.yellow, 5.0f);

                        playerIsAscendingStairs = true;
                        Vector3 playerRelX = Vector3.Cross(playerMoveInput, Vector3.up);

                        if (isFirstStep)
                        {
                            calculatedStepInput = Quaternion.AngleAxis(45.0f, playerRelX) * calculatedStepInput;
                            isFirstStep = false;
                        }
                        else
                        {
                            float stairHeight = rayHits.Count * _rayIncrementAmount * stairHeightPaddingMultiplier;
                            
                            float avgDistance = 0.0f;
                            foreach (RaycastHit r in rayHits)
                            {
                                avgDistance += r.distance;
                            }
                            avgDistance /= rayHits.Count;

                            float tanAngle = Mathf.Atan2(stairHeight, avgDistance) * Mathf.Rad2Deg;
                            calculatedStepInput = Quaternion.AngleAxis(tanAngle, playerRelX) * calculatedStepInput;
                            calculatedStepInput *= ascendingStairsMoveSpeedMultiplier;
                        }
                    }
                    else
                    {
                        // more than 45* angle of approach
                        playerIsAscendingStairs = false;
                        isFirstStep = true;
                    }
                }
                else
                {
                    // top ray hit something
                    playerIsAscendingStairs = false;
                    isFirstStep = true;
                }
            }
            else
            {
                // no ray hits
                playerIsAscendingStairs = false;
                isFirstStep = true;
            }
        }
        else
        {
            // no move input
            playerIsAscendingStairs = false;
            isFirstStep = true;
        }

        return calculatedStepInput;
    }
    
    private Vector3 DescendStairs(Vector3 calculatedStepInput)
    {
        if (_input.MoveIsPressed)
        {
            float ray = 0.0f;
            List<RaycastHit> rayHits = new List<RaycastHit>();
            for (int i = 1; i <= _numberOfStepsDetectRays; i++, ray += _rayIncrementAmount)
            {
                Vector3 rayLowerOrigin = new Vector3(_rigidbodyPosition.x,
                    (_rigidbodyPosition.y - _playerHalfHeightToGround) + ray, _rigidbodyPosition.z);
                RaycastHit hitLower;
                if(Physics.Raycast(rayLowerOrigin, _rigidbody.transform.TransformDirection(playerMoveInput), out hitLower, _collider.radius + _maxAscendRayDistance))
                {
                   float stairSlopeAngle = Vector3.Angle(hitLower.normal, _rigidbody.transform.up);
                   if (stairSlopeAngle == 90.0f)
                   {
                       rayHits.Add(hitLower);
                   }
                }
            }

            if (rayHits.Count > 0)
            {
                Vector3 rayUpperOrigin = new Vector3(_rigidbodyPosition.x,
                    ((_rigidbodyPosition.y - _playerHalfHeightToGround) + maxStepHeight) + _rayIncrementAmount, _rigidbodyPosition.z);
                RaycastHit hitUpper;
                Physics.Raycast(rayUpperOrigin, _rigidbody.transform.TransformDirection(-playerMoveInput), out hitUpper, _collider.radius +
                    (_maxAscendRayDistance * 2.0f));
                if (!hitUpper.collider || hitUpper.distance - rayHits[0].distance > minStepDepth)
                {
                    if (!isGrounded && hitUpper.distance < _collider.radius + (_maxDescendRayDistance * 2.0f))
                    {
                        if(debugMode)
                            Debug.DrawRay(rayUpperOrigin, _rigidbody.transform.TransformDirection(playerMoveInput), Color.yellow, 5.0f);

                        playerIsDescendingStairs = true;
                        Vector3 playerRelX = Vector3.Cross(playerMoveInput, Vector3.up);
                        
                        float stairHeight = rayHits.Count * _rayIncrementAmount * stairHeightPaddingMultiplier;
                            
                        float avgDistance = 0.0f;
                        foreach (RaycastHit r in rayHits)
                        { 
                            avgDistance += r.distance;
                        }
                        avgDistance /= rayHits.Count;

                        float tanAngle = Mathf.Atan2(stairHeight, avgDistance) * Mathf.Rad2Deg;
                        calculatedStepInput = Quaternion.AngleAxis(tanAngle - 90.0f, playerRelX) * calculatedStepInput;
                        calculatedStepInput *= descendingStairsMoveSpeedMultiplier;
                        
                    }
                    else
                    {
                        // more than 45* angle of approach
                        playerIsDescendingStairs = false;
                    }
                }
                else
                {
                    // top ray hit something
                    playerIsDescendingStairs = false;
                }
            }
            else
            {
                // no ray hits
                playerIsDescendingStairs = false;
            }
        }
        else
        {
            // no move input
            playerIsDescendingStairs = false;
        }

        return calculatedStepInput;
    }

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * transformCenterYOffset, _collider.radius * groundCheckRadiusMultiplier);
        }
    }
}
