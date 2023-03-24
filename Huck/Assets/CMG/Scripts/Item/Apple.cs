using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Item
{

    protected override void ItemUse(ItemSlot itemslot_)
    {
        base.ItemUse(itemslot_);

        itemslot_.itemAmount--;
        PlayerStat.curHp += 10;
    }
}
