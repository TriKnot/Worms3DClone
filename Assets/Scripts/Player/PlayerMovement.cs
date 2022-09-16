using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private LayerMask groundMask;
    private Vector3 groundCheckOffset;
    private Rigidbody rb;
    void Start()
    {
        groundCheckOffset = new Vector3(0, -1f, 0);
        groundMask = LayerMask.GetMask("Ground");
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 _moveValue)
    {
        var moveVector = new Vector3(_moveValue.x,  0f, _moveValue.y);
        if (!IsGrounded())
        {
            moveVector *= 0.5f;
        }
        rb.AddForce(moveVector *  moveSpeed);
    }
    
    public void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position + groundCheckOffset, 0.1f, groundMask);
    }

}
