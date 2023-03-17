using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryArray : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> itemSlots = new List<GameObject>();


    #region 인벤토리 드래그 & 드랍을 위한 변수
    private List<RaycastResult> rayList = new List<RaycastResult>();
    private GraphicRaycaster graphicRay = default;
    private PointerEventData pointEvent = default;
    private Canvas myCanvas = default;

    private ItemSlot beginDragSlot = default;
    private Transform beginItemTrans = default;

    private Vector3 beginDragIconPoint;   // 드래그 시작 시 슬롯의 위치
    private Vector3 beginDragCursorPoint; // 드래그 시작 시 커서의 위치
    private int beginDragSlotSiblingIndex;

    #endregion

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
        if (beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            // 위치 이동
            beginItemTrans.position =
                beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
        }
    }
    /// <summary> 클릭을 뗄 경우 </summary>
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // End Drag
            if (beginDragSlot != null)
            {
                // 위치 복원
                beginItemTrans.position = beginDragIconPoint;

                // UI 순서 복원
                beginDragSlot.transform.SetSiblingIndex(beginDragSlotSiblingIndex);

                // 드래그 완료 처리
                // EndDrag();

                // 참조 제거
                beginDragSlot = null;
                beginItemTrans = null;
            }
        }
    }

    public void AddItem(Item item_)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            // 비었는지 검사
            if (transform.GetChild(i).GetComponent<ItemSlot>().Item != null)
            { /* To Do (type검사해서 합칠수 있으면 합침) */ }
            // 같은게 있는지 검사
            // 비어있는 칸에 추가
            else
            {
                transform.GetChild(i).GetComponent<ItemSlot>().Item = item_.itemData;
                break;
            }
        }
    }
}
