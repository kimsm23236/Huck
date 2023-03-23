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
    public Item itemClass = default;

    private GameObject itemAmountObj = default;
    private TMP_Text itemAmountText = default;
    private Color defaultAlpha = new Color(1f, 1f, 1f, 0f);
    private Color itemAlpha = new Color(1f, 1f, 1f, 1f);

    // 델리게이트로 지금 캐싱하고 있는 아이템이 사용되었을 때의 동작을 캐싱하고 있음.
    // 델리게이트 = default;

    // Start is called before the first frame update
    void Start()
    {
        itemAmountObj = transform.GetChild(0).GetChild(0).gameObject;
        itemAmountText = itemAmountObj.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (itemData != null && itemData != default)
        {
            if (itemData.ItemType == EItemType.CombineAble)
            {
                // 아이템의 종류를 이 시점에서 알 수 있음 -> 해당 아이템의 클래스도 가져올 수 있음.
                transform.GetChild(0).GetComponent<Image>().sprite = itemData.ItemIcon;
                transform.GetChild(0).GetComponent<Image>().color = itemAlpha;
                HasItem = true;
                ItemCountText();
            }
            else
            {
                transform.GetChild(0).GetComponent<Image>().sprite = itemData.ItemIcon;
                transform.GetChild(0).GetComponent<Image>().color = itemAlpha;
                HasItem = true;
                itemAmountObj.SetActive(false);
            }
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = default;
            transform.GetChild(0).GetComponent<Image>().color = defaultAlpha;
            HasItem = false;
            itemAmountObj.SetActive(false);
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
