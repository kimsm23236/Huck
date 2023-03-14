using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSearch : IMonsterState
{
    private MonsterController mController;
    private NavMeshAgent mAgent;
    private bool exitState;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.SEARCH;
        mAgent = mController.gameObject.GetComponent<NavMeshAgent>();
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
        mAgent.enabled = false;
        mController.monsterAni.SetBool("isRun", false);
    }

    private IEnumerator TargetChase()
    {
        exitState = false;
        mAgent.enabled = true;
        mController.monsterAni.SetBool("isRun", true);
        while (exitState == false)
        {
            mAgent.SetDestination(mController.targetPos.position);
            yield return null;
        }
    }
}
