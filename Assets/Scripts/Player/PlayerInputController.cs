using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private Vector2 _moveValue;
    private PlayerCharacter _player;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _player = GetComponent<PlayerCharacter>();
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
        }
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        var activeWeapon = _player.Inventory.GetActiveWeapon();
        
        if (activeWeapon == null || activeWeapon.GetAmmoCount() <= 0) return;
        
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
        {
            activeWeapon.Shoot();
        }
    }

    
    

   
}
