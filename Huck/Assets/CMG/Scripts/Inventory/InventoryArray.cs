using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryArray : MonoBehaviour
{

    public List<GameObject> itemSlots = new List<GameObject>();
    public List<ItemSlot> itemSlotScripts = new List<ItemSlot>();
    public GameObject slotPrefab = null;
    private ItemSlot nowSlot = default;


    #region 인벤토리 드래그 & 드랍을 위한 변수
    private List<RaycastResult> rayList = new List<RaycastResult>();
    protected GraphicRaycaster graphicRay = default;
    protected PointerEventData pointEvent = default;
    protected Canvas myCanvas = default;
    private ItemSlot beginDragSlot = default;
    private Transform beginItemTrans = default;
    private GameObject dividedItemIcon = default;

    private Vector3 beginDragIconPoint;   // 드래그 시작 시 슬롯의 위치
    private Vector3 beginDragCursorPoint; // 드래그 시작 시 커서의 위치
    private int beginDragSlotSiblingIndex;
    private GameObject inventoryUi = default;
    #endregion

    protected int horizonSlotCount = 8;
    protected int verticalSlotCount = 4;
    protected float paddingSlot = 10f;
    protected float slotWidth = 0f;
    protected float slotHeight = 0f;

    protected Vector2 beginSlotPos = default;
    protected PlayerStat playerStat = default;
    protected Transform playerPos = default;

    protected virtual void Awake()
    {
        InitSlots();
        for (int i = 0; i < transform.childCount; i++)
        {
            itemSlots.Add(transform.GetChild(i).gameObject);
            itemSlotScripts.Add(itemSlots[i].GetComponent<ItemSlot>());
        }
        UIManager.Instance.inventory = this.gameObject;
        CanvasSetting();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        InvenSetting();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (PlayerOther.isInvenOpen)
        {
            ControlMouse();
        }
    }

    protected void ControlMouse()
    {
        pointEvent.position = Input.mousePosition;
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
        DividItem();
        DividDrag();
        DividDragEnd();
    }
    protected virtual void CanvasSetting()
    {
        myCanvas = transform.parent.parent.parent.GetComponent<Canvas>();
    }
    protected void InvenSetting()
    {
        graphicRay = myCanvas.GetComponent<GraphicRaycaster>();
        pointEvent = new PointerEventData(null);
        playerStat = GameManager.Instance.playerObj.GetComponent<PlayerStat>();
        playerPos = GameManager.Instance.playerObj.transform;
    }

    protected virtual void InitSlots()
    {
        slotWidth = slotPrefab.GetComponent<RectTransform>().sizeDelta.x;
        slotHeight = slotPrefab.GetComponent<RectTransform>().sizeDelta.y;
        beginSlotPos = new Vector2(slotWidth / 2 + paddingSlot, slotHeight / -2 - paddingSlot);
        int slotIdx = 1;
        for (int y = 0; y < verticalSlotCount; y++)
        {
            for (int x = 0; x < horizonSlotCount; x++)
            {
                if (y == verticalSlotCount - 1)
                {
                    GameObject slotGo = Instantiate(slotPrefab);
                    slotGo.transform.SetParent(this.transform, false);
                    slotGo.GetComponent<RectTransform>().anchoredPosition = beginSlotPos + new Vector2((slotWidth + paddingSlot) * x, (slotHeight + paddingSlot + 10f) * y * -1);
                    slotGo.name = $"{slotPrefab.name}{slotIdx}";
                    slotIdx++;
                }
                else
                {
                    GameObject slotGo = Instantiate(slotPrefab);
                    slotGo.transform.SetParent(this.transform, false);
                    slotGo.GetComponent<RectTransform>().anchoredPosition = beginSlotPos + new Vector2((slotWidth + paddingSlot) * x, (slotHeight + paddingSlot) * y * -1);
                    slotGo.name = $"{slotPrefab.name}{slotIdx}";
                    slotIdx++;
                }
            }
        }
    }
    private T RaycastGetFirstComponent<T>() where T : Component
    {
        rayList.Clear();
        graphicRay.Raycast(pointEvent, rayList);
        if (rayList.Count == 0)
        {
            return null;
        }
        return rayList[0].gameObject.GetComponent<T>();
    }

    private void OnPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            beginDragSlot = RaycastGetFirstComponent<ItemSlot>();

            if (beginDragSlot != null && beginDragSlot.HasItem)
            {
                beginItemTrans = beginDragSlot.transform;
                beginDragIconPoint = beginItemTrans.position;
                beginDragCursorPoint = Input.mousePosition;

                // 맨 위에 보이기
                beginDragSlotSiblingIndex = beginDragSlot.transform.GetSiblingIndex();
                beginDragSlot.transform.SetAsLastSibling();
            }
        }
    }


    private void OnPointerDrag()
    {
        if (beginDragSlot == null || beginDragSlot == default) return;

        if (Input.GetMouseButton(0) && beginItemTrans != null)
        {
            // 위치 이동
            beginItemTrans.GetChild(0).position = beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
        }
    }

    private void DropItem(ItemSlot itemSlot_)
    {
        GameObject createItem = default;
        // RaycastHit[] hit = Physics.RaycastAll(createItem.transform.position, playerPos.forward, 3f);
        RaycastHit hit = default;
        if (Physics.Raycast(playerPos.position + Vector3.up, playerPos.forward, out hit, 3f, LayerMask.GetMask(GData.TERRAIN_MASK)) == true)
        {
            createItem = Instantiate(itemSlot_.itemData.OriginPrefab);
            createItem.transform.SetParent(GameManager.Instance.playerObj.transform.parent);
            createItem.transform.position = hit.point + Vector3.up;
        }
        else
        {
            createItem = Instantiate(itemSlot_.itemData.OriginPrefab);
            createItem.transform.SetParent(GameManager.Instance.playerObj.transform.parent);
            createItem.transform.position = SetItemPos(playerPos, 3f);
        }

        createItem.GetComponent<Item>().itemCount = itemSlot_.itemAmount;
        itemSlot_.itemData = default;
        itemSlot_.itemAmount = 0;
        itemSlot_.itemUseDel = default;
        if (dividedItemIcon != null)
        {
            Destroy(dividedItemIcon.gameObject);
        }
    }

    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0) && beginDragSlot != null && beginDragSlot.itemData != null && !EventSystem.current.IsPointerOverGameObject() && dividedItemIcon == null)
        {
            DropItem(beginDragSlot);
            // 위치 복원
            beginItemTrans.position = beginDragIconPoint;

            // UI 순서 복원
            beginDragSlot.transform.SetSiblingIndex(beginDragSlotSiblingIndex);

            // 드래그 완료 처리
            EndDrag();

            // 참조 제거
            beginDragSlot = null;
            beginItemTrans = null;
        }
        else if (Input.GetMouseButtonUp(0) && beginItemTrans != null)
        {
            // End Drag
            // 위치 복원
            beginItemTrans.position = beginDragIconPoint;

            // UI 순서 복원
            beginDragSlot.transform.SetSiblingIndex(beginDragSlotSiblingIndex);

            // 드래그 완료 처리
            EndDrag();

            // 참조 제거
            beginDragSlot = null;
            beginItemTrans = null;
        }
    }

    public void AddItem(Item item_)
    {
        if (item_.itemData.ItemType == EItemType.CombineAble)
        {
            bool isCombine = false;
            // 같은게 있는지 검사
            for (int i = 0; i < transform.childCount; i++)
            {
                nowSlot = itemSlotScripts[i];
                if (nowSlot.itemData != null && nowSlot.itemData.ItemName == item_.itemData.ItemName)
                {
                    nowSlot.itemAmount += item_.itemCount;
                    isCombine = true;

                    break;
                } // 획득한 아이템과 같은 이름의 아이템이 있는지 확인
            }
            if (!isCombine)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    nowSlot = itemSlotScripts[i];
                    if (nowSlot.itemData == null)
                    {
                        nowSlot.itemData = item_.itemData;
                        nowSlot.itemAmount += item_.itemCount;
                        nowSlot.itemUseDel = item_.OnUse;
                        break;
                    }
                }
            } // 합쳐질 아이템이 없을 경우
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                nowSlot = itemSlotScripts[i];
                if (nowSlot.itemData == null)
                {
                    nowSlot.itemData = item_.itemData;
                    nowSlot.itemAmount += item_.itemCount;
                    nowSlot.itemUseDel = item_.OnUse;
                    break;
                }
            }
        } // 합쳐질 수 없는 아이템의 경우

    }

    private void EndDrag()
    {
        ItemSlot endDragSlot = RaycastGetFirstComponent<ItemSlot>();
        if (endDragSlot != null)
        {
            SwapItems(beginDragSlot, endDragSlot);
            beginItemTrans.GetChild(0).position = beginItemTrans.position;
        }
        else
        {
            beginItemTrans.GetChild(0).position = beginItemTrans.position;
        }
    }
    private void SwapItems(ItemSlot startItem, ItemSlot endItem)
    {
        if (startItem != endItem && endItem.itemData != null && startItem.itemData.ItemName == endItem.itemData.ItemName && endItem.itemData.ItemType == EItemType.CombineAble)
        {
            endItem.itemAmount += startItem.itemAmount;
            startItem.itemData = default;
            startItem.itemAmount = 0;
            startItem.itemUseDel = default;
        }
        else
        {
            (startItem.itemData, endItem.itemData) = (endItem.itemData, startItem.itemData);
            (startItem.itemAmount, endItem.itemAmount) = (endItem.itemAmount, startItem.itemAmount);
            (startItem.itemUseDel, endItem.itemUseDel) = (endItem.itemUseDel, startItem.itemUseDel);
        }
    }

    private void DividItem()
    {
        if (Input.GetMouseButtonUp(1) && dividedItemIcon == null)
        {
            ItemSlot dividSlot = RaycastGetFirstComponent<ItemSlot>();
            if (dividSlot != null && dividSlot.HasItem && dividSlot.itemData.ItemType == EItemType.CombineAble)
            {
                if (dividSlot.itemAmount <= 1)
                {
                    // Do nothing
                }
                else
                {
                    int dividedItemAmount = 0;
                    dividedItemAmount = dividSlot.itemAmount / 2;
                    dividSlot.itemAmount -= dividedItemAmount;
                    dividedItemIcon = Instantiate(slotPrefab);
                    dividedItemIcon.transform.SetParent(this.transform, false);
                    dividedItemIcon.GetComponent<ItemSlot>().itemAmount = dividedItemAmount;
                    dividedItemIcon.GetComponent<ItemSlot>().itemData = dividSlot.itemData;
                    dividedItemIcon.GetComponent<Image>().raycastTarget = false;
                    dividedItemIcon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                    // dividedItemIcon.GetComponent<Image>().sprite = dividSlot.Item.itemIcon;
                }
            }
        }
    }

    private void DividDrag()
    {
        if (dividedItemIcon != null)
        {
            dividedItemIcon.transform.position = Input.mousePosition;
        }
    }

    private void DividDragEnd()
    {
        if (Input.GetMouseButtonUp(0) && dividedItemIcon != null)
        {
            ItemSlot clickSlot = RaycastGetFirstComponent<ItemSlot>();
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                DropItem(dividedItemIcon.GetComponent<ItemSlot>());
            }
            else if (clickSlot != null)
            {
                if (clickSlot.HasItem && dividedItemIcon.GetComponent<ItemSlot>().itemData.ItemType == EItemType.CombineAble
                    && clickSlot.itemData.ItemName == dividedItemIcon.GetComponent<ItemSlot>().itemData.ItemName)
                {
                    clickSlot.itemAmount += dividedItemIcon.GetComponent<ItemSlot>().itemAmount;
                    Destroy(dividedItemIcon.gameObject);
                } // 아이템이 들어있고 합칠 수 있는 아이템이면서 이름이 같은 경우
                else if (clickSlot.HasItem && dividedItemIcon.GetComponent<ItemSlot>().itemData.ItemType == EItemType.NoneCombineAble
                        && clickSlot.itemData.ItemName != dividedItemIcon.GetComponent<ItemSlot>().itemData.ItemName)
                {
                    SwapItems(dividedItemIcon.GetComponent<ItemSlot>(), clickSlot);
                } // 아이템이 들어있고 합칠 수 없는 아이템이면서 이름이 같은 경우
                else if (clickSlot.HasItem && dividedItemIcon.GetComponent<ItemSlot>().itemData.ItemType == EItemType.CombineAble
                        && clickSlot.itemData.ItemName != dividedItemIcon.GetComponent<ItemSlot>().itemData.ItemName)
                {
                    SwapItems(dividedItemIcon.GetComponent<ItemSlot>(), clickSlot);
                } // 아이템이 들어있고 합칠 수 있으면서 이름이 다른 경우
                else
                {
                    clickSlot.itemData = dividedItemIcon.GetComponent<ItemSlot>().itemData;
                    clickSlot.itemAmount = dividedItemIcon.GetComponent<ItemSlot>().itemAmount;
                    Destroy(dividedItemIcon.gameObject);
                } // 아이템이 없는 경우
            }
        }
    }

    private Vector3 SetItemPos(Transform playerPos_, float distance)
    {
        Vector3 itemPos = playerPos_.position + new Vector3(0f, 1f, 0f) + (playerPos_.forward * distance);
        return itemPos;
    }
}
