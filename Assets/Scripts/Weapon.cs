using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    [SerializeField] GameObject bulletPrefab;

    private BulletPool _bulletPool;

    private void Awake()
    {
        _bulletPool = new BulletPool(bulletPrefab);
    }
    
    public void Shoot(float shootForce)
    {
        GameObject bullet = _bulletPool.GetBullet();
        
        if(bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().AddForce(transform.up * shootForce, ForceMode.Impulse);
        }
    }
    
}
    // public void Shoot(float shootForce)
    // {
    //     Rigidbody bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    //
    //     var angle = Mathf.Abs(90 - transform.localEulerAngles.x) * 1/90;
    //     var direction = Vector3.up * angle + Vector3.forward;
    //     bullet.AddForce(transform.up * shootForce, ForceMode.Impulse);
    // }
