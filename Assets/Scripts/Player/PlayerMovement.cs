using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxMoveRange = 20f;
    [SerializeField] private float rotationSpeed = 5f;
    private float _movedDistance;
    private LayerMask _groundMask;
    private Vector3 _groundCheckOffset;
    private Rigidbody _rb;
    private UI_PlayerBars _playerBars;
    
    
    
    void Awake()
    {
        _groundCheckOffset = new Vector3(0, -1f, 0);
        _groundMask = LayerMask.GetMask("Ground");
        _rb = GetComponent<Rigidbody>();
        _playerBars = GetComponentInChildren<UI_PlayerBars>();
    }

    private void FixedUpdate()
    {
        var velocity = _rb.velocity;
        _movedDistance += new Vector3(velocity.x, 0, velocity.z).magnitude * Time.fixedDeltaTime;
        _playerBars.UpdateStaminaBar(maxMoveRange, _movedDistance);
    }

    public void Move(Vector2 moveValue)
    {
        if(_movedDistance > maxMoveRange)
        {
            return;
        }
        if (!IsGrounded())
        {
            moveValue *= 0.5f;
        }
        _rb.AddForce(transform.forward * (moveValue.y * moveSpeed));
        _rb.AddForce(transform.right * (moveValue.x * moveSpeed));
    }

    private float _rotationVelocity;
    public void Rotate(Vector2 rotationValue)
    {
        //transform.Rotate(0f, Mathf.SmoothDamp(transform.rotation.x, rotationValue.x, ref _rotationVelocity, rotationSpeedDamp ), 0f);
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotationValue.x * rotationSpeed, 0) * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
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
        return Physics.CheckSphere(transform.position + _groundCheckOffset, 0.1f, _groundMask);
    }

}
