using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemDataStore
{
    public ItemData itemData;

    // ! might be confusing or cause a overflow.
    public StorageData storageData;

    public int? LastPosX;
    public int? LastPosY;

    public bool Rotated;

}
