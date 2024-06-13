using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
	public ItemData itemData;

	public StorageData storageData;

	public ItemDataStore Data;

	public int HEIGHT
	{
		get
		{
			if (rotated == false)
			{
				return itemData.height;
			}
			return itemData.width;
		}
	}

	public int WIDTH
	{
		get
		{
			if (rotated == false)
			{
				return itemData.width;
			}
			return itemData.height;
		}
	}

	public int OnGridPositionX;
	public int OnGridPositionY;

	public bool rotated = false;

	internal void Rotate()
	{
		rotated = !rotated;

		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? -90f : 0f);
		Data.Rotated = rotated;

	}

	internal void Set(ItemData itemData)
	{
		this.itemData = itemData;
		Data.itemData = itemData;

		GetComponent<Image>().sprite = itemData.itemIcon;

		Vector2 size = new Vector2();
		size.x = itemData.width * ItemGrid.tileSizeWidth;
		size.y = itemData.height * ItemGrid.tileSizeHeight;
		GetComponent<RectTransform>().sizeDelta = size;
	}

	internal void SetAndOverideStorageData(ItemData itemData)
	{
		storageData = new StorageData();

		this.itemData = itemData;
		Data.itemData = itemData;

		GetComponent<Image>().sprite = itemData.itemIcon;

		Vector2 size = new Vector2();
		size.x = itemData.width * ItemGrid.tileSizeWidth;
		size.y = itemData.height * ItemGrid.tileSizeHeight;
		GetComponent<RectTransform>().sizeDelta = size;
	}
}
