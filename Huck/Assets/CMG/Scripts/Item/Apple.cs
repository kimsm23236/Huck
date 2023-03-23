using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Item
{

    protected override void ItemUse(ItemSlot itemslot_, PlayerStat playerStat_)
    {
        itemslot_.itemAmount--;
        playerStat_.curHp += 10;

    }
}
