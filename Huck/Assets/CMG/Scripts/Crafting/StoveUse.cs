using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoveUse : MonoBehaviour
{
    private StoveItem nowStove = default;

    private GameObject originItem = default;
    private GameObject fuelItem = default;
    private GameObject resultItem = default;

    private ItemSlot originItemSlot = default;
    private ItemSlot fuelItemSlot = default;
    private ItemSlot resultItemSlot = default;

    private ItemData originItemData = default;
    private ItemData fuelItemData = default;
    private ItemData resultItemData = default;

    private Image percentImg = default;
    private Image energyImg = default;

    // Start is called before the first frame update
    private void Awake()
    {
        originItem = transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        originItemSlot = originItem.GetComponent<ItemSlot>();

        fuelItem = transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        fuelItemSlot = fuelItem.GetComponent<ItemSlot>();

        resultItem = transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
        resultItemSlot = resultItem.GetComponent<ItemSlot>();

        percentImg = transform.GetChild(0).GetChild(1).GetChild(4).GetComponent<Image>();
        energyImg = transform.GetChild(0).GetChild(1).GetChild(6).GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance.playerObj != null)
        {
            nowStove = Camera.main.GetComponent<ItemRange>().stoveItem;
        }
        if (nowStove != null)
        {
            CopyFromStove();
        }
        // if () 플레이어가 화로를 열었을 때 그 화로의 StoveItem컴포넌트를 가져와야함
        // StoveItem에서 originItem , fuelItem, resultItem 들과 개수 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        slotCheck();
        if (nowStove.isChange)
        {
            CopyFromStove();
            nowStove.isChange = false;
        }
        CopyToStove();
        percentImg.fillAmount = nowStove.percentage / 100f;
        energyImg.fillAmount = nowStove.fuelEnergy / 100f;
    }

    private void OnDisable()
    {
        if (nowStove != null)
        {
            // CopyToStove();
            ResetUi();
        }
    }

    private void CopyToStove()
    {
        nowStove.originItemData = originItemData;

        nowStove.originItemCount = originItemSlot.itemAmount;
        nowStove.fuelItemData = fuelItemData;
        nowStove.fuelItemCount = fuelItemSlot.itemAmount;
        nowStove.resultItemData = resultItemData;
        nowStove.resultItemCount = resultItemSlot.itemAmount;
    }

    private void CopyFromStove()
    {
        originItemSlot.itemData = nowStove.originItemData;
        originItemSlot.itemAmount = nowStove.originItemCount;
        fuelItemSlot.itemData = nowStove.fuelItemData;
        fuelItemSlot.itemAmount = nowStove.fuelItemCount;
        resultItemSlot.itemData = nowStove.resultItemData;
        resultItemSlot.itemAmount = nowStove.resultItemCount;
    }

    private void ResetUi()
    {
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
}
