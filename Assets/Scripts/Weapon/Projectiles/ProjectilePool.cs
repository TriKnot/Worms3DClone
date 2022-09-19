using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool 
{
    private Stack<GameObject> _bulletPool;
    private GameObject _bulletPrefab;
    private Transform _poolParent;

    public ProjectilePool(GameObject bulletPrefab)
    {
        _poolParent = new GameObject("BulletPool").GetComponent<Transform>();
        _bulletPool = new Stack<GameObject>();
        _bulletPrefab = bulletPrefab;
    }
    
    public GameObject GetBullet()
    {
        var bullet = _bulletPool.Count > 0 ? _bulletPool.Pop() : Object.Instantiate<GameObject>(_bulletPrefab.gameObject, _poolParent);
        var poolOnImpact = bullet.gameObject.GetComponent<PoolOnImpact>();
        if (poolOnImpact == null)
        {
            poolOnImpact = bullet.gameObject.AddComponent<PoolOnImpact>();
            poolOnImpact.Init(this);
        }

        bullet.transform.parent = null;
        bullet.SetActive(true);
        return bullet;
    }

    public void Return(GameObject bullet)
    {
        bullet.transform.parent = _poolParent;
        bullet.SetActive(false);
        _bulletPool.Push(bullet);
    }
    
    
}
