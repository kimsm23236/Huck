using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftInven : InventoryArray
{
    public InventoryArray inven = default;
    private List<ItemSlot> nowInventory = new List<ItemSlot>();
    [SerializeField]
    private bool isOpen = false;
    private bool Once = false;

    protected override void Awake()
    {
        InitSlots();
    }

    protected override void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            nowInventory.Add(transform.GetChild(i).GetComponent<ItemSlot>());
        }
        for (int i = 0; i < inven.transform.childCount; i++)
        {
            itemSlots.Add(inven.transform.GetChild(i).gameObject);
            itemSlotScripts.Add(inven.itemSlots[i].GetComponent<ItemSlot>());
        }
        myCanvas = transform.parent.parent.parent.parent.GetComponent<Canvas>();
        graphicRay = myCanvas.GetComponent<GraphicRaycaster>();
        pointEvent = new PointerEventData(null);
        playerStat = GameManager.Instance.playerObj.GetComponent<PlayerStat>();
        playerPos = GameManager.Instance.playerObj.transform;
    }

    protected override void Update()
    {
        base.Update();
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
        for (int i = 0; i < itemSlots.Count; i++)
        {
            nowInventory[i].itemData = itemSlotScripts[i].itemData;
            nowInventory[i].itemAmount = itemSlotScripts[i].itemAmount;
        }
    }

    public void CloseUi()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlotScripts[i].itemData = nowInventory[i].itemData;
            itemSlotScripts[i].itemAmount = nowInventory[i].itemAmount;
        }
    }
}
