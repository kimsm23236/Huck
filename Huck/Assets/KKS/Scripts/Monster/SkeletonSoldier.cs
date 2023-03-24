using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float skillACool = 0f;
    private float skillBCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! ���� ó�� �̺�Ʈ�Լ� (RayCast)
    private void EnableAttack()
    {
        RaycastHit[] hits = Physics.BoxCastAll(shield.transform.position, new Vector3(1f, 1f, 0.3f) * 0.5f,
            Vector3.up, shield.transform.rotation, 0f, LayerMask.GetMask(GData.PLAYER_MASK));
        if (hits.Length > 0)
        {
            if (hits[0].collider.tag == GData.PLAYER_MASK)
            {
                Debug.Log("����转 ����!");
            }
        }
    } // EnableAttack

    //! EnableAttack() �����
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(shield.transform.position, new Vector3(1f, 1f, 0.3f));
    } // OnDrawGizmos

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
            SkillA();
            CheckUseSkill();
            return;
        }
        else if (useSkillA == true && mController.distance < 13f)
        {
            useSkillA = false;
            // ������ų�� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ������ų ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckSkillADistance());
            CheckUseSkill();
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (useSkillB == true)
        {
            useSkillB = false;
            SkillB();
            CheckUseSkill();
            return;
        }
    } // SKILL

    //! ��밡���� ��ų�� �ִ��� üũ�ϴ� �Լ� (������Ʈ�ѷ����� �������� üũ�ϱ� ����)
    private void CheckUseSkill()
    {
        if(useSkillA == false && useSkillB == false)
        {
            useSkill = false;
        }
        else
        {
            useSkill = true;
        }
    } // CheckUseSkill

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

    //! ��ųA �Լ� (���� ����)
    private void SkillA()
    {
        StartCoroutine(UseSkillA());
    } // SkillA

    //! ��ųB �Լ� (���� ����)
    private void SkillB()
    {
        mController.monsterAni.SetBool("isSkillB", true);
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! �������� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        mController.monsterAni.SetBool("isSkillB", false);
        weapon.SetActive(false);
        // �������� �� ������ ����
        mController.isDelay = true;
    } // ExitAttack

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
            if(mController.monsterAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && isStart == true)
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
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų ��밡��
        isNoRangeSkill = true;
        while (true)
        {
            skillACool += Time.deltaTime;
            if (skillACool >= skillA_MaxCool)
            {
                skillACool = 0f;
                useSkillA = true;
                isNoRangeSkill = false;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // SkillACooldown

    //! ��ųB ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillBCooldown()
    {
        while (true)
        {
            skillBCool += Time.deltaTime;
            if (skillBCool >= skillB_MaxCool)
            {
                skillBCool = 0f;
                useSkillB = true;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // SkillBCooldown
} // SkeletonSoldier
