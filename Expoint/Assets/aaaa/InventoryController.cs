using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    private ItemGrid selectedItemGrid;

    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    [SerializeField] GameObject itemObjectPrefab;

    List<ItemGrid> AllItemGrids = new List<ItemGrid>();

    InventoryHighlight inventoryHighlight;

    void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();

        // ? was this me? ~ yes
        foreach (ItemGrid _itemGrid in canvasTransform.GetComponentsInChildren<ItemGrid>())
        {
            AllItemGrids.Add(_itemGrid);
        }
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

        // if (Input.GetKeyDown(KeyCode.W)) // ! replace
        // {
        //     if (selectedItem == null)
        //         InsertRandomItem();
        // }

        if (Input.GetKeyDown(KeyCode.R)) // ! replace
        {
            RotateItem();
        }

        if (Input.GetMouseButtonDown(0) && selectedItemGrid == null && selectedItem != null)
        {
            // TODO move into its own function.
            DropItemObject(selectedItem.itemData);
        }

        if (selectedItemGrid == null)
        {
            inventoryHighlight.Show(false);
            return; // ! A return is here, this stops the programe here.
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0)) // ! replace
        {
            LeftMouseButtonPress();
        }
    }

    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }

    private void DropItemObject(ItemData item)
    {
        ObjectItemData objectItemData = Instantiate(itemObjectPrefab, transform.position + transform.forward, Quaternion.identity).GetComponent<ObjectItemData>();
        objectItemData.itemData = item;


        Destroy(selectedItem.gameObject);
        selectedItem = null;
    }

    public bool PickUpItemObject(ItemData item) // me
    {
        CreateItemPrefab(item);
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;

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
            Destroy(itemToInsert.gameObject); // could make a check into a drop the item as inventory is full
            return;
        }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        if (oldPosition == positionOnGrid) { return; }

        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
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
        else // while holding
        {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT));

            inventoryHighlight.SetSize(selectedItem);

            // inventoryHighlight.SetParent(selectedItemGrid);

            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
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


    }

    private void CreateItemPrefab(ItemData itemData) // me
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        inventoryItem.Set(itemData);
    }


    private void LeftMouseButtonPress()
    {
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
}
