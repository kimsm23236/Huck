using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkMushroom : Item
{
    protected override void ItemUse(ItemSlot itemslot_)
    {
        base.ItemUse(itemslot_);
        PlayerStat.curEnergy += 10;
    }

}
