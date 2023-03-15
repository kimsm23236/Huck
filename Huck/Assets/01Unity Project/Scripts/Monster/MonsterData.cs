using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    private string monsterName; //몬스터 이름
    public string MonsterName { get { return monsterName; } }

    [SerializeField]
    private float monsterHp; //몬스터 HP
    public float MonsterHp { get { return monsterHp; } }

    [SerializeField]
    private float monsterMaxHp; //몬스터 MAX_HP
    public float MonsterMaxHp { get { return monsterMaxHp; } }

    [SerializeField]
    private float moveSpeed; //몬스터 이동속도
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private float minDamage; //몬스터 최소 공격력
    public float MinDamage { get { return minDamage; } }

    [SerializeField]
    private float maxDamage; //몬스터 최대 공격력
    public float MaxDamage { get { return maxDamage; } }

    [SerializeField]
    private bool skillA; //몬스터 스킬A 존재 유무
    public bool SkillA { get { return skillA; } }

    [SerializeField]
    private bool skillB; //몬스터 스킬B 존재 유무
    public bool SkillB { get { return skillB; } }

    [SerializeField]
    private float skillA_MaxCooldown; //몬스터 스킬A 쿨다운
    public float SkillA_MaxCooldown { get { return skillA_MaxCooldown; } }

    [SerializeField]
    private float skillB_MaxCooldown; //몬스터 스킬B 쿨다운
    public float SkillB_MaxCooldown { get { return skillB_MaxCooldown; } }

    [SerializeField]
    private float searchRange; //몬스터 탐색 범위
    public float SearchRange { get { return searchRange; } }

    [SerializeField]
    private float attackRange; //몬스터 공격 사거리
    public float AttackRange { get { return attackRange; } }

    [SerializeField]
    private float meleeAttackRange; //몬스터 근접공격 사거리
    public float MeleeAttackRange { get { return meleeAttackRange; } }
}
