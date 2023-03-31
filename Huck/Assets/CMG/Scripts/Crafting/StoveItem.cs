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
    public ItemData prevItemData = default;

    public float percentage = 0f;
    public bool isUsing = false;
    public float fuelEnergy = 0f;

    private bool useCor = false;

    public bool isChange = false;

    private void Update()
    {
        if (prevItemData != originItemData)
        {
            prevItemData = originItemData;
            percentage = 0f;
            StopCoroutine(DelayStove());
        }
        if (fuelEnergy <= 0)
        {
            isUsing = false;
        }
        DescFuelEnergy();
        UseStove();
        // originItemCount 혹은 fuelItemCount 혹은 resultItemCount가 값이 변할때 체크
        if (isUsing)
        {
            if (originItemData == null || originItemData.ResultData == null || (resultItemData != null && resultItemData.ItemName != originItemData.ResultData.ItemName))
            {
                percentage = 0f;
                StopCoroutine(DelayStove());
            }
            else if (!useCor && originItemCount != 0)
            {
                StartCoroutine(DelayStove());
            }
        }
    }
    // 진행도가 100이상이면  originItem.Itemdata에 있는 itemResult를 Result칸에 추가하고 originitem의 갯수를 -1 연료의 퍼센트가 없으면 fuel의 개수 -1


    public void UseStove()
    {

        if (originItemData != null && fuelItemData != null)
        {
            if (originItemData.ResultData != null && fuelItemData.IsFuel)
            {
                if (resultItemData == null)
                {
                    if (fuelEnergy <= 0)
                    {
                        fuelItemCount--;
                        isChange = true;
                        fuelEnergy = 100;
                    }
                }
                else
                {
                    if (resultItemData.ItemName == originItemData.ItemName)
                    {
                        if (fuelEnergy <= 0)
                        {
                            fuelItemCount--;
                            isChange = true;
                            fuelEnergy = 100;
                        }
                    }
                    else
                    {
                        isUsing = false;
                        //Do nothing
                    }
                }
            }
        }
        // if ((originItemData == null || originItemData.ResultData != null) && isUsing)
        // {
        //     percentage = 0f;
        //     prevItemData = originItemData;
        //     StopCoroutine(DelayStove());
        // }

        if (fuelEnergy > 0)
        {
            isUsing = true;
        }
    }

    private void DescFuelEnergy()
    {
        if (fuelEnergy > 0)
        {
            fuelEnergy -= 3.34f * Time.deltaTime;
        }
    }

    public IEnumerator DelayStove()
    {
        if (percentage >= 100f || !isUsing)
        {
            percentage = 0f;
            originItemCount--;
            if (resultItemData == null)
            {
                resultItemData = originItemData.ResultData;
                resultItemCount++;
                isChange = true;
            }
            else if (resultItemData.ItemName == originItemData.ResultData.ItemName)
            {
                resultItemCount++;
                isChange = true;
            }
            yield break;
        }
        useCor = true;
        yield return new WaitForSeconds(0.5f);
        percentage += 10f;
        useCor = false;
    }
}
