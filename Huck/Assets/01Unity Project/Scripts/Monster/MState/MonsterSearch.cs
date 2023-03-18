using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSearch : IMonsterState
{
    private MonsterController mController;
    private bool exitState;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.SEARCH;
        Debug.Log($"추적상태 시작 : {mController.monster.monsterName}");

        mController.CoroutineDeligate(TargetChase());
    }
    public void StateFixedUpdate()
    {
        /*Do Nothing*/
    }
    public void StateUpdate()
    {
        /*Do Nothing*/
    }
    public void StateExit()
    {
        exitState = true;
        mController.mAgent.ResetPath();
        mController.mAgent.speed = mController.monster.moveSpeed;
        mController.monsterAni.SetBool("isWalk", false);
    }

    private IEnumerator TargetChase()
    {
        mController.mAgent.speed = mController.monster.moveSpeed * 0.7f;

        exitState = false;
        mController.monsterAni.SetBool("isWalk", true);
        while (exitState == false)
        {
            mController.mAgent.SetDestination(mController.targetPos.position);
            yield return null;
        }
    }
}
