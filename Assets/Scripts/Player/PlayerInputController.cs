using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private OldPlayerMovement _oldPlayerMovement;
    private Vector2 _moveValue;
    private PlayerCharacter _player;
    private bool _aiming;

    private void Awake()
    {
        _oldPlayerMovement = GetComponent<OldPlayerMovement>();
        _player = GetComponent<PlayerCharacter>();
    }
    
    private void FixedUpdate()
    {
        if(_moveValue != Vector2.zero)
        {
            _oldPlayerMovement.Move(_moveValue);
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
            _oldPlayerMovement.Jump();
        }
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        _player.FireWeapon(context);
    }

    

   
}
