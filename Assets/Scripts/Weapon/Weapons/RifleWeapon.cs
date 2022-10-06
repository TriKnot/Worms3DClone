using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleWeapon : MonoBehaviour, IWeapon
{
    
    public Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    private ProjectilePool _projectilePool;
    [SerializeField] private float shotSpeedMultiplier = 3f;
    private CapsuleCollider _collider;
    private InputHandler _inputHandler;
    
    private WeaponController _weaponController;


    private int _ammo = 1;
    [SerializeField] private int maxAmmo = 5;


    private void Awake()
    {
        _projectilePool = new ProjectilePool(bulletPrefab);
        _collider = GetComponent<CapsuleCollider>();
        EventManager.OnTurnChanged += OnTurnChanged;
        _ammo = maxAmmo;
    }

    private void OnDestroy()
    {
        EventManager.OnTurnChanged -= OnTurnChanged;
    }

    private void LateUpdate()
    {
        if(isActiveAndEnabled && _weaponController != null)
        {
            if (_inputHandler.AimIsPressed)
            {
                _weaponController._lineRenderer.enabled = true;
                _weaponController.AimStraight(firePoint.position, 0.1f);
            }else
            {
                _weaponController._lineRenderer.enabled = false;
            }
        }
    }

    public void Shoot()
    {
        GameObject bullet = _projectilePool.GetBullet();
        
        if(bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            var bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.useGravity = false;
            bulletRB.AddForce(transform.forward * shotSpeedMultiplier, ForceMode.Impulse);
            _ammo--;
            EventManager.InvokeAmmoChanged(_ammo);
        }    
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
        _weaponController = player.GetComponent<WeaponController>();
        _inputHandler = GameManager.Instance.GetComponent<InputHandler>();
    }

    public bool CanShoot()
    {
        return (_ammo > 0);
    }

}
