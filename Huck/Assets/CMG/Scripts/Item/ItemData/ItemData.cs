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

}
public enum EItemTool
{
    NONE = -1
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
