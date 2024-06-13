using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
#nullable enable
	InventoryItem? ItemInSlot;

	// interfaces for differnt types of items.
	// ! IK this is not a good way to do it.
	private IWeaponSlot? _IWeaponSlot;
#nullable restore

	[SerializeField] ItemType SlotType = ItemType.any;

	[SerializeField] ItemGrid storageContainer;

	RectTransform _rectTransform;

	InventoryController _inventoryController;





	// StorageData Data;

	void Start()
	{
		_rectTransform = GetComponent<RectTransform>();
		//_storageContainer = GetComponentInChildren<ItemGrid>();
		storageContainer.GetComponent<RectTransform>().gameObject.SetActive(false);
		_inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
		StorageContainerRemove(storageContainer);

		if (SlotType == ItemType.weapon)
		{
			try // leave as is for now.
			{
				_IWeaponSlot = FindObjectOfType(typeof(IWeaponSlot)) as IWeaponSlot; // ??????
			}
			catch (NullReferenceException e)
			{
				Debug.LogErrorFormat("There needs to be a listening script with IWeaponSlot. var {0}", _IWeaponSlot);
				Debug.LogFormat("Details: {0}", e);
				// There is no script for the weapon slot!
			}
		}
	}

	public InventoryItem PickUpItem()
	{
		// Data = storageContainer.Data;
		InventoryItem toReturn = ItemInSlot;

		if (toReturn == null)
			return null;

		ItemInSlot.storageData = storageContainer._StorageData;

		foreach (Transform child in storageContainer.transform)
		{
			if (child != storageContainer.transform) Destroy(child.gameObject);
			// go.SetActive(false);
		}

		ItemInSlot = null;

		StorageContainerRemove(storageContainer);

		_inventoryController.AllItemsInInventory.Remove(toReturn.gameObject);

		return toReturn;
	}

	public bool PlaceItem(InventoryItem inventoryItem, ref InventoryItem overlapItem)
	{
		if (inventoryItem.itemData.ItemType != SlotType && SlotType != ItemType.any)
		{
			return false;
		}

		if (OverlapCheck(ref overlapItem) == false)
		{
			overlapItem = null;
			return false;
		}

		if (overlapItem != null)
		{
			ItemInSlot = null;
		}

		// this is here, explained in place item func
		ItemInSlot = inventoryItem;

		// TODO redo please
		if (SlotType == ItemType.backpack || SlotType == ItemType.armor)
		{
			StorageContainerCreate(storageContainer);
		}

		PlaceItem(inventoryItem);

		_inventoryController.AllItemsInInventory.Add(inventoryItem.gameObject);

		return true;
	}

	InventoryItem overFlowItem;

	private void GenerateItems()
	{
		if (storageContainer._StorageData.Store_ItemDataStore.Count <= 0)
		{
			//print("empt");
			return;
		}

		List<ItemDataStore> localList = storageContainer._StorageData.Store_ItemDataStore;

		//print(localList.Count);

		// did this as when you add items to the grid, it changes the store data so conflics occur.
		// but, its chaches twice now. :skull:
		// List<ItemData> localChacheOfItemData = data.Store_ItemData;

		// if (localChacheOfItemData.Count <= 0) { return; }

		// print(localChacheOfItemData.Count);

		// foreach (ItemData itemData in localChacheOfItemData.ToList())
		// {
		//     print(itemData.name);

		//     InventoryItem item = _inventoryController.CreateItem(itemData, storageContainer.transform);


		//     if (item.storageData.LastPosX == -1 || item.itemDataStore.LastPosY == -1)
		//     {
		//         print($"{item.itemDataStore.LastPosX}, {item.itemDataStore.LastPosY}");
		//         storageContainer.PlaceItem(item, (int)item.itemDataStore.LastPosX, (int)item.itemDataStore.LastPosY);
		//     }
		//     else
		//     {
		//         print("UH OH");
		//     }

		//     if (overFlowItem != null)
		//     {
		//         // _inventoryController.
		//     }
		// }

		// redo

		foreach (ItemDataStore item in localList.ToList())
		{
			InventoryItem theItem = _inventoryController.CreateItem(item.itemData, storageContainer.transform);

			if (item.LastPosX.HasValue && item.LastPosY.HasValue)
			{
				//print($"{item.LastPosX}, {item.LastPosY}");
				storageContainer.PlaceItemForGeneratingItems(theItem, (int)item.LastPosX, (int)item.LastPosY);

			}
			else
			{
				print("UH OH");
			}
		}


	}

	private void StorageContainerCreate(ItemGrid storageContainer)
	{
		storageContainer.GetComponent<RectTransform>().gameObject.SetActive(true);
		storageContainer.ChangeSize(ItemInSlot.itemData.SorageWidth, ItemInSlot.itemData.SorageHeight);

		if (SlotType == ItemType.backpack || SlotType == ItemType.armor)
		{
			storageContainer.IsAStorageItem = true;
			//storageContainer._StorageData = new StorageData();
		}
		else
		{
			storageContainer.IsAStorageItem = false;
		}

		_inventoryController.AllItemGrids.Add(storageContainer);
	}

	private void StorageContainerRemove(ItemGrid storageContainer)
	{

		storageContainer.ChangeSize(1, 1);
		_inventoryController.AllItemGrids.Remove(storageContainer);

		storageContainer.IsAStorageItem = false;
		storageContainer.ResetLocalStorageData();


		storageContainer.GetComponent<RectTransform>().gameObject.SetActive(false);
	}

	public void PlaceItem(InventoryItem inventoryItem)
	{
		RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
		rectTransform.SetParent(this._rectTransform);

		// removed because issues with gernerating items and storage size creation.
		//ItemInSlot = inventoryItem;


		inventoryItem.OnGridPositionX = 1;
		inventoryItem.OnGridPositionY = 1;
		Vector2 position = CalculateCenter(inventoryItem);

		rectTransform.localPosition = position;

		// storageContainer.Data = Data;
		// if (ItemInSlot.itemData.Data)
		// {
		//     ItemInSlot.itemData.Data = new StorageData();
		// }

		if (ItemInSlot.itemData.ItemType == ItemType.backpack || ItemInSlot.itemData.ItemType == ItemType.armor)
		{
			storageContainer.SetLocalStorageData(ItemInSlot.storageData, ItemInSlot.itemData);

			GenerateItems();
		}


	}

	internal InventoryItem GetItem()
	{
		return ItemInSlot;
	}

	public Vector2 CalculateCenter(InventoryItem inventoryItem)
	{
		Vector2 vector2 = new Vector2();

		vector2.x = inventoryItem.WIDTH / 2;
		vector2.y = inventoryItem.HEIGHT / 2;

		return vector2;
	}

	public Vector2 CalculateCenter()
	{
		Vector2 vector2 = new Vector2();

		vector2.x = _rectTransform.sizeDelta.x / 2;
		vector2.y = _rectTransform.sizeDelta.y / 2;

		return vector2;
	}

	private bool OverlapCheck(ref InventoryItem overlapItem)
	{
		if (ItemInSlot != null)
		{
			if (overlapItem == null)
				overlapItem = ItemInSlot;
			else
			{
				if (overlapItem != ItemInSlot)
					return false;
			}
		}
		return true;
	}

	// private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
	// {
	//     for (int x = 0; x < width; x++)
	//     {
	//         for (int y = 0; y < height; y++)
	//         {
	//             if (inventoryItemSlots[posX + x, posY + y] != null)
	//             {
	//                 if (overlapItem == null)
	//                     overlapItem = inventoryItemSlots[posX + x, posY + y];
	//                 else
	//                 {
	//                     if (overlapItem != inventoryItemSlots[posX + x, posY + y])
	//                         return false;
	//                 }
	//             }
	//         }
	//     }

	//     return true;
	// }

	// Vector2 positionOnTheGrid = new Vector2();
	// Vector2Int tileGridPosition = new Vector2Int();

	internal Vector2 GetSlotPos(Vector2 mousePosition)
	{
		// positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
		// positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

		// tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
		// tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

		// return tileGridPosition;

		return CalculateCenter();

	}
}


