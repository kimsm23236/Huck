using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InHand : MonoBehaviour
{
    [SerializeField]
    private Transform handTrans = default;
    private GameObject inventory = default;
    [HideInInspector]
    public ItemSlot[] inventorySlotItem = new ItemSlot[8];
    private GameObject inHandObj = default;
    [SerializeField]
    private ItemData inHandItem = default;

    private PlayerStat playerStat = default;
    public BuildSystem buildSystem = default;
    public int selectedQuitSlot = 0;
    public StoveItem stoveItem = default;
    // Start is called before the first frame update
    void Start()
    {
        inventory = UIManager.Instance.inventory;
        playerStat = GetComponent<PlayerStat>();
        for (int i = 0; i < 8; i++)
        {
            inventorySlotItem[i] = inventory.GetComponent<InventoryArray>().itemSlots[24 + i].GetComponent<ItemSlot>();
        }
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
            inventorySlotItem[selectedQuitSlot].itemUseDel(inventorySlotItem[selectedQuitSlot]);
        }
    }

    private void OnHandItem()
    {
        if (inventorySlotItem[selectedQuitSlot].itemData != null && inHandItem != inventorySlotItem[selectedQuitSlot].itemData)
        {
            playerStat.damage = 0;
            playerStat.damage += inventorySlotItem[selectedQuitSlot].itemData.ItemDamage;
            if (inHandObj == default)
            {
                inHandItem = inventorySlotItem[selectedQuitSlot].itemData;
                inHandObj = Instantiate(inventorySlotItem[selectedQuitSlot].itemData.OriginPrefab.transform.GetChild(0).gameObject);
                inHandObj.GetComponent<Collider>().enabled = false;
                inHandObj.transform.SetParent(handTrans, false);
                if (inHandItem.IsBuild)
                {
                    inHandObj.GetComponent<Renderer>().enabled = false;
                    buildSystem.IsBuildTime = true;
                    buildSystem.CallingPrev(inHandItem.ItemName);
                }
            }
            else
            {
                if (inHandItem.IsBuild)
                {
                    inHandObj.GetComponent<Collider>().enabled = false;
                    inHandObj.GetComponent<Renderer>().enabled = false;
                    buildSystem.IsBuildTime = false;
                    buildSystem.CallingPrev();
                }
                Destroy(inHandObj);
                inHandItem = inventorySlotItem[selectedQuitSlot].itemData;
                inHandObj = Instantiate(inventorySlotItem[selectedQuitSlot].itemData.OriginPrefab.transform.GetChild(0).gameObject);
                inHandObj.transform.SetParent(handTrans, false);
                inHandObj.GetComponent<Collider>().enabled = false;
                if (inHandItem.IsBuild)
                {
                    inHandObj.GetComponent<Renderer>().enabled = false;

                    buildSystem.IsBuildTime = true;
                    buildSystem.CallingPrev(inHandItem.ItemName);
                }
            }
        }
        else if (inventorySlotItem[selectedQuitSlot].itemData == null && inHandItem != default)
        {
            playerStat.damage = 0;
            Destroy(inHandObj);
            inHandItem = default;
            buildSystem.IsBuildTime = false;
            buildSystem.CallingPrev();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("build"))
        {
            stoveItem = other.GetComponent<StoveItem>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("build"))
        {
            stoveItem = default;
        }
    }
}
