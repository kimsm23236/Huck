using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSearch : IMonsterState
{
    private MonsterController mController;
    private bool exitState; // �ڷ�ƾ while�� ����
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.SEARCH;
        Debug.Log($"�������� ���� : {mController.monster.monsterName}");
        mController.CoroutineDeligate(TargetChase());
    } // StateEnter
    public void StateFixedUpdate()
    {
        /*Do Nothing*/
    } // StateFixedUpdate
    public void StateUpdate()
    {
        /*Do Nothing*/
    } // StateUpdate
    public void StateExit()
    {
        exitState = true;
        mController.mAgent.ResetPath();
        mController.mAgent.speed = mController.monster.moveSpeed;
        mController.monsterAni.SetBool("isWalk", false);
    } // StateExit

    //! Ÿ���� �����ϴ� �ڷ�ƾ�Լ�
    private IEnumerator TargetChase()
    {
        mController.mAgent.speed = mController.monster.moveSpeed * 0.7f;
        exitState = false;
        mController.monsterAni.SetBool("isWalk", true);
        while (exitState == false)
        {
            mController.mAgent.SetDestination(mController.target.transform.position);
            yield return null;
        }
    } // TargetChase
} // MonsterSearch
