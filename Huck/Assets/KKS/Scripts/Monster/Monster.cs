using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //! 몬스터 타입
    public enum MonsterType
    {
        MELEE = 0,
        RANGE
    } // MonsterType

    [HideInInspector] public MonsterType monsterType;
    [HideInInspector] public string monsterName;
    [HideInInspector] public int monsterHp;
    [HideInInspector] public int monsterMaxHp;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public int damage;
    [HideInInspector] public bool isNoRangeAttack;
    [HideInInspector] public bool isNoRangeSkill;
    [HideInInspector] public bool useSkill;
    [HideInInspector] public float searchRange;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float meleeAttackRange;

    //! 몬스터 데이터 초기화하는 함수
    public void InitMonsterData(MonsterType _monsterType, MonsterData monsterData)
    {
        this.monsterType = _monsterType;
        this.monsterName = monsterData.MonsterName;
        this.monsterHp = monsterData.MonsterHp;
        this.monsterMaxHp = monsterData.MonsterMaxHp;
        this.moveSpeed = monsterData.MoveSpeed;
        this.damage = monsterData.Damage;
        this.isNoRangeAttack = monsterData.IsNoRangeAttack;
        this.isNoRangeSkill = monsterData.IsNoRangeSkill;
        this.useSkill = monsterData.UseSkill;
        this.searchRange = monsterData.SearchRange;
        this.attackRange = monsterData.AttackRange;
        this.meleeAttackRange = monsterData.MeleeAttackRange;
    } // InitMonsterData

    //! 공격 함수
    public virtual void Attack()
    {
        /* Do Nothing */
    } // Attack

    //! 스킬공격 함수
    public virtual void Skill()
    {
        /* Do Nothing */
    } // Skill

    
} // Monster
