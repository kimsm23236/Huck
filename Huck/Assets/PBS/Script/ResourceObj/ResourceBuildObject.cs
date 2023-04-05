using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuildObject : BaseResourceObject
{
    [SerializeField]
    private BuildSystem Bsystem;

    private void Awake()
    {
        Bsystem = GameManager.Instance.buildSystem;
    }

    public override void TakeDamage(DamageMessage message)
    {
        ItemData hitItem = message.item;
        float damageMultiplier_type = 0f;
        float damageMultiplier_level = 0f;

        if (hitItem != null && hitItem.ItemTool == EItemTool.AXE)
            damageMultiplier_type = 1f;

        if (hitItem != null && (int)hitItem.ItemLevel >= (int)resLevel)
            damageMultiplier_level = 1f;

        if(message.causer.tag == GData.PLAYER_MASK)
            message.damageAmount = message.damageAmount * (int)damageMultiplier_type * (int)damageMultiplier_level;

        base.TakeDamage(message);
    }

    protected override void Die()
    {
        DropItem(dropItems, transform);
        Bsystem.FindBuildObj(gameObject);
    }
}
