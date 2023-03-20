using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterMove : IMonsterState
{
    private MonsterController mController;
    //private Vector3 dir; // �̵��� ���� ����
    private bool exitState; // �ڷ�ƾ while�� ����
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.MOVE;
        Debug.Log($"������� ���� : {mController.monster.monsterName}");
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

    //! ���� �̵� �ڷ�ƾ�Լ�
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
            //dir = (mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
            //mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            //mController.mAgent.Move(dir * mController.monster.moveSpeed * Time.deltaTime);
            yield return null;
        }
    } // Move
} // MonsterMove
