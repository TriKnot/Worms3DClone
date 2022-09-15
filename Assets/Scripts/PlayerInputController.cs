using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 20f)] private float _moveSpeed = 5f;
    [SerializeField, Range(0.1f, 20f)] private float _jumpHeight = 5f;
    [SerializeField] GameObject _weaponObject;
    private Weapon _weapon;
    private CharacterController controller;
    private Vector2 _moveValue;
    
    private bool chargingShot = false;
    private float shotCharge = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        _weapon = _weaponObject.GetComponent<Weapon>();
    }

    private void FixedUpdate()
    {
        var moveVector = new Vector3(_moveValue.x,  0f, _moveValue.y);
        controller.Move(moveVector * (_moveSpeed * Time.fixedDeltaTime));

        if (chargingShot)
        {
            shotCharge += Time.fixedDeltaTime;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveValue = context.ReadValue<Vector2>();
    }
    
   
    public void Shoot(InputAction.CallbackContext context)
    {

        if(context.started)
        {
            chargingShot = true;
        }
        else if(context.canceled)
        {
            chargingShot = false;
            _weapon.Shoot(shotCharge * 5f);
            shotCharge = 0f;
        }

    }

   
}
