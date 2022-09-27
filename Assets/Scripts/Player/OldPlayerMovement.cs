using System;
using Cinemachine;
using UnityEngine;

public class OldPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private LayerMask _groundMask;
    private Rigidbody _rb;
    
    private StaminaSystem _staminaSystem;
    private bool _canMove = false;
    private Transform _cameraTransform;
    private CapsuleCollider _collider;

    void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
        _rb = GetComponent<Rigidbody>();
        _staminaSystem = gameObject.GetComponent<PlayerCharacter>().StaminaSystem;
        _cameraTransform = Camera.main.transform;
        EventManager.OnActiveCharacterChanged += OnActiveCharacterChanged;
        _collider = GetComponent<CapsuleCollider>();
    }

    private void OnDisable()
    {
        EventManager.OnActiveCharacterChanged -= OnActiveCharacterChanged;
    }

    private void FixedUpdate()
    {
        var velocity = _rb.velocity;
        var movedDistance = new Vector3(velocity.x, 0, velocity.z).magnitude * Time.fixedDeltaTime;
    }

    private Vector3 gizmoDirection;
    public void Move(Vector2 moveValue)
    {
        if (!IsGrounded())
        {
            moveValue *= 0.3f;
        }

        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        Vector3 desiredVelocity = forward * moveValue.y + right * moveValue.x;
        desiredVelocity.Set(desiredVelocity.x, _rb.velocity.y, desiredVelocity.z);

        // var canMove = CheckMoveableTerrain(transform.position, new Vector3(desiredVelocity.x, 0, desiredVelocity.z),
        //     desiredVelocity.magnitude);
        //     print(canMove);
        if(_canMove)
        {
            // _rb.velocity = desiredVelocity * moveSpeed;
            _rb.AddForce(desiredVelocity * moveSpeed);
        }        
        Rotate(desiredVelocity);
        
      
        if(_staminaSystem.Stamina <= 0)
        {
            _canMove = false;
        }
        _staminaSystem.ConsumeStamina(_rb.velocity.magnitude * Time.fixedDeltaTime);
    }
    
    [SerializeField] private float slopeRayHeight = 0.1f;
    [SerializeField] private float steepSlopeAngle = 0.1f;
    [SerializeField] private float slopeThreshold = 0.1f;
    private bool CheckMoveableTerrain(Vector3 position, Vector3 desiredDirection, float distance)
    {
        Ray myRay = new Ray(position, desiredDirection);
        RaycastHit hit;
        if(Physics.Raycast(myRay, out hit, distance, _groundMask))
        {
            float slopeAngle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, hit.normal);
            float radius = Mathf.Abs(slopeRayHeight / Mathf.Sin(slopeAngle));
            if (slopeAngle >= steepSlopeAngle * Mathf.Deg2Rad)
            {
                if(hit.distance - _collider.radius > Mathf.Abs(Mathf.Cos(slopeAngle) * radius) + slopeThreshold)
                {
                    return true;
                }

                return false;
            }
            return true;
        }

        return true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, gizmoDirection);
    }

    private void Rotate(Vector3 rotationDirection)
    {
        rotationDirection.Set(rotationDirection.x, 0, rotationDirection.z);
        transform.rotation = Quaternion.LookRotation(rotationDirection);
    }
    
    public void Jump()
    {
        if(!_canMove) return;
        if (IsGrounded())
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.1f, _groundMask);
    }
    
    private void OnActiveCharacterChanged()
    {
        _canMove = GameManager.Instance.ActiveCharacter.gameObject == gameObject;
    }

}
