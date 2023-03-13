using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSoldier : Monster
{
    private MonsterController mController = default;
    public MonsterData monsterData;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.NOMAL, monsterData);
        mController.monster = this;
    } // Awake
}
