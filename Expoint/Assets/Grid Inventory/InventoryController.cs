using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour, IMouseLeftClickPress
{
	bool _LeftMouseButtonPressed = false;

	bool _revealInventory = false;

	[HideInInspector]
	private ItemGrid selectedItemGrid;

	[HideInInspector]
	private ItemSlot selectedItemSlot;

	public ItemGrid SelectedItemGrid
	{
		get => selectedItemGrid;
		set
		{
			selectedItemGrid = value;
			inventoryHighlight.SetParent(value);
		}
	}

	public ItemSlot SelectedItemSlot
	{
		get => selectedItemSlot;
		set
		{
			selectedItemSlot = value;
			inventoryHighlight.SetParent(value);
		}
	}

	InventoryItem selectedItem;
	InventoryItem overlapItem;
	RectTransform rectTransform;

	[SerializeField] CanvasGroup InventoyCanvasGroup;
	[SerializeField] List<ItemData> items;
	[SerializeField] GameObject itemPrefab;
	[SerializeField] Transform canvasTransform;
	[SerializeField] GameObject itemObjectPrefab;

	public List<ItemGrid> AllItemGrids = new List<ItemGrid>();

	InventoryHighlight inventoryHighlight;

	public List<GameObject> AllItemsInInventory = new();

	void Awake()
	{
		inventoryHighlight = GetComponent<InventoryHighlight>();
	}

	void Start()
	{
		// me. // collecting all the grids. This will cause errors if you do not remove grid from here when you delete the grid.
		foreach (ItemGrid _itemGrid in canvasTransform.GetComponentsInChildren<ItemGrid>())
		{
			AllItemGrids.Add(_itemGrid);
		}

		ToggleInventory(_revealInventory);
	}

	void Update()
	{
		ItemIconDrag();

		// if (Input.GetKeyDown(KeyCode.Q)) // ! replace
		// {
		//     if (selectedItem == null)
		//     {
		//         CreateRandomItem();
		//     }
		// }

		if (Input.GetKeyDown(KeyCode.W)) // ! replace
		{
			if (selectedItem == null)
				InsertRandomItem();
		}

		if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
		{
			_revealInventory = !_revealInventory;
			ToggleInventory(_revealInventory);
		}

		if (Input.GetKeyDown(KeyCode.R)) // ! replace
		{
			RotateItem();
		}

		// do not move to the the if below, it will run because strangeness but you can try, i was sleepy.
		if (_LeftMouseButtonPressed && selectedItemGrid == null && selectedItemSlot == null && selectedItem != null)
		{
			// TODO move into its own function. ?
			DropItemObject(selectedItem);
		}


		if (selectedItemGrid == null && selectedItemSlot == null)
		{
			inventoryHighlight.Show(false);
			return; // ! A return is here, this stops the programe here. ================================== <<<<
		}

		HandleHighlight();

		if (_LeftMouseButtonPressed)
		{
			LeftMouseButtonPress();
		}
	}

	public void MoveItem(GameObject go, Transform parent)
	{
		rectTransform = go.GetComponent<RectTransform>();
		rectTransform.SetParent(parent);
		rectTransform.SetAsLastSibling();
	}

	public InventoryItem CreateItem(ItemData item, Transform parent)
	{
		InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();

		rectTransform = inventoryItem.GetComponent<RectTransform>();
		rectTransform.SetParent(parent);
		rectTransform.SetAsLastSibling();

		inventoryItem.Set(item);

		// inventoryItem = item;

		//inventoryItem.itemData.Data.Store_Objects = new List<ItemData>();

		// TODO if the item has storage, then it should generate data for storage. AHHHHh neeed to look at this and check if what needs to be done.
		// if there is set data or how is data set, what if a item has data?
		inventoryItem.Data.itemData = inventoryItem.itemData;

		if (inventoryItem.itemData.ItemType == ItemType.backpack || inventoryItem.itemData.ItemType == ItemType.armor)
		{
			inventoryItem.storageData.Store_ItemDataStore ??= new List<ItemDataStore>();

			inventoryItem.storageData.Store_InventoryItemSlots ??= new InventoryItem[inventoryItem.itemData.SorageWidth, inventoryItem.itemData.SorageHeight];
		}

		return inventoryItem;
	}

	private void ToggleInventory(bool b)
	{
		if (b) // use  ? : if statement can be used
		{
			InventoyCanvasGroup.alpha = 1;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		else
		{
			InventoyCanvasGroup.alpha = 0;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			if (selectedItem != null)
			{
				DropItemObject(selectedItem);
			}
		}

		InventoyCanvasGroup.interactable = b;
		InventoyCanvasGroup.blocksRaycasts = b;
	}

	private void RotateItem()
	{
		if (selectedItem == null) { return; }

		selectedItem.Rotate();
	}

	public void DropItemObject(InventoryItem item)
	{
		ObjectItemData objectItemData = Instantiate(itemObjectPrefab, transform.position + transform.forward, Quaternion.identity).GetComponent<ObjectItemData>();
		objectItemData.itemData = item.itemData;
		objectItemData.Data = item.Data;
		objectItemData.storageData = item.storageData;


		Destroy(selectedItem.gameObject);
		selectedItem = null;

	}

	public bool PickUpItemObject(ObjectItemData objectItemData) // me
	{
		CreateItemPrefab(objectItemData.itemData);
		InventoryItem itemToInsert = selectedItem;
		selectedItem = null;

		itemToInsert.Data = objectItemData.Data;

		// ! NO, STOP IT! YOU'RE A FOOL, DATA IS SET IN THE CREATE ITEM PREFAB FUNC!!!!
		// ! STOP THIS! SAVE HEADACHES! SAVE TIME! CREATE BETTER CODE, CONFUSTION IS BAD!
		//itemToInsert.storageData = objectItemData.storageData;
		// ! THIS ONE LINE, ONE LINE CAUSED SO MANY DEBUG ISSUES AND ERRORS, AND ITS THE FIRST LAYER OF MILIONS!


		bool b = false;

		for (int i = 0; i < AllItemGrids.Count; i++)
		{
			b = TryToInsertItem(itemToInsert, AllItemGrids[i]);
			if (b)
			{
				break;
			}
		}

		if (!b)
		{
			DropItemObject(itemToInsert);
			Destroy(itemToInsert.gameObject);
			return false;
		}
		else
		{
			return true;
		}
	}

	private void InsertRandomItem()
	{
		if (selectedItemGrid == null) { return; }

		CreateRandomItem();
		InventoryItem itemToInsert = selectedItem;
		selectedItem = null;
		InsertItem(itemToInsert);
	}

	private bool TryToInsertItem(InventoryItem itemToInsert, ItemGrid itemGrid) // me
	{
		Vector2Int? posOnGrid = itemGrid.FindSpaceForObject(itemToInsert);

		if (posOnGrid == null)
		{
			return false;
		}

		// might cause issues.
		itemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);

		return true;
	}

	private void InsertItem(InventoryItem itemToInsert)
	{
		Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

		//print(posOnGrid == null ? "null" : posOnGrid.Value);

		if (posOnGrid == null)
		{
			// remove item as I cannot fit it in. but could do a on pick up failed.
			Destroy(itemToInsert.gameObject); // ! could make a check into a drop the item as inventory is full
			return;
		}

		selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
	}

	Vector2Int oldPosition;
	Vector2 oldSlotPos;
	InventoryItem itemToHighlight;

	private void HandleHighlight()
	{
		Vector2 posOnSlot = Vector2.zero;
		Vector2Int positionOnGrid = Vector2Int.zero;
		if (selectedItemSlot != null)
		{
			posOnSlot = GetSlotPos();

			//if (oldSlotPos == posOnSlot) { return; }

			oldSlotPos = posOnSlot;

		}
		else if (selectedItemGrid != null)
		{
			positionOnGrid = GetTileGridPosition();

			if (oldPosition == positionOnGrid) { return; }

			oldPosition = positionOnGrid;
		}




		if (selectedItem == null && SelectedItemSlot == null)
		{
			// ! look at \/, causing out of bound exepction. Only when stopping for some reason??
			itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

			if (itemToHighlight != null)
			{
				inventoryHighlight.Show(true);

				inventoryHighlight.SetSize(itemToHighlight);

				// inventoryHighlight.SetParent(selectedItemGrid);

				inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
			}
			else
			{
				inventoryHighlight.Show(false);
			}
		}
		else if (selectedItem == null && SelectedItemSlot != null)
		{
			itemToHighlight = selectedItemSlot.GetItem();

			if (itemToHighlight != null)
			{
				inventoryHighlight.Show(true);

				inventoryHighlight.SetSize(itemToHighlight);

				inventoryHighlight.SetPosition(selectedItemSlot, itemToHighlight);
			}
			else
			{
				inventoryHighlight.Show(false);
			}
		}
		else if (selectedItemSlot == null) // while holding
		{
			inventoryHighlight.Show(selectedItemGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT));

			inventoryHighlight.SetSize(selectedItem);

			// inventoryHighlight.SetParent(selectedItemGrid);

			inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
		}
		else if (selectedItemSlot != null)
		{
			inventoryHighlight.Show(selectedItemSlot != null);

			inventoryHighlight.SetSize(selectedItem);

			inventoryHighlight.SetPosition(selectedItemSlot, selectedItem);
		}
		// else
		// {
		//     print("!!! no sutible statment, please fix");
		// }
	}

	private void CreateRandomItem()
	{
		InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
		selectedItem = inventoryItem;

		rectTransform = inventoryItem.GetComponent<RectTransform>();
		rectTransform.SetParent(canvasTransform);
		rectTransform.SetAsLastSibling();

		int selectedItemID = UnityEngine.Random.Range(0, items.Count);
		inventoryItem.Set(items[selectedItemID]);

		// TODO if the item has storage, then it should generate data for storage.
		inventoryItem.Data.itemData = inventoryItem.itemData;

		if (items[selectedItemID].ItemType == ItemType.backpack || items[selectedItemID].ItemType == ItemType.armor)
		{
			if (inventoryItem.storageData.Store_ItemDataStore == null)
			{
				inventoryItem.storageData.Store_ItemDataStore = new List<ItemDataStore>();
			}

			if (inventoryItem.storageData.Store_InventoryItemSlots == null)
			{
				inventoryItem.storageData.Store_InventoryItemSlots = new InventoryItem[inventoryItem.itemData.SorageWidth, inventoryItem.itemData.SorageHeight];
			}
		}
	}

	private void CreateItemPrefab(ItemData itemData) // me
	{
		InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
		selectedItem = inventoryItem;

		rectTransform = inventoryItem.GetComponent<RectTransform>();
		rectTransform.SetParent(canvasTransform);
		rectTransform.SetAsLastSibling();

		inventoryItem.Set(itemData);
		inventoryItem.Data.itemData = inventoryItem.itemData;

		if (inventoryItem.itemData.ItemType == ItemType.backpack || inventoryItem.itemData.ItemType == ItemType.armor)
		{
			if (inventoryItem.storageData.Store_ItemDataStore == null)
			{
				inventoryItem.storageData.Store_ItemDataStore = new List<ItemDataStore>();
			}

			if (inventoryItem.storageData.Store_InventoryItemSlots == null)
			{
				inventoryItem.storageData.Store_InventoryItemSlots = new InventoryItem[inventoryItem.itemData.SorageWidth, inventoryItem.itemData.SorageHeight];
			}
		}

	}


	private void LeftMouseButtonPress()
	{
		if (selectedItemSlot != null)
		{
			if (selectedItem == null)
			{
				PickUpItemOnSlot();
			}
			else
			{
				PlaceItemOnSlot();
			}
			return;
		}

		Vector2Int tileGridPosition = GetTileGridPosition();

		if (selectedItem == null)
		{
			PickUpItem(tileGridPosition);
		}
		else
		{
			PlaceItem(tileGridPosition);
		}
	}

	private Vector2Int GetTileGridPosition()
	{
		Vector2 position = Input.mousePosition;

		if (selectedItem != null)
		{
			position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
			position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
		}

		return selectedItemGrid.GetTileGridPosition(position);
	}

	private Vector2 GetSlotPos()
	{
		Vector2 position = Input.mousePosition;

		// if (selectedItem != null)
		// {
		//     position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
		//     position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
		// }

		return selectedItemSlot.CalculateCenter();
	}

	private void PlaceItemOnSlot()
	{
		bool complete = selectedItemSlot.PlaceItem(selectedItem, ref overlapItem);
		if (complete)
		{
			selectedItem = null;
			if (overlapItem != null)
			{
				selectedItem = overlapItem;
				overlapItem = null;
				rectTransform = selectedItem.GetComponent<RectTransform>();
				rectTransform.SetAsLastSibling();
			}
		}
	}

	private void PickUpItemOnSlot()
	{
		selectedItem = selectedItemSlot.PickUpItem();
		if (selectedItem != null)
		{
			rectTransform = selectedItem.GetComponent<RectTransform>();
			rectTransform.SetParent(rectTransform.parent.parent); // set the item to the front of the inventory
			rectTransform.SetAsLastSibling();
		}
	}

	private void PlaceItem(Vector2Int tileGridPosition)
	{
		bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
		if (complete)
		{
			selectedItem = null;
			if (overlapItem != null)
			{
				selectedItem = overlapItem;
				overlapItem = null;
				rectTransform = selectedItem.GetComponent<RectTransform>();
				rectTransform.SetAsLastSibling();
			}
		}
	}

	private void PickUpItem(Vector2Int tileGridPosition)
	{
		selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
		if (selectedItem != null)
		{
			rectTransform = selectedItem.GetComponent<RectTransform>();
			rectTransform.SetParent(rectTransform.parent.parent); // set the item to the front of the inventory
			rectTransform.SetAsLastSibling();
		}
	}

	private void ItemIconDrag()
	{
		if (selectedItem != null)
		{
			rectTransform.position = Input.mousePosition;
		}
	}

	void IMouseLeftClickPress.left_Mouse_Button_Pressed(bool b)
	{
		_LeftMouseButtonPressed = b;
	}
}
