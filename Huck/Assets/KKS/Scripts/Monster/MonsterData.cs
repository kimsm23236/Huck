using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    private string monsterName; // ���� �̸�
    public string MonsterName { get { return monsterName; } }

    [SerializeField]
    private int monsterHp; // ���� HP
    public int MonsterHp { get { return monsterHp; } }

    [SerializeField]
    private int monsterMaxHp; // ���� MAX_HP
    public int MonsterMaxHp { get { return monsterMaxHp; } }

    [SerializeField]
    private float moveSpeed; // ���� �̵��ӵ�
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private int damage; // ���� �ּ� ���ݷ�
    public int Damage { get { return damage; } }

    [SerializeField]
    private bool isNoRangeAttack; // ���� ���Ÿ� ���� ����
    public bool IsNoRangeAttack { get { return isNoRangeAttack; } }

    [SerializeField]
    private bool isNoRangeSkill; // ���� ���Ÿ� ��ų ����
    public bool IsNoRangeSkill { get { return isNoRangeSkill; } }

    [SerializeField]
    private bool useSkill; // ���� ��ų ��밡�� üũ
    public bool UseSkill { get { return useSkill; } }

    [SerializeField]
    private float searchRange; // ���� Ž�� ����
    public float SearchRange { get { return searchRange; } }

    [SerializeField]
    private float attackRange; // ���� ���� ��Ÿ�
    public float AttackRange { get { return attackRange; } }

    [SerializeField]
    private float meleeAttackRange; // ���� �������� ��Ÿ�
    public float MeleeAttackRange { get { return meleeAttackRange; } }

    [SerializeField]
    private AudioClip roarAudio; // Roar ����
    public AudioClip RoarAudio { get { return roarAudio; } }

    [SerializeField]
    private AudioClip deadAudio; // Dead ����
    public AudioClip DeadAudio { get { return deadAudio; } }

    [SerializeField]
    private AudioClip moveAudio; // Move ����
    public AudioClip MoveAudio { get { return moveAudio; } }

    [SerializeField]
    private AudioClip hitAudio; // Hit ����
    public AudioClip HitAudio { get { return hitAudio; } }

    [SerializeField]
    private AudioClip weaponAudio; // Hit ����
    public AudioClip WeaponAudio { get { return weaponAudio; } }
} // MonsterData
