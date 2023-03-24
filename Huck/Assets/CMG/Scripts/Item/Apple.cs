using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Item
{

    protected override void ItemUse(ItemSlot itemslot_, PlayerStat playerStat_)
    {
        base.ItemUse(itemslot_, playerStat_);

        itemslot_.itemAmount--;
        PlayerStat.curHp += 10;
        Debug.Log("사과");
    }
}
