using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ResourceObject_Ore : BaseResourceObject
{
    public override void TakeDamage(DamageMessage message)
    {
        // 타입에 따른 추가 처리
        // 아이템 무기 타입이 곡괭이일 경우에만 데미지가 들어가게
        ItemData hitItem = message.item;
        float damageMultiplier_type = 0f;
        float damageMultiplier_level = 0f;

        
        if(hitItem != null && hitItem.ItemTool == EItemTool.PICKAXE)
            damageMultiplier_type = 1f;

        if (hitItem != null && (int)hitItem.ItemLevel >= (int)resLevel)
            damageMultiplier_level = 1f;

        message.damageAmount = message.damageAmount * (int)damageMultiplier_type * (int)damageMultiplier_level;

        // 데미지 처리
        base.TakeDamage(message);
    }
}
