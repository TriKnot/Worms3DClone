using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Inventory 
{
    private List<GameObject> inventory;
    private int _activeWeaponIndex;
    
    public delegate void AmmoChanged(int ammo);
    public event AmmoChanged OnAmmoChanged;


    public Inventory()
    {
        inventory = new List<GameObject>();
        _activeWeaponIndex = 0;
    }

    public void ChangeWeapon()
    {
        GetActiveWeaponObject().SetActive(false); 
        _activeWeaponIndex++;
        _activeWeaponIndex %= inventory.Count;
        var newActiveWeapon = GetActiveWeapon();
        newActiveWeapon.GetWeaponObject().SetActive(true);
        UpdateAmmo();
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
    
    public void RemoveWeapon(IWeapon weapon)
    {
        if (inventory.Count == 0) return;
        var weaponObject = weapon.GetWeaponObject();
        if (inventory.Contains(weaponObject))
        {
            if (inventory.Count == 1)
            {
                inventory.Remove(weaponObject);
                _activeWeaponIndex = 0;
                return;
            }
            if (inventory.IndexOf(weaponObject) == _activeWeaponIndex)
            {
                ChangeWeapon();
                inventory.Remove(weaponObject);
            }
            else
            {
                inventory.Remove(weaponObject);
            }
        }
    }
    
        
    public void UpdateAmmo()
    {
        OnAmmoChanged?.Invoke(GetActiveWeapon().GetAmmoCount());
    }
}
