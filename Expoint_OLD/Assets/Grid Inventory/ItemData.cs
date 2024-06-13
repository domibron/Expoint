using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/DATA")]
public class ItemData : ScriptableObject
{
    public int width = 1;
    public int height = 1;

    public Sprite itemIcon;

    public ItemType ItemType = ItemType.any;

    public int SorageWidth = 1;
    public int SorageHeight = 1;
}
