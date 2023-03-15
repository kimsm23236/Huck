using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    private string monsterName; //���� �̸�
    public string MonsterName { get { return monsterName; } }

    [SerializeField]
    private float monsterHp; //���� HP
    public float MonsterHp { get { return monsterHp; } }

    [SerializeField]
    private float monsterMaxHp; //���� MAX_HP
    public float MonsterMaxHp { get { return monsterMaxHp; } }

    [SerializeField]
    private float moveSpeed; //���� �̵��ӵ�
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private float minDamage; //���� �ּ� ���ݷ�
    public float MinDamage { get { return minDamage; } }

    [SerializeField]
    private float maxDamage; //���� �ִ� ���ݷ�
    public float MaxDamage { get { return maxDamage; } }

    [SerializeField]
    private float skillACooldown; //���� ��ųA ��ٿ�
    public float SkillACooldown { get { return skillACooldown; } }

    [SerializeField]
    private float skillBCooldown; //���� ��ųB ��ٿ�
    public float SkillBCooldown { get { return skillBCooldown; } }

    [SerializeField]
    private float searchRange; //���� Ž�� ����
    public float SearchRange { get { return searchRange; } }

    [SerializeField]
    private float attackRange; //���� ���� ��Ÿ�
    public float AttackRange { get { return attackRange; } }

    [SerializeField]
    private float meleeAttackRange; //���� �������� ��Ÿ�
    public float MeleeAttackRange { get { return meleeAttackRange; } }

    //! ���� ��ų Ÿ��
    public enum SkillType
    {
        MELEE = 0,
        RANGE
    } // SkillType

    [SerializeField]
    private SkillType skillAType; // ���� ��ųA Ÿ��
    public SkillType SkillAType { get { return skillAType; } }

    [SerializeField]
    private SkillType skillBType; // ���� ��ųB Ÿ��
    public SkillType SkillBType { get { return skillBType; } }
}
