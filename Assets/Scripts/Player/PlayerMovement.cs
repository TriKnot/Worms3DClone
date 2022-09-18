using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    
    private Cinemachine3rdPersonFollow _cameraFollow;
    private Vector3 _startCameraOffset;
    
    void Awake()
    {
        _groundCheckOffset = new Vector3(0, -1f, 0);
        _groundMask = LayerMask.GetMask("Ground");
        _rb = GetComponent<Rigidbody>();
        _playerBars = GetComponentInChildren<UI_PlayerBars>();
        _cameraFollow = GameManager.Instance.vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        _startCameraOffset = _cameraFollow.ShoulderOffset;
    }

    private void Start()
    {
        _cameraFollow.ShoulderOffset = _startCameraOffset;
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
        //Rotate Player
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotationValue.x * rotationSpeed, 0) * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
        
        //Move Camera Up/Down
        rotationValue.y = (_cameraFollow.ShoulderOffset.y < 0 && rotationValue.y < 0) ?  0 : rotationValue.y;
        rotationValue.y = (_cameraFollow.ShoulderOffset.y > 10 && rotationValue.y > 0) ?  0 : rotationValue.y;
       
        _cameraFollow.ShoulderOffset += new Vector3(0f,-rotationValue.y * Time.fixedDeltaTime, 0f);
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
