using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCraft : MonoBehaviour
{
    [SerializeField]
    private InventoryArray inventory = default;
    [SerializeField]
    private CraftResource craftResource = default;
    private int firstCount = 0;
    private int secondCount = 0;
    private int thirdCount = 0;
    public bool isCraft = false;

    private void Start()
    {
        inventory = UIManager.Instance.inventory.GetComponent<InventoryArray>();

    }

    private void Update()
    {
        if (PlayerOther.isInvenOpen)
        {
            ItemCreateAble();
        }
    }

    protected virtual void ItemCreateAble()
    {
        firstCount = 0;
        secondCount = 0;
        thirdCount = 0;
        for (int i = 0; i < inventory.itemSlots.Count; i++)
        {
            if (inventory.itemSlotScripts[i].itemData != null)
            {
                if (craftResource.firstResourceName == inventory.itemSlotScripts[i].itemData.ItemName)
                {
                    firstCount += inventory.itemSlotScripts[i].itemAmount;
                }

                if (craftResource.secondResourceName == inventory.itemSlotScripts[i].itemData.ItemName)
                {
                    secondCount += inventory.itemSlotScripts[i].itemAmount;
                }

                if (craftResource.secondResourceName == inventory.itemSlotScripts[i].itemData.ItemName)
                {
                    thirdCount += inventory.itemSlotScripts[i].itemAmount;
                }

            }
        }
        if (firstCount >= craftResource.firstResourceCount && secondCount >= craftResource.secondResourceCount && thirdCount >= craftResource.thirdResourceCount)
        {
            isCraft = true;
        }
        else
        {
            isCraft = false;
        }
    }
}
