using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Item
{
    protected override void ItemUse(ItemSlot itemslot_)
    {
        base.ItemUse(itemslot_);
        PlayerStat.curHungry += 10;
        PlayerStat.curHp += 10;
    }
}
