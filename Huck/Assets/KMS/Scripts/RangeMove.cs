using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMove : MonoBehaviour, IDamageable
{
    private GameObject camera_1p = default;
    private InHand playerInHand = default;
    private PlayerStat playerStat = default;
    private ResourceObjectSO BRO = default;

    void Start()
    {
        camera_1p = Camera.main.gameObject;
        playerInHand = transform.parent.GetComponent<InHand>();
        playerStat = transform.parent.GetComponent<PlayerStat>();
    }

    void Update()
    {
        // Follow camera
        gameObject.transform.position = camera_1p.transform.position;
        gameObject.transform.rotation = camera_1p.transform.rotation;
    }


    private void OnTriggerEnter(Collider other)
    {
        ItemData item_ = playerInHand.inventorySlotItem[playerInHand.selectedQuitSlot].itemData;

        DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage, item_);
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(dm);
        }

        /*
        if (other.tag == GData.ENEMY_MASK)
        {
            ItemData item_ = playerInHand.inventorySlotItem[playerInHand.selectedQuitSlot].itemData;

            DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage);
            other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
        }
        if (other.tag == GData.GATHER_MASK)
        {
            BRO = other.GetComponent<BaseResourceObject>().ResourceConfig;
            ItemData item_ = playerInHand.inventorySlotItem[playerInHand.selectedQuitSlot].itemData;

            // DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage, item_);
            // other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
            if (item_ != null)
            {
                if (item_.ItemTool == EItemTool.AXE)
                {
                    if (BRO.ResourceType == EResourceType.WOOD)
                    {
                        if ((int)item_.ItemLevel < (int)BRO.ResourceLevel)
                        {
                            DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage * 0, item_);
                            other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
                        }
                        else
                        {
                            DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage, item_);
                            other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
                        }
                    }
                    else
                    {
                        DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage * 0, item_);
                        other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
                    }
                }
                else if (item_.ItemTool == EItemTool.PICKAXE)
                {
                    if (BRO.ResourceType == EResourceType.ORE)
                    {
                        if ((int)item_.ItemLevel < (int)BRO.ResourceLevel)
                        {
                            DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage * 0, item_);
                            other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
                        }
                        else
                        {
                            DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage, item_);
                            other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
                        }
                    }
                    else
                    {
                        DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage * 0, item_);
                        other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
                    }
                }
                else
                {
                    if (BRO.ResourceType == EResourceType.ORE || BRO.ResourceType == EResourceType.WOOD)
                    {
                        DamageMessage dm = new DamageMessage(transform.parent.gameObject, playerStat.damage * 0, item_);
                        other.gameObject.GetComponent<IDamageable>().TakeDamage(dm);
                    }
                }
            }
        }
        */
    }
}
