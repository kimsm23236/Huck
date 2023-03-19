using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public bool HasItem { get; private set; } = false;
    public ItemData Item = default;
    public int itemAmount = 0;


    private Color defaultAlpha = new Color(1f, 1f, 1f, 0f);
    private Color itemAlpha = new Color(1f, 1f, 1f, 1f);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Item != null && Item != default)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = Item.itemIcon;
            transform.GetChild(0).GetComponent<Image>().color = itemAlpha;
            HasItem = true;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = default;
            transform.GetChild(0).GetComponent<Image>().color = defaultAlpha;
            HasItem = false;
        }
    }


}
