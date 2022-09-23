using System;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private LayerMask _groundMask;
    private Rigidbody _rb;
    
    private StaminaSystem _staminaSystem;
    private bool _canMove = false;
    private Transform _cameraTransform;

    void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
        _rb = GetComponent<Rigidbody>();
        _staminaSystem = gameObject.GetComponent<PlayerCharacter>().StaminaSystem;
        _cameraTransform = Camera.main.transform;
        EventManager.OnActiveCharacterChanged += OnActiveCharacterChanged;
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

    private void LateUpdate()
    {
    }

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
        
        Vector3 desiredMoveDirection = forward * moveValue.y + right * moveValue.x;
        if(_canMove)
            _rb.AddForce(desiredMoveDirection * moveSpeed, ForceMode.Force);
        Rotate(desiredMoveDirection);
        
        
        if(_staminaSystem.Stamina <= 0)
        {
            _canMove = false;
        }
        _staminaSystem.ConsumeStamina(_rb.velocity.magnitude * Time.fixedDeltaTime);
    }


    private void Rotate(Vector3 velocity)
    {
        var targetRotation = Quaternion.LookRotation(velocity);
        transform.rotation = targetRotation;
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
        _canMove = GameManager.Instance.ActivePlayerCharacter.gameObject == gameObject;
    }

}
