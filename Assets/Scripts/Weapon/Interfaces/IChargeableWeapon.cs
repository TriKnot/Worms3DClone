using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeableWeapon
{
    /// <summary>
    /// Pass true when starting and false when ending charge
    /// </summary>
    /// <param name="active"></param>
    void ChargeShot(bool active);

}
