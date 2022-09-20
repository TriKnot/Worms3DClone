using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatWeapon : MonoBehaviour, IWeapon, IChargeableWeapon
{
    
    private UI_WeaponChargeBar _chargeBarScript;
    private CapsuleCollider _collider;
    private Animator _animator;
    private bool _isCharging = false;
    private float shotCharge = 0f;
    [SerializeField] private float maxCharge = 1f;

    private bool _canAttack = true;

    private void Awake()
    {
        _chargeBarScript = GetComponent<UI_WeaponChargeBar>();
        _collider = GetComponent<CapsuleCollider>();
        _collider.enabled = false;
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (_isCharging && shotCharge < maxCharge)
        {
            ChargeWeaponUp();
        }
    }

    public void Shoot()
    {
        _animator.SetTrigger("Attack");
        _collider.enabled = true;
        _isCharging = false;
        shotCharge = 0f;
        _canAttack = true;
    }

    public GameObject GetWeaponObject()
    {
        return gameObject;
    }

    public void SetCollider(bool state)
    {
        _collider.enabled = state;
    }

    public int GetAmmoCount()
    {
        return _canAttack ? 1 : 0;
    }

    public void AddAmmo(int amount)
    {
        //Ignores amount
    }

    public void ChargeShot(bool active)
    {
        _isCharging = active;
        _chargeBarScript.SetActive(true);
        _animator.SetBool("Charge", active);
    }

    private void ChargeWeaponUp()
    {
        shotCharge += Time.deltaTime;
        _chargeBarScript.UpdateChargeBar(shotCharge, maxCharge);
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
    }
}
