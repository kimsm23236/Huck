using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public bool HasItem { get; private set; } = false;
    public ItemData itemData = default;
    public int itemAmount = 0;

    private bool isEmpty = true;
    private GameObject itemAmountObj = default;
    private Image itemIconImg = default;
    private TMP_Text itemAmountText = default;
    private Color defaultAlpha = new Color(1f, 1f, 1f, 0f);
    private Color itemAlpha = new Color(1f, 1f, 1f, 1f);

    public delegate void OnUseDel(ItemSlot itemSlot_);
    public OnUseDel itemUseDel = default;
    // 델리게이트로 지금 캐싱하고 있는 아이템이 사용되었을 때의 동작을 캐싱하고 있음.
    // 델리게이트 = default;

    private PlayerStat playerStat = default;
    // Start is called before the first frame update
    void Start()
    {
        itemAmountObj = transform.GetChild(0).GetChild(0).gameObject;
        itemAmountText = itemAmountObj.GetComponent<TMP_Text>();
        itemIconImg = transform.GetChild(0).GetComponent<Image>();
        playerStat = GameManager.Instance.playerObj.GetComponent<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (itemData != null && itemData != default)
        {
            if (itemData.ItemType == EItemType.CombineAble)
            {
                // 아이템의 종류를 이 시점에서 알 수 있음 -> 해당 아이템의 클래스도 가져올 수 있음.
                itemIconImg.sprite = itemData.ItemIcon;
                itemIconImg.color = itemAlpha;
                HasItem = true;
                isEmpty = false;
                ItemCountText();
            } // 합칠 수 있는 아이템일 경우
            else
            {
                itemIconImg.sprite = itemData.ItemIcon;
                itemIconImg.color = itemAlpha;
                HasItem = true;
                isEmpty = false;
                itemAmountObj.SetActive(false);
            } // 합칠 수 없는 아이템일 경우

            if (itemAmount <= 0)
            {
                itemIconImg.sprite = default;
                itemIconImg.color = defaultAlpha;
                HasItem = false;
                itemData = default;
                itemUseDel = default;
                itemAmountObj.SetActive(false);
            }
        } // 아이템 데이터가 있는지 없는지 판단
        else
        {
            if (!isEmpty)
            {
                itemIconImg.sprite = default;
                itemIconImg.color = defaultAlpha;
                itemAmountObj.SetActive(false);
                HasItem = false;
                itemData = default;
                isEmpty = true;
            }
        }
    }
    private void ItemCountText()
    {
        if (itemData.ItemType == EItemType.CombineAble)
        {
            itemAmountObj.SetActive(true);
            itemAmountText.text = $"{itemAmount}";
        }
    }


}
