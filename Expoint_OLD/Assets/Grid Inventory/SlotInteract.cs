using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemSlot))]
public class SlotInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController _inventoryController;
    ItemSlot _itemSlot;

    void Awake()
    {
        _inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        _itemSlot = GetComponent<ItemSlot>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _inventoryController.SelectedItemSlot = _itemSlot;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inventoryController.SelectedItemSlot = null;
    }
}