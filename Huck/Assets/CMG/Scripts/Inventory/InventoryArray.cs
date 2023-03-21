using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryArray : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> itemSlots = new List<GameObject>();
    public GameObject slotPrefab = null;


    #region 인벤토리 드래그 & 드랍을 위한 변수
    private List<RaycastResult> rayList = new List<RaycastResult>();
    private GraphicRaycaster graphicRay = default;
    private PointerEventData pointEvent = default;
    private Canvas myCanvas = default;
    private ItemSlot beginDragSlot = default;
    private Transform beginItemTrans = default;
    private GameObject dividedItemIcon = default;

    private Vector3 beginDragIconPoint;   // 드래그 시작 시 슬롯의 위치
    private Vector3 beginDragCursorPoint; // 드래그 시작 시 커서의 위치
    private int beginDragSlotSiblingIndex;
    private GameObject inventoryUi = default;

    #endregion


    private int horizonSlotCount = 8;
    private int verticalSlotCount = 4;
    private float paddingSlot = 10f;
    private float slotWidth = 0f;
    private float slotHeight = 0f;

    private Vector2 beginSlotPos = default;

    private void Awake()
    {
        InitSlots();
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            itemSlots.Add(transform.GetChild(i).gameObject);
        }
        myCanvas = transform.parent.parent.parent.GetComponent<Canvas>();
        graphicRay = myCanvas.GetComponent<GraphicRaycaster>();
        pointEvent = new PointerEventData(null);
    }

    // Update is called once per frame
    void Update()
    {
        pointEvent.position = Input.mousePosition;
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
        DividItem();
        DividDrag();
        DividDragEnd();
    }

    private void InitSlots()
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
                slotGo.GetComponent<RectTransform>().anchoredPosition = beginSlotPos + new Vector2((slotWidth + paddingSlot) * x, (slotHeight + paddingSlot) * y * -1);
                slotGo.name = $"{slotPrefab.name}{slotIdx}";
                slotIdx++;
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

    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && dividedItemIcon == null)
        {
            GameObject createItem = Instantiate(beginDragSlot.Item.originPrefab);
            createItem.transform.SetParent(GameManager.Instance.playerObj.transform.parent);
            createItem.transform.position = GameManager.Instance.playerObj.transform.position;
            createItem.GetComponent<Item>().itemCount = beginDragSlot.itemAmount;
            beginDragSlot.Item = default;
            beginDragSlot.itemAmount = 0;

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
        if (item_.itemData.itemType == ItemType.CombineAble)
        {
            bool isCombine = false;
            // 같은게 있는지 검사
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<ItemSlot>().Item != null && transform.GetChild(i).GetComponent<ItemSlot>().Item.itemName == item_.itemData.itemName)
                {
                    transform.GetChild(i).GetComponent<ItemSlot>().itemAmount += item_.itemCount;
                    isCombine = true;
                    break;
                } // 획득한 아이템과 같은 이름의 아이템이 있는지 확인
            }
            if (!isCombine)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<ItemSlot>().Item == null)
                    {
                        transform.GetChild(i).GetComponent<ItemSlot>().Item = item_.itemData;
                        transform.GetChild(i).GetComponent<ItemSlot>().itemAmount += item_.itemCount;
                        break;
                    }
                }
            } // 합쳐질 아이템이 없을 경우
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<ItemSlot>().Item != null)
                {
                    transform.GetChild(i).GetComponent<ItemSlot>().Item = item_.itemData;
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
        if (startItem != endItem && endItem.Item != null && startItem.Item.itemName == endItem.Item.itemName)
        {
            endItem.itemAmount += startItem.itemAmount;
            startItem.Item = default;
            startItem.itemAmount = 0;
        }
        else
        {
            ItemData tempItem = default;
            tempItem = startItem.Item;
            startItem.Item = endItem.Item;
            endItem.Item = tempItem;

            int tmepItemCount = 0;
            tmepItemCount = startItem.itemAmount;
            startItem.itemAmount = endItem.itemAmount;
            endItem.itemAmount = tmepItemCount;
        }
    }

    private void DividItem()
    {
        if (Input.GetMouseButtonUp(1) && dividedItemIcon == null)
        {
            ItemSlot dividSlot = RaycastGetFirstComponent<ItemSlot>();
            if (dividSlot != null && dividSlot.HasItem && dividSlot.Item.itemType == ItemType.CombineAble)
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
                    dividedItemIcon.GetComponent<ItemSlot>().Item = dividSlot.Item;
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
                GameObject createItem = Instantiate(dividedItemIcon.GetComponent<ItemSlot>().Item.originPrefab);
                createItem.transform.SetParent(GameManager.Instance.playerObj.transform.parent);
                createItem.transform.position = GameManager.Instance.playerObj.transform.position;
                createItem.GetComponent<Item>().itemCount = dividedItemIcon.GetComponent<ItemSlot>().itemAmount;
                Destroy(dividedItemIcon.gameObject);
            }
            else if (clickSlot != null)
            {
                if (clickSlot.HasItem && dividedItemIcon.GetComponent<ItemSlot>().Item.itemType == ItemType.CombineAble
                    && clickSlot.Item.itemName == dividedItemIcon.GetComponent<ItemSlot>().Item.itemName)
                {
                    clickSlot.itemAmount += dividedItemIcon.GetComponent<ItemSlot>().itemAmount;
                    Destroy(dividedItemIcon.gameObject);
                } // 아이템이 들어있고 합칠 수 있는 아이템이면서 이름이 같은 경우
                else if (clickSlot.HasItem && dividedItemIcon.GetComponent<ItemSlot>().Item.itemType == ItemType.NoneCombineAble
                        && clickSlot.Item.itemName != dividedItemIcon.GetComponent<ItemSlot>().Item.itemName)
                {
                    SwapItems(dividedItemIcon.GetComponent<ItemSlot>(), clickSlot);
                } // 아이템이 들어있고 합칠 수 없는 아이템이면서 이름이 같은 경우
                else if (clickSlot.HasItem && dividedItemIcon.GetComponent<ItemSlot>().Item.itemType == ItemType.CombineAble
                        && clickSlot.Item.itemName != dividedItemIcon.GetComponent<ItemSlot>().Item.itemName)
                {
                    SwapItems(dividedItemIcon.GetComponent<ItemSlot>(), clickSlot);
                } // 아이템이 들어있고 합칠 수 있으면서 이름이 다른 경우
                else
                {
                    clickSlot.Item = dividedItemIcon.GetComponent<ItemSlot>().Item;
                    clickSlot.itemAmount = dividedItemIcon.GetComponent<ItemSlot>().itemAmount;
                    Destroy(dividedItemIcon.gameObject);
                } // 아이템이 없는 경우
            }
        }
    }

}
