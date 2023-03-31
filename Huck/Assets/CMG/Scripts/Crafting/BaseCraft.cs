using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseCraft : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private InventoryArray inventory = default;
    private CraftInven nowInven = default;
    public Item craftThing = default;
    [SerializeField]
    private CraftResource craftResource = default;
    private int firstCount = 0;
    private int secondCount = 0;
    private int thirdCount = 0;
    public bool isCraft = false;
    private Image CraftImg = default;

    private void Start()
    {
        inventory = UIManager.Instance.inventory.GetComponent<InventoryArray>();
        CraftImg = transform.GetChild(0).GetComponent<Image>();
        if (transform.parent.parent.GetChild(1).GetComponent<CraftInven>() != null)
        {
            nowInven = transform.parent.parent.GetChild(1).GetComponent<CraftInven>();
        }
    }

    private void Update()
    {
        if (PlayerOther.isAnvilOpen || PlayerOther.isWorkbenchOpen || PlayerOther.isInvenOpen)
        {
            ItemCreateAble();
        }
        ChangeIconAlpha();
    }

    private void CreateItem()
    {
        for (int i = 0; i < inventory.itemSlotScripts.Count; i++)
        {
            if (inventory.itemSlotScripts[i].itemData != null)
            {
                if (craftResource.firstResourceName == inventory.itemSlotScripts[i].itemData.ItemName)
                {
                    inventory.itemSlotScripts[i].itemAmount -= craftResource.firstResourceCount;
                }

                if (craftResource.secondResourceName == inventory.itemSlotScripts[i].itemData.ItemName)
                {
                    inventory.itemSlotScripts[i].itemAmount -= craftResource.secondResourceCount;
                }

                if (craftResource.thirdResourceName == inventory.itemSlotScripts[i].itemData.ItemName)
                {
                    inventory.itemSlotScripts[i].itemAmount -= craftResource.thirdResourceCount;
                }
            }
        }
        inventory.AddItem(craftThing);
    }



    private void ChangeIconAlpha()
    {
        if (isCraft)
        {
            CraftImg.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            CraftImg.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }
    private void ItemCreateAble()
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isCraft)
        {
            CreateItem();
            if (nowInven != null)
            {
                nowInven.OpenUi();
            }
        }
    }
}
