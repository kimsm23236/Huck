using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitSlot : MonoBehaviour
{
    [SerializeField]
    private GameObject inven = default;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ItemSlot>().Item = inven.GetComponent<InventoryArray>().itemSlots[24 + i].GetComponent<ItemSlot>().Item;
            transform.GetChild(i).GetComponent<ItemSlot>().itemAmount = inven.GetComponent<InventoryArray>().itemSlots[24 + i].GetComponent<ItemSlot>().itemAmount;
        }
    }
}
