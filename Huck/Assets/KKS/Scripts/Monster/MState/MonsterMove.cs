using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : IMonsterState
{
    private MonsterController mController;
    private bool exitState; // �ڷ�ƾ while�� ����
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.MOVE;
        //Debug.Log($"������� ���� : {mController.monster.monsterName}");
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
        exitState = true;
        mController.mAgent.ResetPath();
        mController.monsterAni.SetBool("isRun", false);
    } // StateExit

    //! ���� �̵� �ڷ�ƾ�Լ�
    private IEnumerator Move()
    {
        mController.mAgent.speed = mController.monster.moveSpeed;
        mController.monsterAni.SetBool("isRun", true);
        exitState = false;
        while (exitState == false)
        {
            mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            yield return null;
        }
    } // Move
} // MonsterMove
