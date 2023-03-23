using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterMove : IMonsterState
{
    private MonsterController mController;
    //private Vector3 dir; // 이동할 방향 변수
    private bool exitState; // 코루틴 while문 조건
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.MOVE;
        Debug.Log($"무브상태 시작 : {mController.monster.monsterName}");
        exitState = false;
        mController.CoroutineDeligate(Move());
    } // StateEnter
    public void StateFixedUpdate()
    {
        /*Do Nothing*/
    } // StateFixedUpdate
    public void StateUpdate()
    {
        Move();
        /*Do Nothing*/
    } // StateUpdate
    public void StateExit()
    {
        mController.monsterAni.SetBool("isRun", false);
        mController.mAgent.ResetPath();
        exitState = true;
    } // StateExit

    //! 몬스터 이동 코루틴함수
    private IEnumerator Move()
    {
        mController.monsterAni.SetBool("isRun", true);
        while (exitState == false)
        {
            if (exitState == true)
            {
                yield break;
            }
            mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            yield return null;
        }
    } // Move
} // MonsterMove
