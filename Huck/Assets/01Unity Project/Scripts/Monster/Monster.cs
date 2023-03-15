using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //! ���� Ÿ��
    public enum MonsterType
    {
        MELEE = 0,
        RANGE
    } // MonsterType

    [HideInInspector] public MonsterType monsterType;
    [HideInInspector] public string monsterName;
    [HideInInspector] public float monsterHp;
    [HideInInspector] public float monsterMaxHp;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float minDamage;
    [HideInInspector] public float maxDamage;
    [HideInInspector] public bool skillA;
    [HideInInspector] public bool skillB;
    [HideInInspector] public float skillA_MaxCool;
    [HideInInspector] public float skillB_MaxCool;
    [HideInInspector] public float searchRange;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float meleeAttackRange;

    //! ���� ������ �ʱ�ȭ�ϴ� �Լ�
    public void InitMonsterData(MonsterType _monsterType, MonsterData monsterData)
    {
        this.monsterType = _monsterType;
        this.monsterName = monsterData.MonsterName;
        this.monsterHp = monsterData.MonsterHp;
        this.monsterMaxHp = monsterData.MonsterMaxHp;
        this.moveSpeed = monsterData.MoveSpeed;
        this.minDamage = monsterData.MinDamage;
        this.maxDamage = monsterData.MaxDamage;
        this.skillA = monsterData.SkillA;
        this.skillB = monsterData.SkillB;
        this.skillA_MaxCool = monsterData.SkillA_MaxCooldown;
        this.skillB_MaxCool = monsterData.SkillB_MaxCooldown;
        this.searchRange = monsterData.SearchRange;
        this.attackRange = monsterData.AttackRange;
        this.meleeAttackRange = monsterData.MeleeAttackRange;
    } // InitMonsterData

    //! ���ݵ����� �ִ� �ڷ�ƾ�Լ�
    protected IEnumerator AttackDelay(MonsterController mController)
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
}