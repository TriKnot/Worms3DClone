using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 20f)] private float moveSpeed = 5f;
    [SerializeField] GameObject weaponObject;
    private GrenadeWeapon _grenadeWeapon;
    //private CharacterController _controller;
    private PlayerMovement _playerMovement;
    private Vector2 _moveValue;
    private Vector2 _lookValue;
    

    

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _grenadeWeapon = weaponObject.GetComponent<GrenadeWeapon>();
    }
    
    private void FixedUpdate()
    {
        if(_moveValue != Vector2.zero)
        {
            _playerMovement.Move(_moveValue);
        }
        if(_lookValue != Vector2.zero)
        {
            _playerMovement.Rotate(_lookValue);
        }
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        _moveValue = context.ReadValue<Vector2>();
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            _playerMovement.Jump();
        }
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
    
        if(context.started)
        {
            
            _grenadeWeapon.ChargeShot(true);
        }
        else if(context.canceled)
        {
            _grenadeWeapon.ChargeShot(false);
            _grenadeWeapon.Shoot();
        }
    
    }
    
    public void Aim(InputAction.CallbackContext context)
    {
        _lookValue = context.ReadValue<Vector2>();
    }
    
    

   
}
