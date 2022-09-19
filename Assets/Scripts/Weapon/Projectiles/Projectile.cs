using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent( out PlayerCharacter player))
        {
            player.Damage(damage);
        }
        
        Destroy(gameObject);
    }
}
