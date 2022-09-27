using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class InputHandler : MonoBehaviour
{
    // Declare references to the components we need
    InputActions _input;
    
    // Declare variables for movement
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public bool MoveIsPressed { get; private set; } = false;
    public Vector2 LookInput { get; private set; } = Vector2.zero;
    public bool InvertMouseY { get; private set; } = true;
    public bool RunIsPressed { get; private set; } = false;
    public bool JumpIsPressed { get; private set; } = false;
    public float ZoomCameraInput { get; private set; } = 0f;
    public bool InvertCameraZoom { get; private set; } = false;
    public bool CameraChangePressed { get; private set; } = false;
    
    private void OnEnable()
    {
        _input = new InputActions();
        _input.Player.Enable();
        
        // Subscribe to events
        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;
        
        _input.Player.Look.performed += OnLook;
        _input.Player.Look.canceled += OnLook;
        
        _input.Player.Jump.started += OnJump;
        _input.Player.Jump.canceled += OnJump;
        
        _input.Player.Run.started += OnRun;
        _input.Player.Run.canceled += OnRun;
        
        _input.Player.ZoomCamera.started += OnZoomCamera;
        _input.Player.ZoomCamera.canceled += OnZoomCamera;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        _input.Player.Move.performed -= OnMove;
        _input.Player.Move.canceled -= OnMove;
        
        _input.Player.Look.performed -= OnLook;
        _input.Player.Look.canceled -= OnLook;
        
        _input.Player.Jump.started -= OnJump;
        _input.Player.Jump.canceled -= OnJump;

        _input.Player.Run.started -= OnRun;
        _input.Player.Run.canceled -= OnRun;

        _input.Player.ZoomCamera.started -= OnZoomCamera;
        _input.Player.ZoomCamera.canceled -= OnZoomCamera;
        
        _input.Player.Disable();
    }

    private void Update()
    {
        CameraChangePressed = _input.Player.ChangeCamera.WasPressedThisFrame();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        MoveIsPressed = MoveInput != Vector2.zero;
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }
    
    private void OnRun(InputAction.CallbackContext context)
    {
        RunIsPressed = context.started;
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        JumpIsPressed = context.started;
    }
    
    private void OnZoomCamera(InputAction.CallbackContext context)
    {
        ZoomCameraInput = context.ReadValue<float>();
    }
}
