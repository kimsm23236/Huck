using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //! ���� Ÿ��
    public enum MonsterType
    {
        NOMAL = 0,
        ELITE,
        NAMEED,
        BOSS
    } // MonsterType

    [HideInInspector] public MonsterType monsterType;
    [HideInInspector] public string monsterName;
    [HideInInspector] public float monsterHp;
    [HideInInspector] public float monsterMaxHp;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float minDamage;
    [HideInInspector] public float maxDamage;
    [HideInInspector] public float searchRange;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float meleeAttackRange;

    //! ���� ������ �ʱ�ȭ�ϴ� �Լ�
    public void InitMonsterData(MonsterType _monsterType, MonsterData monsterData)
    {
        this.monsterType = _monsterType;
        this.monsterName = monsterData.MonsterName;
        this.monsterHp = monsterData.MonsterHp;
        this.monsterMaxHp = monsterData.MonsterMaxHp;
        this.moveSpeed = monsterData.MoveSpeed;
        this.minDamage = monsterData.MinDamage;
        this.maxDamage = monsterData.MaxDamage;
        this.searchRange = monsterData.SearchRange;
        this.attackRange = monsterData.AttackRange;
        this.meleeAttackRange = monsterData.MeleeAttackRange;
    } // InitMonsterData
}
