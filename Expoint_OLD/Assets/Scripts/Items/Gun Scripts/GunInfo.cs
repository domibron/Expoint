using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damage;
    public float fireRate;
    public float reloadSpeed;
    public int maxAmmoInClip;
    public int maxAmmoInReserve;
}
