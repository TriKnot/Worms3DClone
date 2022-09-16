using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrenadeWeapon : MonoBehaviour, IWeapon
{
    public Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private GameObject chargeBar;
    private UI_WeaponChargeBar _chargeBarScript;

    private BulletPool _bulletPool;
    private float shotCharge = 0f;
    [SerializeField] private float maxCharge = 3f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    [SerializeField] private float shotSpeedMultiplier = 3f;
    private bool _isCharging = false;

    private void Awake()
    {
        _bulletPool = new BulletPool(bulletPrefab);
        _chargeBarScript = GetComponent<UI_WeaponChargeBar>();
    }

    private void Update()
    {
        if (_isCharging && shotCharge < maxCharge)
        {
            shotCharge += Time.deltaTime * chargeSpeedMultiplier;
            _chargeBarScript.UpdateChargeBar(shotCharge, maxCharge);
        }
    }

    public void ChargeShot(bool active)
    {
        _isCharging = active;
        _chargeBarScript.SetActive(true);
    }
    
    public void Shoot()
    {
        GameObject bullet = _bulletPool.GetBullet();
        
        if(bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().AddForce(transform.up * shotCharge * shotSpeedMultiplier, ForceMode.Impulse);
        }
        shotCharge = 0f;
        _chargeBarScript.SetActive(false);
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
