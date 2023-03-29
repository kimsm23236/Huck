using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private EItemType itemType;
    public EItemType ItemType
    {
        get { return itemType; }
    }

    [SerializeField]
    private EItemLevel itemLevel;
    public EItemLevel ItemLevel
    {
        get { return itemLevel; }
    }

    [SerializeField]
    private EItemTool itemTool;
    public EItemTool ItemTool
    {
        get { return itemTool; }
    }

    [SerializeField]
    private string itemName;
    public string ItemName
    {
        get { return itemName; }
    }

    [SerializeField]
    private Sprite itemIcon;
    public Sprite ItemIcon
    {
        get { return itemIcon; }
    }

    [SerializeField]
    private GameObject originPrefab;
    public GameObject OriginPrefab
    {
        get { return originPrefab; }
    }

    [SerializeField]
    private ItemData resultData;
    public ItemData ResultData
    {
        get { return resultData; }
    }

    [SerializeField]
    private bool itemUseAble;
    public bool ItemUseAble
    {
        get { return itemUseAble; }
    }

    [SerializeField]
    private int itemDamage;
    public int ItemDamage
    {
        get { return itemDamage; }
    }

    [SerializeField]
    private bool isBuild;
    public bool IsBuild
    {
        get { return isBuild; }
    }


    [SerializeField]
    private bool isFuel;
    public bool IsFuel
    {
        get { return isFuel; }
    }

    public virtual void OnUseData(ItemSlot itemSlot_)
    {
        if (itemUseAble)
        {
            itemSlot_.itemAmount--;
        }
    }

}
public enum EItemTool
{
    NONE = 0,
    AXE = 1,
    PICKAXE = 2
}

public enum EItemType
{
    NONE = -1,
    CombineAble,
    NoneCombineAble
}

public enum EItemLevel
{
    NONE = -1,
    LOW,
    MEDIUM,
    HIGH
}
