using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    private Inventory _inventory;
    public LineRenderer _lineRenderer {get; private set; }
    [SerializeField] private LayerMask bulletLayer;
    
    private readonly float _maxWeaponCharge = 1;
    private float _weaponCharge = 0;
    private bool _isChargingWeapon = false;
    
    public delegate void WeaponChargeChanged(float maxCharge, float currentCharge);
    public event WeaponChargeChanged OnWeaponChargeChanged;

    private void Awake()
    {
        _inventory = GetComponent<CharacterManager>().Inventory;
        _lineRenderer = GetComponent<LineRenderer>();
        bulletLayer = LayerMask.GetMask("Bullet");
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
        var chargeDirection = 1;
        while(_isChargingWeapon)
        {
            _lineRenderer.enabled = true;
            if(_weaponCharge >= _maxWeaponCharge)
            {
                chargeDirection = -1;
            }
            else if(_weaponCharge <= 0)
            {
                chargeDirection = 1;
            }
            _weaponCharge += Time.deltaTime * chargeDirection;
            OnWeaponChargeChanged?.Invoke(_maxWeaponCharge, _weaponCharge);
            yield return new WaitForFixedUpdate();
        }    
        _lineRenderer.enabled = false;
    }

    public void AimCurved(Vector3 force, Vector3 initialPosition, float projectileMass, float lineWidth)
    {
        var stepCount = 100;
        force *= _weaponCharge;
        var vel = (force / projectileMass);
        var flightDuration = (2 * vel.y) / -Physics.gravity.y;
        float stepTime = flightDuration / stepCount;
        
        _lineRenderer.positionCount = stepCount;
        
        for(int i = 0; i < stepCount; i++)
        {
            var timePassed = stepTime * i;
            var height = vel.y * timePassed - (0.5f * -Physics.gravity.y * timePassed * timePassed);
            var pos = initialPosition + new Vector3(vel.x * timePassed, height, vel.z * timePassed);
            _lineRenderer.SetPosition(i, pos);
        }
    }

    public void AimStraight(Vector3 position, float lineWidth)
    {
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, position);
        Physics.Raycast(position, transform.forward, out RaycastHit hit, Mathf.Infinity, ~bulletLayer);
        _lineRenderer.SetPosition(1, hit.point);
    }

}
