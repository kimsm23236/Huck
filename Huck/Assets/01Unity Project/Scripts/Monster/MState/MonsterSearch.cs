using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSearch : IMonsterState
{
    private MonsterController mController;
    private NavMeshAgent mAgent;
    private GameObject target;
    private bool exitState;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.SEARCH;
        Debug.Log($"추적상태 시작 : {mController.enumState}");
        mAgent = mController.gameObject.GetComponent<NavMeshAgent>();
        exitState = false;
        TargetChase();
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
        mAgent.enabled = false;
        mController.monsterAni.SetBool("isRun", false);
        /*Do Nothing*/
    }

    private IEnumerator TargetChase()
    {
        mAgent.enabled = true;
        target = mController.targetSearch.hit.gameObject;
        Debug.Log($"타겟 : {target}");
        mController.monsterAni.SetBool("isRun", true);
        while (exitState == false)
        {
            mAgent.SetDestination(target.transform.position);
            float distance = Vector3.Distance(mController.transform.position, target.transform.position);
            if (distance <= 0.1f)
            {
                mAgent.enabled = false;
                mController.monsterAni.SetBool("isRun", false);
                exitState = true;
                yield break;
            }
            yield return null;
        }
    }
}
