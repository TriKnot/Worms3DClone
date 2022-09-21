using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RocketWeapon : MonoBehaviour, IWeapon, IChargeableWeapon
{
    [SerializeField] private GameObject firePoint;
    [SerializeField] GameObject rocketPrefab;

    private ProjectilePool _projectilePool;
    private float shotCharge = 0f;
    [SerializeField] private float maxCharge = 3f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    [SerializeField] private float shotSpeedMultiplier = 3f;
    private bool _isCharging = false;
    private CapsuleCollider _collider;
    [SerializeField] private GameObject _shootEffect;

    private int _ammo ;
    [SerializeField] private int maxAmmo = 2;

    private void Awake()
    {
        _projectilePool = new ProjectilePool(rocketPrefab);
        _collider = GetComponent<CapsuleCollider>();
        EventManager.OnTurnChanged += OnTurnChanged;
        EventManager.OnTogglePlayerControl += ResetWeapon;
        _ammo = maxAmmo;
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
        GameObject bullet = _projectilePool.GetBullet();
        firePoint.SetActive(false);
        if(bullet != null)
        {
            bullet.transform.position = firePoint.transform.position;
            bullet.transform.rotation = firePoint.transform.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().AddForce(-transform.forward * shotCharge * shotSpeedMultiplier, ForceMode.Impulse);
            _shootEffect.SetActive(true);
            _ammo--;
        }
        EventManager.InvokeAmmoChanged(_ammo);
    }
    
    private void ResetWeapon(bool toggle)
    {
        if (!toggle) return;
        shotCharge = 0f;
        EventManager.InvokeChargeChanged(maxCharge, shotCharge);
        if(_ammo > 0) firePoint.SetActive(true);
    }

    public void ChargeShot(bool active)
    {
        _isCharging = active;
    }

    private void ChargeWeaponUp()
    {
        shotCharge += Time.deltaTime * chargeSpeedMultiplier;
        EventManager.InvokeChargeChanged(maxCharge, shotCharge);

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
        return _ammo;
    }


    public void AddAmmo(int amount)
    {
        _ammo = _ammo + amount > maxAmmo ? maxAmmo : _ammo + amount;
    }
    
    public void OnTurnChanged()
    {
        _ammo = maxAmmo;
    }

}

