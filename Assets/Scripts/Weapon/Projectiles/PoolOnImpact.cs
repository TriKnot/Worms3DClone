using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolOnImpact : MonoBehaviour
{
    private ProjectilePool _projectilePool;
    private Rigidbody _rigidbody;
    
    public void Init(ProjectilePool projectilePool)
    {
        _projectilePool = projectilePool;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == gameObject.layer)
        {
            return;
        }
        
        _rigidbody.Sleep();
        Invoke(nameof(ReturnToPool), 0.2f);
    }

    public void ReturnToPool()
    {
        _projectilePool.Return(gameObject);
    }
}
