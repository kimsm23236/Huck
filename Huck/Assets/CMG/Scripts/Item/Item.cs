using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData = null;
    public int itemCount = 1;
    public bool itemInHand = false;


    public void OnUse(ItemSlot itemSlot_, PlayerStat playerstat_)
    {
        if (itemData.ItemUseAble)
        {
            ItemUse(itemSlot_, playerstat_);
        }
    }

    protected virtual void ItemUse(ItemSlot itemSlot_, PlayerStat playerStat_)
    {

    }
}