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
    [SerializeField]
    private ItemData inHandItem = default;
    public int selectedQuitSlot = 0;

    private BuildSystem buildSystem = default;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            inventorySlotItem[i] = inventory.GetComponent<InventoryArray>().itemSlots[24 + i].GetComponent<ItemSlot>();
        }
        buildSystem = GFunc.GetRootObj("BuildSystem").GetComponent<BuildSystem>();
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
            if (inHandObj == default)
            {
                inHandItem = inventorySlotItem[selectedQuitSlot].itemData;
                inHandObj = Instantiate(inventorySlotItem[selectedQuitSlot].itemData.OriginPrefab.transform.GetChild(0).gameObject);
                inHandObj.transform.SetParent(handTrans, false);
                if (inHandItem.IsBuild)
                {
                    inHandObj.GetComponent<Collider>().enabled = false;
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
                if (inHandItem.IsBuild)
                {
                    inHandObj.GetComponent<Collider>().enabled = false;
                    inHandObj.GetComponent<Renderer>().enabled = false;

                    buildSystem.IsBuildTime = true;
                    buildSystem.CallingPrev(inHandItem.ItemName);
                }

            }
        }
        else if (inventorySlotItem[selectedQuitSlot].itemData == null && inHandItem != default)
        {
            Destroy(inHandObj);
            inHandItem = default;
            buildSystem.IsBuildTime = false;
            buildSystem.CallingPrev();
        }
    }

    // //! 퀵슬롯의 아이템을 손에 적용해주는 함수
    // private void OnHandItem()
    // {
    //     // bool isNotSameInHandItemAndIvenSlotItem = inHandItem != inventorySlotItem[selectedQuitSlot].itemData;
    //     if (inventorySlotItem[selectedQuitSlot].itemData == null)
    //     {
    //         if (inHandItem == default || inHandItem == null) { return; }

    //         // 퀵 슬롯에 아이템이 없는데, 손에 아이템이 있는 경우 / A 케이스

    //         // A 케이스는 퀵 슬롯에 있던 아이템을 인벤토리로 뺀 경우 발생한다.
    //         // 손에 들고 있던 아이템을 빼고, 빌딩 시스템을 해제하는 등 동작을 수행해야 함.
    //         Debug.Log("??");
    //         buildSystem.IsBuildTime = false;
    //         buildSystem.CallingPrev();
    //         inHandItem = default;
    //         Destroy(inHandItem);
    //     }   // if: 퀵 슬롯에 아이템이 없는 경우
    //     else
    //     {
    //         if (inHandItem == default || inHandItem == null)
    //         {
    //             ChangeHandItem();
    //         }   // if: 손에 아이템이 없는 경우
    //         else
    //         {
    //             // 퀵 슬롯의 아이템과 손에 든 아이템이 같은 경우
    //             if (inHandItem == inventorySlotItem[selectedQuitSlot].itemData) { return; }
    //             else
    //             {
    //                 Destroy(inHandObj);
    //                 if (buildSystem.IsBuildTime == true)
    //                 {
    //                     buildSystem.IsBuildTime = false;
    //                     buildSystem.CallingPrev(inventorySlotItem[selectedQuitSlot].itemData.ItemName);
    //                 }
    //                 inHandItem = default;
    //             }       // else: 퀵 슬롯의 아이템과 손에 든 아이템이 다른 경우
    //         }   // else: 손에 아이템을 무언가 들고 있는 경우
    //     }   // else: 퀵 슬롯에 아이템이 있는 경우 

    // LEGACY: 
    // if (inventorySlotItem[selectedQuitSlot].itemData != null && isNotSameInHandItemAndIvenSlotItem)
    // {
    //     // 퀵 슬롯에 존재하는 아이템을 손에 들 예정

    //     ChangeHandItem();
    // } // 퀵슬롯에 선택된 아이템이 존재하고, 손에 해당 아이템을 들고 있지 않은 경우
    // else if (inventorySlotItem[selectedQuitSlot].itemData == null && inHandItem != default)
    // {
    //     // 
    //     Debug.Log("여기로 가니?");
    //     Destroy(inHandObj);
    //     if (buildSystem.IsBuildTime == true)
    //     {
    //         buildSystem.IsBuildTime = false;
    //         buildSystem.CallingPrev(inventorySlotItem[selectedQuitSlot].itemData.ItemName);
    //     }
    //     inHandItem = default;
    // } // 퀵 슬롯이 비어 있고, 손에 아이템을 들고 있을 경우

    // }       // OnHandItem()

    //! 손에 든 아이템을 바꾼다.
    // private void ChangeHandItem()
    // {
    //     if (inventorySlotItem[selectedQuitSlot].itemData.IsBuild && inventorySlotItem[selectedQuitSlot].itemData != null)
    //     {
    //         if (buildSystem.IsBuildTime == false)
    //         {
    //             Destroy(inHandObj);
    //             buildSystem.IsBuildTime = true;
    //             buildSystem.CallingPrev(inventorySlotItem[selectedQuitSlot].itemData.ItemName);
    //             Debug.Log("왜 true?");
    //         }       // if: 건축 시스템을 켜지 않은 경우
    //     } // 건축 아이템일 경우
    //     else
    //     {
    //         Debug.Log("들어가");
    //         if (buildSystem.IsBuildTime == true)
    //         {
    //             buildSystem.IsBuildTime = false;
    //             buildSystem.CallingPrev(inventorySlotItem[selectedQuitSlot].itemData.ItemName);
    //         }

    //         if (inHandObj == default)
    //         {
    //             inHandItem = inventorySlotItem[selectedQuitSlot].itemData;
    //             inHandObj = Instantiate(inventorySlotItem[selectedQuitSlot].itemData.OriginPrefab.transform.GetChild(0).gameObject);
    //             inHandObj.transform.SetParent(handTrans, false);
    //         } // 손에 아무것도 없었을 경우
    //         else
    //         {
    //             Destroy(inHandObj);
    //             inHandItem = inventorySlotItem[selectedQuitSlot].itemData;
    //             inHandObj = Instantiate(inventorySlotItem[selectedQuitSlot].itemData.OriginPrefab.transform.GetChild(0).gameObject);
    //             inHandObj.transform.SetParent(handTrans, false);
    //         } // 손에 든게 있었을 경우
    //     } // 건축아이템이 아닐경우
    // }       // ChangeHandItem()

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
