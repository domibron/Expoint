using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct StorageData
{
    private InventoryItem[,] _store_InventoryItemSlots;
    public List<ItemDataStore> Store_ItemDataStore; // for the items itself | pos of list, data relating to the object.

    // public List<ItemDataStore> Store_ItemDataStore; // will need to do this after base is done.

    public InventoryItem[,] Store_InventoryItemSlots
    {
        get
        {
            return _store_InventoryItemSlots;
        }

        set
        {
            _store_InventoryItemSlots = value;
        }
    }

    // public List<InventoryItem> ItemsInStore_
    // {
    //     get
    //     {
    //         return ItemsInStore;
    //     }
    //     set
    //     {
    //         ItemsInStore = value;
    //     }
    // }

    // public List<InventoryItem> convertItemsToItems(InventoryItem[,] multiArray)
    // {
    //     List<InventoryItem> localList = new List<InventoryItem>();

    //     int width = multiArray.GetLength(0), height = multiArray.GetLength(0);

    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int y = 0; y < height; y++)
    //         {
    //             if (localList.Count > 0)
    //             {
    //                 foreach (InventoryItem OtherItem in localList)
    //                 {
    //                     if (multiArray[x, y] == OtherItem) continue;
    //                 }
    //                 localList.Add(multiArray[x, y]);
    //             }
    //             else
    //             {
    //                 localList.Add(multiArray[x, y]);
    //             }
    //         }
    //     }

    //     return localList;
    // }
}
