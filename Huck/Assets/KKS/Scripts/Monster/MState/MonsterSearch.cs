using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSearch : IMonsterState
{
    private MonsterController mController;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.SEARCH;
        Debug.Log($"추적상태 시작 : {mController.monster.monsterName}");
        mController.CoroutineDeligate(TargetChase());
    } // StateEnter
    public void StateFixedUpdate()
    {
        /*Do Nothing*/
    } // StateFixedUpdate
    public void StateUpdate()
    {
        mController.targetSearch.SearchTarget();
    } // StateUpdate
    public void StateExit()
    {
        mController.mAgent.ResetPath();
        mController.monsterAni.SetBool("isWalk", false);
        mController.mAgent.speed = mController.monster.moveSpeed;
    } // StateExit

    //! 타겟을 추적하는 코루틴함수
    private IEnumerator TargetChase()
    {
        mController.mAgent.speed = mController.monster.moveSpeed * 0.7f;
        mController.monsterAni.SetBool("isWalk", true);
        GetRandomPosition getRandomPosition = new GetRandomPosition();
        Vector3 targetPos = getRandomPosition.GetRandomCirclePos(mController.transform.position, 20, 10);
        float time = 0f;
        bool isTargetToPlayer = false;
        mController.targetSearch.hit = null;
        while (mController.targetSearch.hit == null)
        {
            float distance = Vector3.Distance(targetPos, mController.transform.position);
            // 타겟위치와의 거리가 0.5f 이하면 새로운 타겟좌표를 정함
            if (distance <= 0.5f)
            {
                time += Time.deltaTime;
                mController.mAgent.ResetPath();
                mController.monsterAni.SetBool("isWalk", false);
                // 3초간 Idle 모션으로 대기
                if (time > 3f)
                {
                    // 60% 확률로 새로운 랜덤좌표, 40% 확률로 플레이어 좌표 선택
                    int number = Random.Range(0, 10);
                    if (number < 5)
                    {
                        isTargetToPlayer = false;
                        targetPos = getRandomPosition.GetRandomCirclePos(mController.transform.position, 20, 10);
                    }
                    else
                    {
                        isTargetToPlayer = true;
                        targetPos = mController.target.transform.position;
                    }
                    time = 0f;
                    mController.monsterAni.SetBool("isWalk", true);
                }
            }
            else
            {
                mController.mAgent.SetDestination(targetPos);
                // 새로 정한 좌표가 플레이어 좌표라면 4초간 이동
                if (isTargetToPlayer == true)
                {
                    yield return new WaitForSeconds(4f);
                    isTargetToPlayer = false;
                    targetPos = mController.transform.position;
                }
            }
            yield return null;
        }
    } // TargetChase
} // MonsterSearch
