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
    private float _movedDistance;
    private LayerMask _groundMask;
    private Vector3 _groundCheckOffset;
    private Rigidbody _rb;
    
    
    
    void Awake()
    {
        _groundCheckOffset = new Vector3(0, -1f, 0);
        _groundMask = LayerMask.GetMask("Ground");
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var velocity = _rb.velocity;
        _movedDistance += new Vector3(velocity.x, 0, velocity.z).magnitude * Time.fixedDeltaTime;
    }

    public void Move(Vector2 moveValue)
    {
        if(_movedDistance > maxMoveRange)
        {
            return;
        }
        var moveVector = new Vector3(moveValue.x,  0f, moveValue.y);
        if (!IsGrounded())
        {
            moveVector *= 0.5f;
        }
        _rb.AddForce(moveVector *  moveSpeed);
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
