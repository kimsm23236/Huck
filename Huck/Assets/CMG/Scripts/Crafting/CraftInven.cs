using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EInvenKind
{
    WORKBENCH = 0,
    STOVE,
    ANVIL
}

public class CraftInven : InventoryArray
{
    private InventoryArray inven = default;
    private List<ItemSlot> nowInventory = new List<ItemSlot>();
    public EInvenKind invenKind = default;
    private ItemSlot[] upSlots = new ItemSlot[3];

    protected override void Awake()
    {
        InitSlots();
    }

    private void OnEnable()
    {
        OpenUi();
    }

    protected override void Start()
    {
        inven = UIManager.Instance.inventory.GetComponent<InventoryArray>();
        for (int i = 0; i < transform.childCount; i++)
        {
            nowInventory.Add(transform.GetChild(i).GetComponent<ItemSlot>());
        }
        if (invenKind == EInvenKind.STOVE)
        {
            for (int i = 0; i < upSlots.Length; i++)
            {
                upSlots[i] = transform.parent.GetChild(1).GetChild(i).GetComponent<ItemSlot>();
            }
        }

        for (int i = 0; i < inven.transform.childCount; i++)
        {
            itemSlots.Add(inven.transform.GetChild(i).gameObject);
            itemSlotScripts.Add(inven.itemSlots[i].GetComponent<ItemSlot>());
        }
        CanvasSetting();
        InvenSetting();
        switch (invenKind)
        {
            case EInvenKind.WORKBENCH:
                UIManager.Instance.workBench.SetActive(false);
                break;
            case EInvenKind.STOVE:
                UIManager.Instance.stove.SetActive(false);
                break;
            case EInvenKind.ANVIL:
                UIManager.Instance.anvil.SetActive(false);
                break;
        }

    }

    protected override void Update()
    {
        switch (invenKind)
        {
            case EInvenKind.WORKBENCH:
                if (PlayerOther.isWorkbenchOpen)
                {
                    ControlMouse();
                }
                break;
            case EInvenKind.STOVE:
                ControlMouse();
                if (PlayerOther.isStoveOpen)
                {
                    if (beginDragSlot == upSlots[0] || beginDragSlot == upSlots[1] || beginDragSlot == upSlots[1])
                    {
                        Debug.Log(beginDragSlot);
                        upSlots[0].transform.parent.SetAsLastSibling();
                    }
                    else
                    {
                        upSlots[0].transform.parent.SetSiblingIndex(1);
                    }
                }
                break;
            case EInvenKind.ANVIL:
                if (PlayerOther.isAnvilOpen)
                {
                    ControlMouse();
                }
                break;
        }
    }

    private void OnDisable()
    {
        CloseUi();
    }
    protected override void CanvasSetting()
    {
        myCanvas = UIManager.Instance.UiObjs.GetComponent<Canvas>();
    }

    protected override void InitSlots()
    {
        slotWidth = slotPrefab.GetComponent<RectTransform>().sizeDelta.x;
        slotHeight = slotPrefab.GetComponent<RectTransform>().sizeDelta.y;
        beginSlotPos = new Vector2(slotWidth / 2 + paddingSlot, slotHeight / -2 - paddingSlot);
        int slotIdx = 1;
        for (int y = 0; y < verticalSlotCount; y++)
        {
            for (int x = 0; x < horizonSlotCount; x++)
            {
                GameObject slotGo = Instantiate(slotPrefab);
                slotGo.transform.SetParent(this.transform, false);
                slotGo.GetComponent<RectTransform>().anchoredPosition = beginSlotPos + new Vector2((slotWidth + paddingSlot) * x, (slotHeight + paddingSlot + 10f) * y * -1);
                slotGo.name = $"{slotPrefab.name}{slotIdx}";
                slotIdx++;

            }
        }
    }

    public void OpenUi()
    {
        if (itemSlots.Count == 0)
        {
            // Do nothing
        }
        else
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                nowInventory[i].itemData = itemSlotScripts[i].itemData;
                nowInventory[i].itemAmount = itemSlotScripts[i].itemAmount;
                nowInventory[i].itemUseDel = itemSlotScripts[i].itemUseDel;
            }

        }
    }

    public void CloseUi()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlotScripts[i].itemData = nowInventory[i].itemData;
            itemSlotScripts[i].itemAmount = nowInventory[i].itemAmount;
            itemSlotScripts[i].itemUseDel = nowInventory[i].itemUseDel;
        }
    }
}
