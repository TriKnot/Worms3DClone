using System;
using System.Collections;
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
    
    [Header("Camera")]
    [SerializeField] private float playerLookInputLerpTime = 0.35f;
    private Vector3 _playerMoveInput = Vector3.zero;
    private Vector3 _playerLookInput = Vector3.zero;
    private Vector3 _previousLookInput = Vector3.zero;
    private float _cameraPitch = 0f;

    [Header("Movement")] 
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float notGroundedMoveSpeedMultiplier = 0.5f;
    [SerializeField] private float rotationSpeedMultiplier = 180f;
    [SerializeField] private float pitchSpeedMultiplier = 180f;
    [SerializeField] private float runSpeedMultiplier = 2f;

    [Header("Ground Check")] 
    [SerializeField] private bool isGrounded = true;
    [SerializeField] [Range(0.0f, 1.8f)] private float groundCheckRadiusMultiplier = 0.9f;
    [SerializeField] [Range(-0.95f, 2.05f)] private float groundCheckDistance = 0.05f;
    private RaycastHit _groundCheckHit;

    [Header("Gravity")] [SerializeField] private float gravityFallCurrent = -100f;
    [SerializeField] private float gravityFallMin = -100f;
    [SerializeField] private float gravityFallMax = -500f;
    [SerializeField] [Range(-5.0f, -35.0f)] private float gravityFallIncrementAmount = -20f;
    [SerializeField] private float gravityFallIncrementTime = 0.1f;
    [SerializeField] private float playerFallTimer = 0f;
    [SerializeField] private float gravityGrounded = -1f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] [Range(0f, -2f)] private float slipSpeedModifier = -0.2f;

    [Header("Jump")] 
    [SerializeField] private float initialJumpForce = 750f;
    [SerializeField] private float continualJumpForceMultiplier = 0.5f;
    [SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float jumpTimeCounter = 0f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimeCounter = 0f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpBufferTimeCounter = 0f;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool jumpWasPressedLastFrame = false;
    [SerializeField] private float maxSlopeAngleToJump = 45f;

    

    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _input = GameManager.Instance.GetComponent<InputHandler>();
        _collider = GetComponent<CapsuleCollider>();
        
        CameraFollow = transform.Find("CameraFollowTarget").transform;
        _cameraController = GameManager.Instance.GetComponent<CameraController>();
    }

    private void FixedUpdate()
    {
        if(!_cameraController.UsingOrbitalCamera)
        {
            _playerLookInput = GetLookInput();
            PlayerLook();
            PitchCamera();
        }
        else if(_cameraController.UsingOrbitalCamera && _playerMoveInput != Vector3.zero)
        {
            PlayerLookOrbitalCamera();
        }
        
        _playerMoveInput = GetMoveInput();
        isGrounded = GroundCheck();
        
        _playerMoveInput = PlayerMove();
        _playerMoveInput = PlayerSlope();
        _playerMoveInput = PlayerRun();
        
        _playerMoveInput.y = PlayerFallGravity();
        _playerMoveInput.y = PlayerJump();
        
        _playerMoveInput *= _rigidbody.mass; // for dev testing
        
        _rigidbody.AddRelativeForce(_playerMoveInput, ForceMode.Force);
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
        return ((isGrounded) ? (_playerMoveInput * moveSpeed) : (_playerMoveInput * moveSpeed * notGroundedMoveSpeedMultiplier));
    }

    private bool GroundCheck()
    {
        float radius = _collider.radius * groundCheckRadiusMultiplier;
        float distance = _collider.bounds.extents.y + transformCenterYOffset - radius + groundCheckDistance;
        return Physics.SphereCast(transform.position + Vector3.up * transformCenterYOffset, radius, Vector3.down, out _groundCheckHit, distance);
    }

    private Vector3 PlayerSlope()
    {
        Vector3 calculatedPlayerMovement = _playerMoveInput;

        if (isGrounded)
        {
            Vector3 rigidBodyTransformUp = _rigidbody.transform.up;
            Vector3 localGroundCheckHitNormal = _rigidbody.transform.InverseTransformDirection(_groundCheckHit.normal);

            float groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, rigidBodyTransformUp);
            
            if (groundSlopeAngle == 0f)
            {
                if (_input.MoveIsPressed)
                {
                    RaycastHit rayHit;
                    float rayHeightFromGround = 0.1f;
                    Vector3 rigidbodyPosition = _rigidbody.position + (Vector3.up * transformCenterYOffset);
                    float rayCalculatedRayHeight = rigidbodyPosition.y - _collider.bounds.extents.y + rayHeightFromGround ;
                    Vector3 rayOrigin = new Vector3(rigidbodyPosition.x, rayCalculatedRayHeight, rigidbodyPosition.z);
                    if(Physics.Raycast(rayOrigin, _rigidbody.transform.TransformDirection(calculatedPlayerMovement), out rayHit, 0.75f))
                    {
                        if (Vector3.Angle(rayHit.normal, _rigidbody.transform.up) > maxSlopeAngle)
                        {
                            calculatedPlayerMovement.y = -moveSpeed;
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
#if UNITY_EDITOR
            if(debugMode)
                Debug.DrawRay(_rigidbody.position + (Vector3.up * transformCenterYOffset), _rigidbody.transform.TransformDirection(calculatedPlayerMovement),
                Color.red, 1f);
#endif
        }

        return calculatedPlayerMovement;
    }

    private float PlayerFallGravity()
    {
        float gravity = _playerMoveInput.y;
        if (isGrounded)
        {
            gravityFallCurrent = gravityFallMin;
        }
        else
        {
            playerFallTimer -= Time.fixedDeltaTime;
            if (playerFallTimer < 0.0f)
            {
                if (gravityFallCurrent > gravityFallMax)
                {
                    gravityFallCurrent += gravityFallIncrementAmount;
                }
                playerFallTimer = gravityFallIncrementTime;
            }

            gravity = gravityFallCurrent;
        }

        return gravity;
    }

    private Vector3 PlayerRun()
    {
        Vector3 calculatedPlayerRunSpeed = _playerMoveInput;
        if (_input.MoveIsPressed && _input.RunIsPressed)
        {
            calculatedPlayerRunSpeed *= runSpeedMultiplier;
        }

        return calculatedPlayerRunSpeed;
    }

    
    private float PlayerJump()
    {
        float calculatedJumpInput = _playerMoveInput.y;

        SetJumpTimeCounter();
        SetCoyoteTimeCounter();
        SetJumpBufferTimeCounter();

        if (jumpBufferTimeCounter > 0f && !isJumping && coyoteTimeCounter > 0f)
        {
            if(Vector3.Angle(_rigidbody.transform.up, _groundCheckHit.normal) < maxSlopeAngleToJump)
            {
                calculatedJumpInput = initialJumpForce;
                isJumping = true;
                coyoteTimeCounter = 0f;
                jumpBufferTimeCounter = 0f;
            }
        }
        else if(_input.JumpIsPressed && isJumping && !isGrounded && jumpTimeCounter > 0f)
        {
            calculatedJumpInput = initialJumpForce * continualJumpForceMultiplier;
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

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * transformCenterYOffset, _collider.radius * groundCheckRadiusMultiplier);
        }
    }
}
