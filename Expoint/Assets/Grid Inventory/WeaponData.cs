using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Weapon DATA")]
public class WeaponData : ItemData
{
    [Header("Weapon Stuff")]
    public float Damage = 1f;
    public float FireRate = 1f;

    // sets the item type automatically
    public void Reset() // look at this for more info https://forum.unity.com/threads/scriptableobjects-and-derived-classes-not-playing-nice-with-the-inspector.646873/
    {
        ItemType = ItemType.weapon;
    }

    // how to access the data.

    //((WeaponData)ItemInSlot.itemData).FireRate
}
