using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //! ���� Ÿ��
    public enum MonsterType
    {
        NOMAL = 0,
        NAMEED,
        BOSS
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
    [HideInInspector] public AudioClip roarClip;
    [HideInInspector] public AudioClip deadClip;
    [HideInInspector] public AudioClip moveClip;
    [HideInInspector] public AudioClip hitClip;
    [HideInInspector] public AudioClip weaponClip;

    //! ���� ������ �ʱ�ȭ�ϴ� �Լ�
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
        this.roarClip = monsterData.RoarAudio;
        this.deadClip = monsterData.DeadAudio;
        this.moveClip = monsterData.MoveAudio;
        this.hitClip = monsterData.HitAudio;
        this.weaponClip = monsterData.WeaponAudio;
    } // InitMonsterData

    //! ���� �Լ�
    public virtual void Attack()
    {
        /* Do Nothing */
    } // Attack

    //! ��ų���� �Լ�
    public virtual void Skill()
    {
        /* Do Nothing */
    } // Skill

    //! �������� �Լ�
    public virtual void ExitAttack()
    {
        /* Do Nothing */
    } // ExitAttack

    //! �������� ����ó�� �Լ�
    public virtual void BossDead()
    {
        /* Do Nothing */
    } // BossDead
} // Monster
