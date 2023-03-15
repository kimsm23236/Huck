using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSoldier : Monster
{
    private MonsterController mController = default;
    public MonsterData monsterData;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! ���� ������ ó�� �Լ�
    private void OnDamage()
    {

    } // OnDamage

    //! ���Ÿ� ���� �Լ�
    private void LongDistanceAttack()
    {
        StartCoroutine(AttackC());
    } // LongDistanceAttack

    //! ������ ������ �� �����ϴ� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isAttackC_End", false);
        StartCoroutine(AttackDelay());
    } // ExitAttack

    //! ���ݵ����� �ִ� �ڷ�ƾ�Լ�
    private IEnumerator AttackDelay()
    {
        int number = Random.Range(0, 3);
        switch (number)
        {
            case 0:
                float checkTime = 0f;
                bool isBackMove = false;
                Debug.Log($"�鹫�� ����");
                mController.monsterAni.SetBool("isBack", true);
                while (isBackMove == false)
                {
                    checkTime += Time.deltaTime;
                    if (checkTime >= 2f)
                    {
                        isBackMove = true;
                    }
                    Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
                    mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
                    mController.transform.position += -dir * moveSpeed * Time.deltaTime;

                    yield return null;
                }
                Debug.Log($"�鹫�� ����");
                mController.monsterAni.SetBool("isBack", false);
                break;
            case 1:
                float checkTime2 = 0f;
                bool isIdle = false;
                Debug.Log($"��� ����");
                mController.monsterAni.SetBool("isIdle", true);
                while (isIdle == false)
                {
                    checkTime2 += Time.deltaTime;
                    if (checkTime2 >= 2f)
                    {
                        isIdle = true;
                    }
                    Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
                    mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);

                    yield return null;
                }
                Debug.Log($"��� ����");
                mController.monsterAni.SetBool("isIdle", false);

                break;
            case 2:
                float checkTime3 = 0f;
                bool isSideMove = false;
                int sideNumber = Random.Range(0, 2);
                Debug.Log("���̵幫�� ����");
                while (isSideMove == false)
                {
                    checkTime3 += Time.deltaTime;
                    if (checkTime3 >= 2f)
                    {
                        isSideMove = true;
                    }
                    Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
                    mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
                    if (sideNumber == 0)
                    {
                        mController.monsterAni.SetBool("isRight", true);
                        mController.transform.position += mController.transform.right.normalized * moveSpeed * Time.deltaTime;
                    }
                    else
                    {
                        mController.monsterAni.SetBool("isLeft", true);
                        mController.transform.position += -mController.transform.right.normalized * moveSpeed * Time.deltaTime;
                    }

                    yield return null;
                }
                mController.monsterAni.SetBool("isRight", false);
                mController.monsterAni.SetBool("isLeft", false);
                Debug.Log($"���̵幫�� ����");
                break;
        }
        IMonsterState nextState = new MonsterIdle();
        Debug.Log($"���� ���� : {nextState}");
        mController.MStateMachine.onChangeState?.Invoke(nextState);
    } // AttackDelay

    //! AttackC ���� ���� �ڷ�ƾ�Լ�
    private IEnumerator AttackC()
    {
        Debug.Log("AttackC �ڷ�ƾ ����");
        // ���� �غ� ��� ������ ���� ����
        mController.monsterAni.SetBool("isAttackC_Start", false);
        mController.monsterAni.SetBool("isAttackC_Loop", true);
        bool isAttackC = false;
        while (isAttackC == false)
        {
            Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            mController.transform.position += dir * (moveSpeed * 2f) * Time.deltaTime;
            // ���� �� Ÿ���� �������ݻ�Ÿ� ���̶�� ���� ������ ����
            if (mController.distance <= meleeAttackRange)
            {
                mController.monsterAni.SetBool("isAttackC_Loop", false);
                mController.monsterAni.SetBool("isAttackC_End", true);
                isAttackC = true;
            }
            yield return null;
        }
    } // AttackC
}
