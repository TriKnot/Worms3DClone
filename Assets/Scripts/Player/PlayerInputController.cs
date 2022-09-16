using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 20f)] private float moveSpeed = 5f;
    [SerializeField] GameObject weaponObject;
    private Weapon _weapon;
    private CharacterController _controller;
    private Vector2 _moveValue;
    

    

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _weapon = weaponObject.GetComponent<Weapon>();
    }

    private void FixedUpdate()
    {
        var moveVector = new Vector3(_moveValue.x,  0f, _moveValue.y);
        _controller.Move(moveVector * (moveSpeed * Time.fixedDeltaTime));
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveValue = context.ReadValue<Vector2>();
    }
    
   
    public void Shoot(InputAction.CallbackContext context)
    {

        if(context.started)
        {
            _weapon.ChargeShot(true);
        }
        else if(context.canceled)
        {
            _weapon.ChargeShot(false);
            _weapon.Shoot();
        }

    }

    public void Aim(InputAction.CallbackContext context)
    {
    }
    
    

   
}
