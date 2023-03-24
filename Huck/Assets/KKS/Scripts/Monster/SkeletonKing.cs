using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKing : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
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

    //! �������� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        weapon.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isAttackD", false);
        mController.monsterAni.SetBool("isAttackE", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        mController.monsterAni.SetBool("isSkillB", false);
        // �������� �� ������ ���·� ��ȯ
        mController.isDelay = true;
    } // ExitAttack

    //! �ذ�� ���� �������̵�
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        // ��� 5�� �� �������� �Ѱ� ����
        if (mController.distance <= meleeAttackRange)
        {
            int number = Random.Range(0, 11);
            if (number > 8)
            {
                mController.monsterAni.SetBool("isAttackE", true);
            }
            else if (number > 6)
            {
                mController.monsterAni.SetBool("isAttackD", true);
            }
            else if (number > 4)
            {
                mController.monsterAni.SetBool("isAttackC", true);
                return;
            }
            else if (number > 2)
            {
                mController.monsterAni.SetBool("isAttackB", true);
                return;
            }
            else
            {
                mController.monsterAni.SetBool("isAttackA", true);
            }
        }
    } // Attack

    //! �ذ�� ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (useSkillA == true)
        {
            useSkillA = false;
            SkillA();
            CheckUseSkill();
            return;
        }
        if (useSkillB == true && mController.distance >= 13f)
        {
            useSkillB = false;
            SkillB();
            CheckUseSkill();
            return;
        }
        else if (useSkillB == true && mController.distance < 13f)
        {
            useSkillB = false;
            // ������ų�� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ������ų ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckSkillBDistance());
            CheckUseSkill();
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }
    } // Skill

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

    //! ��ųA ���� ��� �Ÿ�üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckSkillBDistance()
    {
        isNoRangeSkill = true;
        while (isNoRangeSkill == true)
        {
            // Ÿ���� ���� �ּһ�Ÿ� �ۿ� ������ ���� ��밡��
            if (mController.distance >= 13f)
            {
                useSkillB = true;
                isNoRangeSkill = false;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // CheckSkillBDistance

    //! �ذ�� ��ųA �Լ� (��ȯ ��ų)
    private void SkillA()
    {
        StartCoroutine(UseSkillA());
    } // SkillA

    //! ��ųA ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillA()
    {
        StartCoroutine(SkillACooldown());
        mController.monsterAni.SetBool("isSkillA_Start", true);
        bool isStart = true;
        bool isSkillA = false;
        float timeCheck = 0f;
        while (isSkillA == false)
        {
            timeCheck += Time.deltaTime;
            if (mController.monsterAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && isStart == true)
            {
                // ��ȯ �غ� ��� ������ ��ȯ ����
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", true);
                isStart = false;
            }
            // 5�ʰ� ������ ��ȯ ������ ����
            if (isStart == false && timeCheck >= 5f)
            {
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = true;
            }
            yield return null;
        }
    } // UseSkillA

    //! �ذ�� ��ųB �Լ� (���� ����)
    private void SkillB()
    {
        StartCoroutine(UseSkillB());
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! ��ųB (���� ����) �ڷ�ƾ�Լ�
    private IEnumerator UseSkillB()
    {
        // ������ �̵��Լ��� ����ϱ� ���� Parabola �ʱ�ȭ
        Parabola parabola = new Parabola();
        // ���Ͱ� Ÿ���� �ٶ󺸴� ������ �ݴ������ ����
        Vector3 dir = -(mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
        // ��ǥ��ġ�� dir�������� meleeAttackRange��ŭ �̵��� ��ǥ�� ����
        Vector3 targetPos = mController.targetSearch.hit.transform.position + dir * meleeAttackRange;
        mController.monsterAni.SetBool("isSkillB", true);
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(parabola.ParabolaMoveToTarget(mController.transform.position, targetPos, 1f, gameObject));
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length - 0.8f);
        mController.monsterAni.SetBool("isSkillB", false);
        mController.monsterAni.SetTrigger("isRoar");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        mController.isDelay = true;
    } // UseSkillB



    //! ��ųA ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillACooldown()
    {
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų �� ����
        while (true)
        {
            skillACool += Time.deltaTime;
            if (skillACool >= skillA_MaxCool)
            {
                skillACool = 0f;
                useSkillA = true;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // SkillACooldown

    //! ��ųB ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillBCooldown()
    {
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų �� ����
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
} // SkeletonKing
