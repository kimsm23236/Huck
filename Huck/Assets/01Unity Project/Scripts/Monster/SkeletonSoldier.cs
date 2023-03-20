using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSoldier : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private GameObject shield = default;
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
        RaycastHit[] hits = Physics.BoxCastAll(shield.transform.position, new Vector3(1f, 1f, 0.3f) * 0.5f, Vector3.up, shield.transform.rotation, 0f, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            if (hits[0].collider.tag == "Player")
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

        if (useSkillA == true && mController.distance >= 10f)
        {
            SkillA();
            return;
        }
        else if (useSkillA == true && mController.distance < 10f)
        {
            // ������ų�� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ������� ������ųX Idle���·� ��ȯ
            isNoRangeAttack = true;
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (useSkillB == true)
        {
            SkillB();
            return;
        }
    } // SKILL

    //! ��ųA �Լ�
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA_Start", true);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! ��ųA ���� ���� �̺�Ʈ�Լ�
    private void SkillA_Combo()
    {
        StartCoroutine(UseSkillA());
    } // SkillA_Combo

    //! ��ųB �Լ�
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
        StartCoroutine(AttackDelay(mController, 4));
    } // ExitAttack

    //! ��ųA ���� ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillA()
    {
        // ���� �غ� ��� ������ ���� ����
        mController.monsterAni.SetBool("isSkillA_Start", false);
        mController.monsterAni.SetBool("isSkillA_Loop", true);
        bool isSkillA = true;
        mController.mAgent.speed = moveSpeed * 2f;
        while (isSkillA == true)
        {
            mController.mAgent.SetDestination(mController.targetPos.position);
            // ���� �� Ÿ���� �������ݻ�Ÿ� ���̶�� ���� ������ ����
            if (mController.distance <= meleeAttackRange)
            {
                mController.mAgent.speed = moveSpeed;
                mController.mAgent.ResetPath();
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
        useSkillA = false;
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
                yield break;
            }
            yield return null;
        }
    } // SkillACooldown

    //! ��ųB ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillBCooldown()
    {
        useSkillB = false;
        while (true)
        {
            skillBCool += Time.deltaTime;
            if (skillBCool >= skillB_MaxCool)
            {
                skillBCool = 0f;
                useSkillB = true;
                yield break;
            }
            yield return null;
        }
    } // SkillBCooldown
} // SkeletonSoldier
