using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSoldier : Monster
{
    public MonsterData monsterData;
    void Awake()
    {
        InitMonsterData(MonsterType.MELEE, monsterData);
    } // Awake

    // Update is called once per frame
    void Update()
    {

    } // Update
}
