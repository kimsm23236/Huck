using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData = null;
    public int itemCount = 1;

    public void OnUse(ItemSlot itemSlot_)
    {
        ItemUse(itemSlot_);
        Debug.Log("대리자 실행됨?");
    }

    protected virtual void ItemUse(ItemSlot itemSlot_)
    {
        if (itemSlot_.itemData.ItemUseAble)
        {
            itemSlot_.itemAmount--;
        }
    }
}