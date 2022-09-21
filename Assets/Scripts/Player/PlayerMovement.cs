using System;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private LayerMask _groundMask;
    private Rigidbody _rb;
    
    private Cinemachine3rdPersonFollow _cameraFollow;
    private Vector3 _startCameraOffset;

    private Quaternion _lookRotation;
    
    private bool _isActiveCharacter;
    public delegate void Moved(float movedDistance);
    public event Moved OnMoved;


    void Awake()
    {
        _groundMask = LayerMask.GetMask("Ground");
        _rb = GetComponent<Rigidbody>();
        EventManager.OnActiveCharacterChanged += SetActiveCharacter;
    }

    private void FixedUpdate()
    {
        var velocity = _rb.velocity;
        var movedDistance = new Vector3(velocity.x, 0, velocity.z).magnitude * Time.fixedDeltaTime;
        if(movedDistance > 0.01f)
            OnMoved?.Invoke(movedDistance);
    }

    private void LateUpdate()
    {
        if (_isActiveCharacter)
        {
            Rotate();
        }
        
    }

    private void SetActiveCharacter(PlayerCharacter character)
    {
        _isActiveCharacter = character == gameObject.GetComponent<PlayerCharacter>();
    }

    public void Move(Vector2 moveValue)
    {
        var vel = _rb.velocity;
        if (!IsGrounded())
        {
            moveValue *= 0f;
        }

        var t = transform;
        vel += t.forward * (moveValue.y * moveSpeed * Time.fixedDeltaTime) + t.right * (moveValue.x * moveSpeed * Time.fixedDeltaTime);
        _rb.velocity = vel;
    }


    private void Rotate()
    {
        transform.rotation = Quaternion.Euler(0, GameManager.MainCamera.transform.eulerAngles.y, 0);
    }
    
    public void Jump()
    {
        if (IsGrounded())
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.1f, _groundMask);
    }

}
