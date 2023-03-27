using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowMushroom : Item
{
    protected override void ItemUse(ItemSlot itemslot_)
    {
        base.ItemUse(itemslot_);
        PlayerStat.curHungry += 10;
    }
}
