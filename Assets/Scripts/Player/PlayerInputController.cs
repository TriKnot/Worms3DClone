using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 20f)] private float moveSpeed = 5f;
    //private CharacterController _controller;
    private PlayerMovement _playerMovement;
    private Vector2 _moveValue;
    private PlayerCharacter player;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        player = GetComponent<PlayerCharacter>();
    }
    
    private void FixedUpdate()
    {
        if(_moveValue != Vector2.zero)
        {
            _playerMovement.Move(_moveValue);
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
            player.Inventory.ChangeWeapon();
        }
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        var activeWeapon = player.Inventory.GetActiveWeapon();
        if (activeWeapon == null) return;
        
        if (activeWeapon.GetWeaponObject().TryGetComponent(out IChargeableWeapon chargeableWeapon))
        {
            if (context.started)
            {
                chargeableWeapon.ChargeShot(true);
            }
            else if (context.canceled)
            {
                chargeableWeapon.ChargeShot(false);
            }
        }
        
        if (context.canceled)
            activeWeapon.Shoot();

    }

    
    

   
}
