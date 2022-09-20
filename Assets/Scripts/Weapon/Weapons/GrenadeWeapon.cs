using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeWeapon : MonoBehaviour, IWeapon, IChargeableWeapon
{
    [SerializeField] private GameObject firePoint;
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] private GameObject chargeBar;
    private UI_WeaponChargeBar _chargeBarScript;

    private ProjectilePool _projectilePool;
    private float shotCharge = 0f;
    [SerializeField] private float maxCharge = 3f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    [SerializeField] private float shotSpeedMultiplier = 3f;
    private bool _isCharging = false;
    private CapsuleCollider _collider;
    [SerializeField] private GameObject _shootEffect;

    private int _ammo = 1;
    [SerializeField] private int maxAmmo = 2;

    private void Awake()
    {
        _projectilePool = new ProjectilePool(rocketPrefab);
        _chargeBarScript = GetComponent<UI_WeaponChargeBar>();
        _collider = GetComponent<CapsuleCollider>();
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
        GameObject bullet = _projectilePool.GetBullet();
        firePoint.SetActive(false);
        if(bullet != null)
        {
            bullet.transform.position = firePoint.transform.position;
            bullet.transform.rotation = firePoint.transform.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().AddForce(-transform.forward * shotCharge * shotSpeedMultiplier, ForceMode.Impulse);
        }
        shotCharge = 0f;
        _chargeBarScript.SetActive(false);
        _shootEffect.SetActive(true);
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
        print("Ammo: " + _ammo);
        return _ammo;
    }


    public void AddAmmo(int amount)
    {
        _ammo = _ammo + amount > maxAmmo ? maxAmmo : _ammo + amount;
    }
}

