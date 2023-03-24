using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitSlot : MonoBehaviour
{
    [SerializeField]
    private GameObject inven = default;
    private ItemSlot[] quitSlotsItem = new ItemSlot[8];
    private ItemSlot[] inventorySlotItem = new ItemSlot[8];
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            quitSlotsItem[i] = transform.GetChild(i).GetComponent<ItemSlot>();
            inventorySlotItem[i] = inven.GetComponent<InventoryArray>().itemSlots[24 + i].GetComponent<ItemSlot>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            quitSlotsItem[i].itemData = inventorySlotItem[i].itemData;
            quitSlotsItem[i].itemAmount = inventorySlotItem[i].itemAmount;
        }
    }
}
