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

    //! 공격 데미지 처리 함수
    private void OnDamage()
    {

    } // OnDamage

    //! 원거리 공격 함수
    private void LongDistanceAttack()
    {
        StartCoroutine(AttackC());
    } // LongDistanceAttack

    //! 공격이 끝났을 때 실행하는 이벤트함수
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isAttackC_End", false);
        StartCoroutine(AttackDelay());
    } // ExitAttack

    //! 공격딜레이 주는 코루틴함수
    private IEnumerator AttackDelay()
    {
        int number = Random.Range(0, 3);
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
                    if (checkTime >= 2f)
                    {
                        isBackMove = true;
                    }
                    Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
                    mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
                    mController.transform.position += -dir * moveSpeed * Time.deltaTime;

                    yield return null;
                }
                Debug.Log($"백무빙 종료");
                mController.monsterAni.SetBool("isBack", false);
                break;
            case 1:
                float checkTime2 = 0f;
                bool isIdle = false;
                Debug.Log($"대기 시작");
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
                Debug.Log($"대기 종료");
                mController.monsterAni.SetBool("isIdle", false);

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
                Debug.Log($"사이드무빙 종료");
                break;
        }
        IMonsterState nextState = new MonsterIdle();
        Debug.Log($"상태 변경 : {nextState}");
        mController.MStateMachine.onChangeState?.Invoke(nextState);
    } // AttackDelay

    //! AttackC 돌진 공격 코루틴함수
    private IEnumerator AttackC()
    {
        Debug.Log("AttackC 코루틴 시작");
        // 돌진 준비 모션 끝나면 돌진 시작
        mController.monsterAni.SetBool("isAttackC_Start", false);
        mController.monsterAni.SetBool("isAttackC_Loop", true);
        bool isAttackC = false;
        while (isAttackC == false)
        {
            Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            mController.transform.position += dir * (moveSpeed * 2f) * Time.deltaTime;
            // 돌진 중 타겟이 근접공격사거리 안이라면 돌진 마무리 시작
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
