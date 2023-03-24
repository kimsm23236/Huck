using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InHand : MonoBehaviour
{
    [SerializeField]
    private Transform handTrans = default;
    [SerializeField]
    private GameObject inventory = default;
    private ItemSlot[] inventorySlotItem = new ItemSlot[8];
    private GameObject inHandObj = default;
    private ItemData inHnadItem = default;
    public int selectedQuitSlot = 0;
    [SerializeField]
    private PlayerStat playerStat = default;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            inventorySlotItem[i] = inventory.GetComponent<InventoryArray>().itemSlots[24 + i].GetComponent<ItemSlot>();
        }
        playerStat = GetComponent<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {
        OnHandItem();
        QuitSlotChange();
        UseItem();
    }

    private void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.J) && inventorySlotItem[selectedQuitSlot].itemData != null)
        {
            inventorySlotItem[selectedQuitSlot].itemUseDel(inventorySlotItem[selectedQuitSlot], playerStat);
        }
    }
    private void OnHandItem()
    {
        if (inventorySlotItem[selectedQuitSlot].itemData != null && inHnadItem != inventorySlotItem[selectedQuitSlot].itemData)
        {
            if (inHandObj == default)
            {
                inHnadItem = inventorySlotItem[selectedQuitSlot].itemData;
                inHandObj = Instantiate(inventorySlotItem[selectedQuitSlot].itemData.OriginPrefab.transform.GetChild(0).gameObject);
                inHandObj.transform.SetParent(handTrans, false);
            }
            else
            {
                Destroy(inHandObj);
                inHnadItem = inventorySlotItem[selectedQuitSlot].itemData;
                inHandObj = Instantiate(inventorySlotItem[selectedQuitSlot].itemData.OriginPrefab.transform.GetChild(0).gameObject);
                inHandObj.transform.SetParent(handTrans, false);
            }
        }
        else if (inventorySlotItem[selectedQuitSlot].itemData == null && inHnadItem != default)
        {
            Destroy(inHandObj);
            inHnadItem = default;
        }
    }

    private void QuitSlotChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedQuitSlot = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedQuitSlot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedQuitSlot = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedQuitSlot = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedQuitSlot = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedQuitSlot = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedQuitSlot = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedQuitSlot = 7;
        }
    }
}
