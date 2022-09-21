using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatWeapon : MonoBehaviour, IWeapon, IChargeableWeapon
{
    
    private CapsuleCollider _collider;
    private Animator _animator;
    private bool _isCharging = false;
    private float shotCharge = 0f;
    [SerializeField] private float maxCharge = 1f;

    private bool _canAttack = true;
    
    public delegate void ChargeChanged(float shotCharge, float maxCharge);
    public event ChargeChanged OnChargeChanged;

    private void Awake()
    {
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
        _animator.SetBool("Charge", active);
    }

    private void ChargeWeaponUp()
    {
        shotCharge += Time.deltaTime;
        OnChargeChanged?.Invoke(shotCharge, maxCharge);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerCharacter player))
        {
            var direction = Vector3.ClampMagnitude(player.gameObject.transform.position - transform.position, 1f);
            player.gameObject.GetComponent<Rigidbody>().AddForce(direction * 10f, ForceMode.Impulse);
        }
    }
}
