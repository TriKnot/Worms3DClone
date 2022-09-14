using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 20f)] private float _moveSpeed = 5f;
    [SerializeField, Range(0.1f, 20f)] private float _jumpHeight = 5f;
    private CharacterController controller;
    private Vector2 _moveValue;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        var moveVector = new Vector3(_moveValue.x,  0f, _moveValue.y);
        controller.Move(moveVector * (_moveSpeed * Time.fixedDeltaTime));
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveValue = context.ReadValue<Vector2>();
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controller.Move(Vector3.up * _jumpHeight);
        }
    }
    
    
}
