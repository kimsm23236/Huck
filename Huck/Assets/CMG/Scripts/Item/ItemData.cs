using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public int itemCount;
    public Transform parentsTransform;
    public Sprite itemIcon;
}

public enum ItemType
{
    None = -1, CombineAble, NoneCombineAble
}