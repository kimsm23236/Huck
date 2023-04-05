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
        Debug.Log($"�������� ���� : {mController.monster.monsterName}");
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

    //! Ÿ���� �����ϴ� �ڷ�ƾ�Լ�
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
            // Ÿ����ġ���� �Ÿ��� 0.5f ���ϸ� ���ο� Ÿ����ǥ�� ����
            if (distance <= 0.5f)
            {
                time += Time.deltaTime;
                mController.mAgent.ResetPath();
                mController.monsterAni.SetBool("isWalk", false);
                // 3�ʰ� Idle ������� ���
                if (time > 3f)
                {
                    // 60% Ȯ���� ���ο� ������ǥ, 40% Ȯ���� �÷��̾� ��ǥ ����
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
                // ���� ���� ��ǥ�� �÷��̾� ��ǥ��� 4�ʰ� �̵�
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
