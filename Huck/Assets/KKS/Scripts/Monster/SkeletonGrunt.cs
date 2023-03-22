using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Monster;
using static UnityEngine.Rendering.DebugUI;

public class SkeletonGrunt : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private GameObject shoulder = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    private float rushCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! ���� ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! ��� ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableShoulderAttack()
    {
        shoulder.SetActive(true);
    } // EnableShoulderAttack

    //! �ذ�׷�Ʈ ���� �������̵�
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (mController.distance >= 13f)
        {
            StartCoroutine(RushAttack());
            return;
        }
        else if(mController.distance > meleeAttackRange)
        {
            // ���������� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ��������X Idle���·� ��ȯ
            StartCoroutine(CheckRushDistance());
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (mController.distance <= meleeAttackRange)
        {
            int number = Random.Range(0, 10);
            if (number > 7)
            {
                mController.monsterAni.SetBool("isAttackC", true);
                return;
            }
            else if (number > 4)
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

    //! �ذ�׷�Ʈ ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (useSkillA == true)
        {
            SkillA();
            Debug.Log($"��ųA �ߵ� {isNoRangeSkill}");
            return;
        }

        if (useSkillB == true)
        {
            SkillB();
            Debug.Log($"��ųB �ߵ� {isNoRangeSkill}");
            return;
        }
    } // Skill

    //! �ذ�׷�Ʈ ��ųA �Լ�
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! ��ųA ���������� �Լ�
    private void SkillA_Damage()
    {
        RaycastHit[] hits = Physics.SphereCastAll(weapon.transform.position, 3f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK));
        if (hits.Length > 0)
        {
            if (hits[0].collider.tag == GData.PLAYER_MASK)
            {
                Debug.Log("�÷��̾� ����");
            }
        }
    } // SkillA_Damage

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weapon.transform.position, 3f);
    }

    //! �ذ�׷�Ʈ ��ųB �Լ�
    private void SkillB()
    {
        mController.monsterAni.SetBool("isSkillB_Start", true);
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! �������� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        weapon.SetActive(false);
        shoulder.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isRushAttack", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isSkillB_End", false);
        // �������� �� ������ ����
        StartCoroutine(AttackDelay(mController, 4));
    } // ExitAttack

    //! ���� ���� ��� �Ÿ� üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckRushDistance()
    {
        isNoRangeAttack = true;
        while (isNoRangeAttack == true)
        {
            float distance = Vector3.Distance(mController.targetSearch.hit.transform.position, mController.transform.position);
            // Ÿ���� ���� �ּһ�Ÿ� �ۿ� ������ ���� ��밡��
            if (distance >= 13f)
            {
                Debug.Log($"�Ÿ� : {distance}");
                isNoRangeAttack = false;
                yield break;
            }
            yield return null;
        }
    } // CheckRushDistance

    //! ���� ���� �ڷ�ƾ �Լ�
    private IEnumerator RushAttack()
    {
        StartCoroutine(RushCooldown());
        mController.monsterAni.SetBool("isRun", true);
        bool isRush = true;
        bool isFinishRush = false;
        float timeCheck = 0f;
        mController.mAgent.speed = moveSpeed * 2f;
        while (isRush == true)
        {
            // if : ���� ������ ���� ���� ������
            if (isFinishRush == false)
            {
                mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            }
            else
            {
                // ���� ������ ���� ���۵Ǹ� �����ϴ� ���� �״�� 1�ʰ� ����
                timeCheck += Time.deltaTime;
                mController.mAgent.Move(mController.transform.forward * moveSpeed * Time.deltaTime);
                if (timeCheck >= 1f)
                {
                    isRush = false;
                }
            }
            // ���� ������ ���� ����
            if (mController.distance <= meleeAttackRange && isFinishRush == false)
            {
                mController.mAgent.speed = moveSpeed;
                mController.mAgent.ResetPath();
                mController.monsterAni.SetBool("isRun", false);
                mController.monsterAni.SetBool("isRushAttack", true);
                isFinishRush = true;
            }
            yield return null;
        }
    } // RushAttack

    //! ��ųA ���� ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillA()
    {
        mController.monsterAni.SetBool("isSkillB_Start", false);
        mController.monsterAni.SetBool("isSkillB_Loop", true);
        yield return new WaitForSeconds(2f);
        mController.monsterAni.SetBool("isSkillB_Loop", false);
        mController.monsterAni.SetBool("isSkillB_End", true);
    } // UseSkillA

    //! ���� ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator RushCooldown()
    {
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų �� ����
        isNoRangeAttack = true;
        while (isNoRangeAttack == true)
        {
            rushCool += Time.deltaTime;
            if (rushCool >= 20f)
            {
                rushCool = 0f;
                isNoRangeAttack = false;
                yield break;
            }
            yield return null;
        }
    } // RushCooldown

    //! ��ųA ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillACooldown()
    {
        useSkillA = false;
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų �� ����
        while (true)
        {
            skillACool += Time.deltaTime;
            if (skillACool >= skillA_MaxCool)
            {
                skillACool = 0f;
                useSkillA = true;
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
} // SkeletonGrunt
