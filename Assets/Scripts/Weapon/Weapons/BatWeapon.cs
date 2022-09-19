using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BatWeapon : MonoBehaviour, IWeapon, IChargeableWeapon
{
    
    [SerializeField] private GameObject chargeBar;
    private UI_WeaponChargeBar _chargeBarScript;
    
    private float shotCharge = 0f;
    [SerializeField] private float maxCharge = 3f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    [SerializeField] private float chargeForceMultiplier = 3f;
    private bool _isCharging = false;

    private Animator _animator;

    private void Awake()
    {
        _chargeBarScript = GetComponent<UI_WeaponChargeBar>();
        _animator = GetComponent<Animator>();
        
    }
    private void Update()
    {
        if (_isCharging && shotCharge < maxCharge)
        {
            ChargeWeaponUp();
        }
    }
    public void ChargeShot(bool active)
    {
        _isCharging = active;
        _chargeBarScript.SetActive(true);
    }

    private void ChargeWeaponUp()
    {
        shotCharge += Time.deltaTime * chargeSpeedMultiplier;
        _chargeBarScript.UpdateChargeBar(shotCharge, maxCharge);
    }

    public void Shoot()
    {
        
        shotCharge = 0f;
        _chargeBarScript.SetActive(false);
    }

    private void Swing()
    {
        _animator.SetBool("IsSwinging", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
            
        if (collision.gameObject == gameObject) return;
        
        if (collision.gameObject.TryGetComponent(out PlayerCharacter player))
        {
            var  force = 2;
            var  upForce = 2;
            var contact = collision.contacts[0];
            player.gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, contact.point, 1f, upForce);
        }

    }

    public GameObject GetWeaponObject()
    {
        return gameObject;
    }
 //TODO Make swing work
}