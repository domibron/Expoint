using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    // Multi-dimensional array x and y first part of the arry. the second is in Init.
    InventoryItem[,] inventoryItemSlots;

    [SerializeField] int gridSizeWidth = 20;
    [SerializeField] int gridSizeHeight = 10;

    RectTransform rectTransform;

    public bool IsAStorageItem = false;
    public StorageData _StorageData;
    // public ItemDataStore _ItemDataStore;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    void Start()
    {
        _StorageData = new StorageData();
        ResetLocalStorageData();
        // _StorageData.Store_InventoryItemSlots = new Array[,];
        // _StorageData.Store_ItemDataStore = new List<ItemDataStore>();
        // _StorageData.ItemsInStore = new List<InventoryItem>();
        // print(_StorageData.Store_ItemData.Count);
    }

    public void ResetLocalStorageData()
    {
        _StorageData.Store_InventoryItemSlots = new InventoryItem[1, 1];
        _StorageData.Store_ItemDataStore = new List<ItemDataStore>();
    }

    public void SetLocalStorageData(StorageData storageData, ItemData itemData)
    {
        _StorageData.Store_InventoryItemSlots = new InventoryItem[itemData.SorageWidth, itemData.SorageHeight];
        _StorageData.Store_ItemDataStore = storageData.Store_ItemDataStore;
    }

    public void ChangeSize(int width, int height)
    {
        gridSizeWidth = width;
        gridSizeHeight = height;

        inventoryItemSlots = new InventoryItem[width, height]; // seconhd part of the array, sets the size.
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlots[x, y];

        if (toReturn == null)
            return null;

        CleanGridReference(toReturn);

        toReturn.Data.LastPosX = null;
        toReturn.Data.LastPosY = null;

        if (IsAStorageItem)
        {

            _StorageData.Store_InventoryItemSlots = inventoryItemSlots;
            _StorageData.Store_ItemDataStore.Remove(toReturn.Data);

        }


        return toReturn;
    }

    private void CleanGridReference(InventoryItem item) // removes the refernced Item from the grid.
    {
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlots[item.OnGridPositionX + ix, item.OnGridPositionY + iy] = null;
            }
        }
    }

    private void Init(int width, int height)
    {
        inventoryItemSlots = new InventoryItem[width, height]; // seconhd part of the array, sets the size.
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;

    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlots[x, y];
    }

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }


    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (IsAStorageItem && (inventoryItem.itemData.ItemType == ItemType.backpack || inventoryItem.itemData.ItemType == ItemType.armor))
        {
            print("cannot put storage Items within themselfs");
            return false;
        }

        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    public void PlaceItemForGeneratingItems(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlots[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.OnGridPositionX = posX;
        inventoryItem.OnGridPositionY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;

        inventoryItem.Data.LastPosX = posX;
        inventoryItem.Data.LastPosY = posY;

        _StorageData.Store_InventoryItemSlots = inventoryItemSlots;
        // print(inventoryItem.gameObject.name);


        // ! ???? start
        // int = _StorageData
        // end

        // if (IsAStorageItem)
        // {
        //     _StorageData.Store_ItemDataStore.Add(inventoryItem.Data);
        //     _StorageData.Store_InventoryItemSlots = inventoryItemSlots;
        //     // _StorageData.Store_ItemDataStore.Remove(inventoryItem.Data);

        //     // handling resetting the storage data when item is removed.
        // }
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlots[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.OnGridPositionX = posX;
        inventoryItem.OnGridPositionY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;

        inventoryItem.Data.LastPosX = posX;
        inventoryItem.Data.LastPosY = posY;

        _StorageData.Store_InventoryItemSlots = inventoryItemSlots;
        // print(inventoryItem.gameObject.name);


        // ! ???? start
        // int = _StorageData
        // end

        if (IsAStorageItem)
        {
            _StorageData.Store_ItemDataStore.Add(inventoryItem.Data);
            _StorageData.Store_InventoryItemSlots = inventoryItemSlots;
            // _StorageData.Store_ItemDataStore.Remove(inventoryItem.Data);

            // handling resetting the storage data when item is removed.
        }


    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlots[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                        overlapItem = inventoryItemSlots[posX + x, posY + y];
                    else
                    {
                        if (overlapItem != inventoryItemSlots[posX + x, posY + y])
                            return false;
                    }
                }
            }
        }

        return true;
    }

    bool PositionCheck(int posX, int posY)
    {
        if (posX < 0 || posY < 0)
        {
            return false;
        }

        if (posX >= gridSizeWidth || posY >= gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if (PositionCheck(posX, posY) == false)
            return false;

        posX += width - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false)
            return false;


        return true;
    }

    private bool CheckAvalibleSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlots[posX + x, posY + y] != null)
                {

                    return false;

                }
            }
        }

        return true;
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvalibleSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }
}
