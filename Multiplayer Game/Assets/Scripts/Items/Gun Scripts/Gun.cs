using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public abstract override void UseMouse0();

    public abstract override void UseRKey();

    public GameObject bulletImpactPrefab;

    public GameObject tracerPrefab;
}
