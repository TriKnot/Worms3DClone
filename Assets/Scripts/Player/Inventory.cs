using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Inventory 
{
    private List<GameObject> inventory;
    private int _activeWeaponIndex;

    public Inventory()
    {
        inventory = new List<GameObject>();
        _activeWeaponIndex = 0;
    }

    public void ChangeWeapon()
    {
        GetActiveWeapon().GetWeaponObject().SetActive(false);
        _activeWeaponIndex++;
        _activeWeaponIndex %= inventory.Count;
        var newActiveWeapon = GetActiveWeapon();
        newActiveWeapon.GetWeaponObject().SetActive(true);
        EventManager.InvokeAmmoChanged(newActiveWeapon.GetAmmoCount());
    }

    public IWeapon GetActiveWeapon()
    {
        return inventory[_activeWeaponIndex].GetComponent<IWeapon>();
    }
    
    public GameObject GetActiveWeaponObject()
    {
        return inventory[_activeWeaponIndex];
    }

    public void AddWeapon(GameObject weapon)
    {
        if(inventory.Count > 0) weapon.SetActive(false);
        inventory.Add(weapon);
        ChangeWeapon();
    }
    
    //TODO add remove weapon
    //TODO set active weapon to new weapon on pickup
    
}
