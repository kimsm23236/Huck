using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GFunc
{
    //! ��ųʸ� ������Ʈ �������� ĳ���ϴ� �Լ�
    public static void AddObjs(this Dictionary<string, GameObject> dict_, GameObject[] prefabs_)
    {
        foreach (var prefab_ in prefabs_) 
        {
            dict_.Add(prefab_.name, prefab_);
        }
    }
    //! ����Ʈ�� ���� �Լ�
    public static void Shuffle<T>(this List<T> targetList, int shuffleCnt = 0)
    {
        if(shuffleCnt.Equals(0)) { shuffleCnt = (int)(targetList.Count * 2.0f); }

        int sourIdx = 0;
        int destIdx = 0;
        T tempVar = default(T);

        for(int i = 0; i < shuffleCnt; i++)
        {
            sourIdx = Random.Range(0, targetList.Count);
            destIdx = Random.Range(0, targetList.Count);

            tempVar = targetList[sourIdx];
            targetList[sourIdx] = targetList[destIdx];
            targetList[destIdx] = tempVar;
        }
    }   // Shuffle()

    //! ����Ʈ�� element �� �ٸ� ���� Swap �ϴ� �Լ�
    public static void Swap<T>(this List<T> targetList, ref T swapValue, int swapIdx)
    {
        T tempValue = targetList[swapIdx];
        targetList[swapIdx] = swapValue;
        swapValue = tempValue;
    }   // Swap()

    //! int�� ���� ���� �ȿ� ���� �ִ��� �˻��ϴ� �Լ�
    public static bool IsInRange(this int targetValue, int minInclude, int maxExclude)
    {
        return (minInclude <= targetValue && targetValue < maxExclude);
    }

    //! float�� ���� ������ ���ϴ� �Լ�
    public static bool IsEquals(this float targetValue, float compareValue)
    {
        bool isEqual = Mathf.Approximately(targetValue, compareValue);
        return isEqual;
    }   // IsEquals()

}
