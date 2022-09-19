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

    private void Awake()
    {
        _projectilePool = new ProjectilePool(bulletPrefab);
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
            bulletRB.AddForce(transform.up * shotSpeedMultiplier, ForceMode.Impulse);
        }    
    }

    public GameObject GetWeaponObject()
    {
        return gameObject;
    }
}
