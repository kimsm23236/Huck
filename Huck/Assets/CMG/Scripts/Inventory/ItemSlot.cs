using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public bool HasItem { get; private set; } = false;
    public ItemData Item = default;
    public int itemAmount = 0;

    private GameObject itemAmountObj = default;
    private TMP_Text itemAmountText = default;
    private Color defaultAlpha = new Color(1f, 1f, 1f, 0f);
    private Color itemAlpha = new Color(1f, 1f, 1f, 1f);
    // Start is called before the first frame update
    void Start()
    {
        itemAmountObj = transform.GetChild(0).GetChild(0).gameObject;
        itemAmountText = itemAmountObj.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Item != null && Item != default)
        {
            if (Item.ItemType == EItemType.CombineAble)
            {
                transform.GetChild(0).GetComponent<Image>().sprite = Item.ItemIcon;
                transform.GetChild(0).GetComponent<Image>().color = itemAlpha;
                HasItem = true;
                ItemCountText();
            }
            else
            {
                transform.GetChild(0).GetComponent<Image>().sprite = Item.ItemIcon;
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
        if (Item.ItemType == EItemType.CombineAble)
        {
            itemAmountObj.SetActive(true);
            itemAmountText.text = $"{itemAmount}";
        }
    }


}
