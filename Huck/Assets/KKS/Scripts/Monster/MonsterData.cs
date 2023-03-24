using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    private string monsterName; // 몬스터 이름
    public string MonsterName { get { return monsterName; } }

    [SerializeField]
    private int monsterHp; // 몬스터 HP
    public int MonsterHp { get { return monsterHp; } }

    [SerializeField]
    private int monsterMaxHp; // 몬스터 MAX_HP
    public int MonsterMaxHp { get { return monsterMaxHp; } }

    [SerializeField]
    private float moveSpeed; // 몬스터 이동속도
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private int damage; // 몬스터 최소 공격력
    public int Damage { get { return damage; } }

    [SerializeField]
    private bool isNoRangeAttack; // 몬스터 원거리 공격 유무
    public bool IsNoRangeAttack { get { return isNoRangeAttack; } }

    [SerializeField]
    private bool isNoRangeSkill; // 몬스터 원거리 스킬 유무
    public bool IsNoRangeSkill { get { return isNoRangeSkill; } }

    [SerializeField]
    private bool useSkill; // 몬스터 스킬 사용가능 체크
    public bool UseSkill { get { return useSkill; } }

    [SerializeField]
    private float searchRange; // 몬스터 탐색 범위
    public float SearchRange { get { return searchRange; } }

    [SerializeField]
    private float attackRange; // 몬스터 공격 사거리
    public float AttackRange { get { return attackRange; } }

    [SerializeField]
    private float meleeAttackRange; // 몬스터 근접공격 사거리
    public float MeleeAttackRange { get { return meleeAttackRange; } }
} // MonsterData
