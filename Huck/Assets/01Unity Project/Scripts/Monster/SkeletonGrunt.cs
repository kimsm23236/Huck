using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Monster;

public class SkeletonGrunt : Monster
{
    private MonsterController mController = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    private float rushCool = 0f;
    public MonsterData monsterData;
    public GameObject weapon;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! �ذ�׷�Ʈ ���� �������̵�
    public override void Attack()
    {
        if (mController.distance > meleeAttackRange)
        {
            StartCoroutine(RushAttack());
        }
        else
        {
            int number = Random.Range(0, 10);
            if (number <= 4)
            {
                mController.monsterAni.SetBool("isAttackA", true);
            }
            else if (number > 4 && number <= 7)
            {
                mController.monsterAni.SetBool("isAttackB", true);
            }
            else
            {
                mController.monsterAni.SetBool("isAttackC", true);
            }
        }
    } // Attack

    //! �ذ�׷�Ʈ ��ų �������̵�
    public override void Skill()
    {
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
        RaycastHit[] hits = Physics.SphereCastAll(weapon.transform.position, 3f, Vector3.up, 0f, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            if (hits[0].collider.tag == "Player")
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
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isRushAttack", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isSkillB_End", false);
        // �������� �� ������ ����
        StartCoroutine(AttackDelay(mController, 4));
    } // ExitAttack

    //! ���� ���� �ڷ�ƾ �Լ�
    private IEnumerator RushAttack()
    {
        StartCoroutine(RushCooldown());
        mController.monsterAni.SetBool("isRun", true);
        bool isRush = true;
        bool isFinishRush = false;
        float timeCheck = 0f;
        while (isRush == true)
        {
            // if : ������ ���� ���� ������
            if (isFinishRush == false)
            {
                Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
                mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
                mController.transform.position += dir * (moveSpeed * 2f) * Time.deltaTime;
            }
            else
            {
                // ������ ���� ���۵Ǹ� �����ϴ� ���� �״�� 1�ʰ� ����
                timeCheck += Time.deltaTime;
                mController.transform.position += transform.forward * (moveSpeed * 1.5f) * Time.deltaTime;
                if (timeCheck >= 1f)
                {
                    isRush = false;
                }
            }

            if (mController.distance <= meleeAttackRange && isFinishRush == false)
            {
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
        while (true)
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
