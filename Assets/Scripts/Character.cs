using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Team team;
    private LayerMask bulletLayer;

    private void Awake()
    {
        bulletLayer = LayerMask.NameToLayer("Bullet");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == bulletLayer)
        {
            Debug.Log("Bullet hit");
        }
    }
}
