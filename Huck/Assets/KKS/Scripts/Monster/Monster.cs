using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //! 몬스터 타입
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
    [HideInInspector] public bool isNoRangeAttack;
    [HideInInspector] public bool isNoRangeSkill;
    [HideInInspector] public bool useSkillA;
    [HideInInspector] public bool useSkillB;
    [HideInInspector] public float skillA_MaxCool;
    [HideInInspector] public float skillB_MaxCool;
    [HideInInspector] public float searchRange;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float meleeAttackRange;

    //! 몬스터 데이터 초기화하는 함수
    public void InitMonsterData(MonsterType _monsterType, MonsterData monsterData)
    {
        this.monsterType = _monsterType;
        this.monsterName = monsterData.MonsterName;
        this.monsterHp = monsterData.MonsterHp;
        this.monsterMaxHp = monsterData.MonsterMaxHp;
        this.moveSpeed = monsterData.MoveSpeed;
        this.minDamage = monsterData.MinDamage;
        this.maxDamage = monsterData.MaxDamage;
        this.isNoRangeAttack = monsterData.IsNoRangeAttack;
        this.isNoRangeSkill = monsterData.IsNoRangeSkill;
        this.useSkillA = monsterData.UseSkillA;
        this.useSkillB = monsterData.UseSkillB;
        this.skillA_MaxCool = monsterData.SkillA_MaxCooldown;
        this.skillB_MaxCool = monsterData.SkillB_MaxCooldown;
        this.searchRange = monsterData.SearchRange;
        this.attackRange = monsterData.AttackRange;
        this.meleeAttackRange = monsterData.MeleeAttackRange;
    } // InitMonsterData

    //! 공격 함수
    public virtual void Attack()
    {
        /* Do Nothing */
    } // Attack

    //! 스킬공격 함수
    public virtual void Skill()
    {
        /* Do Nothing */
    } // Skill

    //! 공격딜레이 주는 코루틴함수
    protected IEnumerator AttackDelay(MonsterController mController, int _number)
    {
        int number = Random.Range(0, _number);
        switch (number)
        {
            case 0:
                float checkTime = 0f;
                bool isBackMove = false;
                Debug.Log($"백무빙 시작");
                mController.monsterAni.SetBool("isBack", true);
                while (isBackMove == false)
                {
                    checkTime += Time.deltaTime;
                    if (checkTime >= 1.5f)
                    {
                        isBackMove = true;
                    }
                    Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
                    mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
                    mController.mAgent.Move(-dir * moveSpeed * Time.deltaTime);
                    yield return null;
                }
                Debug.Log($"백무빙 종료");
                mController.monsterAni.SetBool("isBack", false);
                break;
            case 1:
                float checkTime2 = 0f;
                bool isIdle = false;
                Debug.Log($"대기 시작");
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
                Debug.Log($"대기 종료");
                break;
            case 2:
                float checkTime3 = 0f;
                bool isSideMove = false;
                int sideNumber = Random.Range(0, 2);
                Debug.Log("사이드무빙 시작");
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
                        mController.mAgent.Move(mController.transform.right.normalized * moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        mController.monsterAni.SetBool("isLeft", true);
                        mController.mAgent.Move(-mController.transform.right.normalized * moveSpeed * Time.deltaTime);
                    }
                    yield return null;
                }
                mController.monsterAni.SetBool("isRight", false);
                mController.monsterAni.SetBool("isLeft", false);
                Debug.Log($"사이드무빙 종료");
                break;
            case 3:
                mController.monsterAni.SetTrigger("isRoar");
                yield return new WaitForSeconds(0.1f);
                yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
                break;
        }
        // 공격딜레이가 끝났으면 Idle상태로 초기화
        IMonsterState nextState = new MonsterIdle();
        mController.MStateMachine.onChangeState?.Invoke(nextState);
    } // AttackDelay
} // Monster
