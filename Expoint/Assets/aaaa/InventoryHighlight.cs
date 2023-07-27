using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;

    public void Show(bool b)
    {
        highlighter.gameObject.SetActive(b);
    }

    public void SetSize(InventoryItem tartgetItem)
    {
        Vector2 size = new Vector2();
        size.x = tartgetItem.WIDTH * ItemGrid.tileSizeWidth;
        size.y = tartgetItem.HEIGHT * ItemGrid.tileSizeHeight;
        highlighter.sizeDelta = size;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.OnGridPositionX, targetItem.OnGridPositionY);

        highlighter.localPosition = pos;

    }

    public void SetParent(ItemGrid targetGrid)
    {
        if (targetGrid == null) { return; }
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }

    public void SetPosition(ItemSlot targetSlot, InventoryItem targetItem)
    {
        Vector2 pos = targetSlot.CalculateCenter(targetItem);

        highlighter.localPosition = pos;

    }

    public void SetParent(ItemSlot targetSlot)
    {
        if (targetSlot == null) { return; }
        highlighter.SetParent(targetSlot.GetComponent<RectTransform>());
    }
}
