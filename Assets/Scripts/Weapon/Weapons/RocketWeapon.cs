using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RocketWeapon : MonoBehaviour, IWeapon, IChargeableWeapon
{
    private ProjectilePool _projectilePool;
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject rocketPrefab;

    [SerializeField] private float shotSpeedMultiplier = 3f;
    private CapsuleCollider _collider;
    [SerializeField] private GameObject _shootEffect;

    private int _ammo ;
    [SerializeField] private int maxAmmo = 2;
    
    private GameObject _currentRocket;

    private void Awake()
    {
        _projectilePool = new ProjectilePool(rocketPrefab);
        _collider = GetComponent<CapsuleCollider>();
        _ammo = maxAmmo;
        EventManager.OnTurnChanged += OnTurnChanged;
    }

    private void OnEnable()
    {
        ToggleVisibleRocket(true);
    }
    
    private void OnDestroy()
    {
        EventManager.OnTurnChanged -= OnTurnChanged;
    }

    public void Shoot()
    {
        Shoot(0);
    }
    public void Shoot(float shotCharge)
    {
        _currentRocket = _projectilePool.GetBullet();
        if(_currentRocket != null)
        {
            _currentRocket.GetComponent<RocketProjectile>().Init(this);
            _currentRocket.transform.position = firePoint.transform.position;
            _currentRocket.transform.rotation = firePoint.transform.rotation;
            _currentRocket.SetActive(true);
            _currentRocket.GetComponent<Rigidbody>().AddForce(-transform.forward * shotCharge * shotSpeedMultiplier, ForceMode.Impulse);
            _shootEffect.SetActive(true);
            _ammo--;
        }
    }

    public void SetChargeAnimation(bool active)
    {
        //No charge animation (yet?)
    }

    public void ToggleVisibleRocket(bool active)
    {
        var showRocket = active && _ammo > 0;
        firePoint.SetActive(showRocket);
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

    public void OnPickup(CharacterManager player)
    {
        
    }
    
    public bool CanShoot()
    {
        return (_ammo > 0 && (_currentRocket == null || _currentRocket.activeSelf == false));
    }
}

