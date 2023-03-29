using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveItem : MonoBehaviour
{

    public ItemData originItemData = default;
    public ItemData fuelItemData = default;
    public ItemData resultItemData = default;

    public int originItemCount = 0;
    public int fuelItemCount = 0;
    public int resultItemCount = 0;
    // 진행도가 100이상이면  originItem.Itemdata에 있는 itemResult를 Result칸에 추가하고 originitem의 갯수를 -1 연료의 퍼센트가 없으면 fuel의 개수 -1


    private void UseStove()
    {
        if (originItemData != null && fuelItemData != null)
        {
            if (originItemData.ResultData != null && fuelItemData.IsFuel)
            {
                if (resultItemData == null)
                {
                    Debug.Log("화로 사용중");
                }
                else
                {
                    if (resultItemData.ItemName == originItemData.ItemName)
                    {
                        Debug.Log("화로 사용중");
                    }
                    else
                    {
                        //Do nothing
                    }
                }
            }
        }
    }
}
