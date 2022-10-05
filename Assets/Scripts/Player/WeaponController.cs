using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    private InputHandler _inputHandler;
    private Inventory _inventory;
    
    private readonly float _maxWeaponCharge = 1;
    private float _weaponCharge = 0;
    private bool _isChargingWeapon = false;

    
    public delegate void WeaponChargeChanged(float maxCharge, float currentCharge);
    public event WeaponChargeChanged OnWeaponChargeChanged;

    private void Awake()
    {
        _inputHandler = GameManager.Instance.GetComponent<InputHandler>();
        _inventory = GetComponent<CharacterManager>().Inventory;
    }

    public void FireWeapon(InputAction.CallbackContext context)
    {
        IWeapon weapon = _inventory.GetActiveWeapon();

        //If weapon is chargeable
        if (_inventory.GetActiveWeaponObject().TryGetComponent(out IChargeableWeapon chargeableWeapon))
        {
            if (!_inventory.GetActiveWeaponObject().TryGetComponent(out IMeleeWeapon meleeWeapon) 
                && _inventory.GetActiveWeapon().GetAmmoCount() <= 0)
            {
                return;
            }
            if (context.started)
            {
                if(weapon.CanShoot())
                    _isChargingWeapon = true;
                StartCoroutine(ChargeWeaponUp());
            }else if (context.canceled && _isChargingWeapon)
            { 
                chargeableWeapon.Shoot(_weaponCharge);
                _isChargingWeapon = false;
                _weaponCharge = 0;
                OnWeaponChargeChanged?.Invoke(_maxWeaponCharge, _weaponCharge);
            }
            chargeableWeapon.SetChargeAnimation(_isChargingWeapon);
            return;
        }
        //If weapon is not chargeable
        if(context.started )
        {
            if(weapon.CanShoot())
                weapon.Shoot();
        }
    }
    
    private IEnumerator ChargeWeaponUp()
    {
        while(_isChargingWeapon)
        {
            _weaponCharge = Mathf.Min(_weaponCharge + Time.fixedDeltaTime, _maxWeaponCharge);
            OnWeaponChargeChanged?.Invoke(_maxWeaponCharge, _weaponCharge);
            yield return new WaitForFixedUpdate();
        }    
    }

    public void Aim()
    {
        
    }
}
