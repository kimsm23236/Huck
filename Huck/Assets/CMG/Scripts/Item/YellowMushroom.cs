using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowMushroom : Item
{
    protected override void ItemUse(ItemSlot itemslot_, PlayerStat playerStat_)
    {
        base.ItemUse(itemslot_, playerStat_);
        itemslot_.itemAmount--;
        PlayerStat.curHungry += 10;
        Debug.Log("버섯");
    }
}
