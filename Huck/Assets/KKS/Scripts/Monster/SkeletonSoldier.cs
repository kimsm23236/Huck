using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkeletonSoldier : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private GameObject shield = default;
    [SerializeField] private bool useSkillA = default;
    [SerializeField] private bool useSkillB = default;
    [SerializeField] private float skillA_MaxCool = default;
    [SerializeField] private float skillB_MaxCool = default;
    private int defaultDamage = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.NOMAL, monsterData);
        mController.monster = this;
        defaultDamage = damage;
        CheckUseSkill();
    } // Awake

    //! �ذ񺴻� ���� �������̵�
    public override void Attack()
    {
        //��� 2�� �� �������� �Ѱ� ����
        int number = Random.Range(0, 10);
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (number <= 6)
        {
            mController.monsterAni.SetBool("isAttackA", true);
        }
        else
        {
            mController.monsterAni.SetBool("isAttackB", true);
        }
    } // Attack

    //! �ذ񺴻� ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);

        if (useSkillA == true && mController.distance >= 13f)
        {
            useSkillA = false;
            CheckUseSkill();
            SkillA();
            return;
        }
        else if (useSkillA == true && mController.distance < 13f)
        {
            useSkillA = false;
            CheckUseSkill();
            // ������ų�� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ������ų ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckSkillADistance());
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (useSkillB == true)
        {
            useSkillB = false;
            CheckUseSkill();
            SkillB();
            return;
        }
    } // SKILL

    //! ��밡���� ��ų�� �ִ��� üũ�ϴ� �Լ� (������Ʈ�ѷ����� �������� üũ�ϱ� ����)
    private void CheckUseSkill()
    {
        if (useSkillA == false && useSkillB == false)
        {
            useSkill = false;
        }
        else
        {
            useSkill = true;
        }
    } // CheckUseSkill

    //! { �ذ񺴻� �׸� region ����
    #region ���� ó�� (Collider)
    //! ���� ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! ���� ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableShield()
    {
        shield.SetActive(true);
    } // EnableShield

    //! �������� �̺�Ʈ�Լ�
    public override void ExitAttack()
    {
        damage = defaultDamage;
        weapon.SetActive(false);
        shield.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA_Start", false);
        mController.monsterAni.SetBool("isSkillA_Loop", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        mController.monsterAni.SetBool("isSkillB", false);
        // �������� �� ������ ����
        mController.isDelay = true;
    } // ExitAttack
    #endregion // ���� ó�� (Collider, RayCast)

    #region ��ųA (����)
    //! ��ųA �Լ� (���� ����)
    private void SkillA()
    {
        StartCoroutine(UseSkillA());
    } // SkillA

    //! ��ųA ���� ��� �Ÿ�üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckSkillADistance()
    {
        isNoRangeSkill = true;
        while (isNoRangeSkill == true)
        {
            // Ÿ���� ���� �ּһ�Ÿ� �ۿ� ������ ���� ��밡��
            if (mController.distance >= 13f)
            {
                useSkillA = true;
                isNoRangeSkill = false;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // CheckSkillADistance

    //! ��ųA ���� ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillA()
    {
        // ���� ��Ÿ�� ����
        StartCoroutine(SkillACooldown());
        // ���� ���� �� �Լ� ����
        mController.monsterAni.SetTrigger("isRoar");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        mController.monsterAni.SetBool("isSkillA_Start", true);
        // ���� �غ� ��� ������ ���� ����
        bool isStart = true;
        bool isSkillA = true;
        mController.mAgent.speed = moveSpeed * 2f;
        while (isSkillA == true)
        {
            if (mController.targetSearch.hit == null)
            {
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                yield break;
            }
            if (mController.monsterAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && isStart == true)
            {
                // ���� ���� ��� ������ Loop������� ��ȯ
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", true);
                isStart = false;
            }
            mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            // ���� �� Ÿ���� �������ݻ�Ÿ� ���̶�� ���� ������ ����
            if (mController.distance <= meleeAttackRange)
            {
                damage = Mathf.FloorToInt(defaultDamage * 1.5f);
                mController.mAgent.speed = moveSpeed;
                mController.mAgent.ResetPath();
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = false;
            }
            yield return null;
        }
    } // UseSkillA

    //! ��ųA ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillACooldown()
    {
        skillACool = 0f;
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų ��밡��
        isNoRangeSkill = true;
        while (skillACool < skillA_MaxCool)
        {
            skillACool += Time.deltaTime;
            yield return null;
        }
        skillACool = 0f;
        useSkillA = true;
        isNoRangeSkill = false;
        CheckUseSkill();
    } // SkillACooldown
    #endregion //��ųA (����)

    #region ��ųB (���� ����)
    //! ��ųB �Լ� (���� ����)
    private void SkillB()
    {
        // ��ų ������ ����
        damage = Mathf.FloorToInt(defaultDamage * 0.7f);
        mController.monsterAni.SetBool("isSkillB", true);
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! ��ųB ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillBCooldown()
    {
        skillBCool = 0f;
        while (skillBCool < skillB_MaxCool)
        {
            skillBCool += Time.deltaTime;
            yield return null;
        }
        skillBCool = 0f;
        useSkillB = true;
        CheckUseSkill();
    } // SkillBCooldown
    #endregion // ��ųB (���� ����)

    #region ���� ����
    private void RoarSound()
    {
        mController.monsterAudio.clip = roarClip;
        mController.monsterAudio.Play();
    } // RoarSound
    private void DeadSound()
    {
        mController.monsterAudio.clip = deadClip;
        mController.monsterAudio.Play();
    } // DeadSound
    private void MoveSound()
    {
        mController.monsterAudio.clip = moveClip;
        mController.monsterAudio.Play();
    } // MoveSound
    private void HitSound()
    {
        mController.monsterAudio.clip = hitClip;
        mController.monsterAudio.Play();
    } // HitSound
    private void WeaponSound()
    {
        mController.monsterAudio.clip = weaponClip;
        mController.monsterAudio.Play();
    } // WeaponSound
    #endregion // ���� ����
    //! } �ذ񺴻� �׸� region ����
} // SkeletonSoldier
