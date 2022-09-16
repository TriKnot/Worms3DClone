using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolOnImpact : MonoBehaviour
{
    private BulletPool _bulletPool;
    private Rigidbody _rigidbody;
    
    public void Init(BulletPool bulletPool)
    {
        _bulletPool = bulletPool;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == gameObject.layer)
        {
            return;
        }
        
        _rigidbody.Sleep();
        Invoke("DestroyMe", 0.2f);
    }

    private void DestroyMe()
    {
        _bulletPool.Return(gameObject);
    }
}
