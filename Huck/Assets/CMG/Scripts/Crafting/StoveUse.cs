using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveUse : MonoBehaviour
{
    public StoveItem nowStove = default;

    public GameObject originItem = default;
    public GameObject fuelItem = default;
    public GameObject resultItem = default;

    public ItemSlot originItemSlot = default;
    public ItemSlot fuelItemSlot = default;
    public ItemSlot resultItemSlot = default;

    public ItemData originItemData = default;
    public ItemData fuelItemData = default;
    public ItemData resultItemData = default;

    // Start is called before the first frame update
    private void Awake()
    {
        originItem = transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        originItemSlot = originItem.GetComponent<ItemSlot>();

        fuelItem = transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        fuelItemSlot = fuelItem.GetComponent<ItemSlot>();

        resultItem = transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
        resultItemSlot = resultItem.GetComponent<ItemSlot>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance.playerObj != null)
        {
            nowStove = GameManager.Instance.playerObj.GetComponent<InHand>().stoveItem;
        }
        if (nowStove != null)
        {
            originItemSlot.itemData = nowStove.originItemData;
            originItemSlot.itemAmount = nowStove.originItemCount;
            fuelItemSlot.itemData = nowStove.fuelItemData;
            fuelItemSlot.itemAmount = nowStove.fuelItemCount;
            resultItemSlot.itemData = nowStove.resultItemData;
            resultItemSlot.itemAmount = nowStove.resultItemCount;
        }
        // if () 플레이어가 화로를 열었을 때 그 화로의 StoveItem컴포넌트를 가져와야함
        // StoveItem에서 originItem , fuelItem, resultItem 들과 개수 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        slotCheck();
        // UseStove();
    }

    private void OnDisable()
    {
        if (nowStove != null)
        {
            nowStove.originItemData = originItemData;
            nowStove.originItemCount = originItemSlot.itemAmount;
            nowStove.fuelItemData = fuelItemData;
            nowStove.fuelItemCount = fuelItemSlot.itemAmount;
            nowStove.resultItemData = resultItemData;
            nowStove.resultItemCount = resultItemSlot.itemAmount;
            originItemData = default;
            fuelItemData = default;
            resultItemData = default;

            originItemSlot.itemData = null;
            originItemSlot.itemAmount = 0;
            originItemSlot.DisableImg();

            fuelItemSlot.itemData = null;
            fuelItemSlot.itemAmount = 0;
            fuelItemSlot.DisableImg();

            resultItemSlot.itemData = null;
            resultItemSlot.itemAmount = 0;
            resultItemSlot.DisableImg();
        }

        // StoveItem에 originItem , fuelItem, resultItem 다시 집어 넣기 => 얼마나 구어졌는지 진행도도 넘겨줘야 함
    }

    private void slotCheck()
    {
        if (originItemSlot.itemData != null)
        {
            originItemData = originItemSlot.itemData;
        }
        else
        {
            originItemData = default;
        }
        if (fuelItemSlot.itemData != null)
        {
            fuelItemData = fuelItemSlot.itemData;
        }
        else
        {
            fuelItemData = default;
        }
        if (resultItemSlot.itemData != null)
        {
            resultItemData = resultItemSlot.itemData;
        }
        else
        {
            resultItemData = default;
        }
    }



    private void UseStove()
    {
        if (originItemData != null && fuelItemData != null)
        {
            if (originItemData.ResultData != null && fuelItemData.IsFuel)
            {
                if (resultItemData == null)
                {
                    Debug.Log("화로 사용중");
                }
                else
                {
                    if (resultItemData.ItemName == originItemData.ItemName)
                    {
                        Debug.Log("화로 사용중");
                    }
                    else
                    {
                        //Do nothing
                    }
                }
            }
        }
    }
}
