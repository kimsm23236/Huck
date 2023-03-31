using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropable
{
    public void DropItem(List<DropItemConfig> dropItems, Transform targetTransform)
    {
        /* virtual method */
    }
}
