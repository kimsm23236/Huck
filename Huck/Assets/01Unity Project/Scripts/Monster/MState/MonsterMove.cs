using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : IMonsterState
{
    private MonsterController mController;
    private Vector3 dir; // 이동할 방향 변수
    private bool exitState; // 코루틴 while문 조건
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.MOVE;
        Debug.Log($"무브상태 시작 : {mController.monster.monsterName}");
        exitState = false;
        mController.CoroutineDeligate(Move());
        //mController.CoroutineDeligate(RandomMovePos());
    }
    public void StateFixedUpdate()
    {
        /*Do Nothing*/
    }
    public void StateUpdate()
    {
        Move();
        /*Do Nothing*/
    }
    public void StateExit()
    {
        mController.monsterAni.SetBool("isWalk", false);
        exitState = true;
    }

    private IEnumerator Move()
    {
        mController.monsterAni.SetBool("isWalk", true);
        while (exitState == false)
        {
            if (exitState == true)
            {
                yield break;
            }
            dir = (mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            mController.transform.position += dir * mController.monster.moveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RandomMovePos()
    {
        Vector3 movePos = new Vector3();
        movePos.x = Random.Range(-5, 5);
        movePos.z = Random.Range(-5, 5);
        mController.monsterAni.SetBool("isWalk", true);
        while (exitState == false)
        {
            // Move상태 나갈 때 while 탈출
            if (exitState == true)
            {
                mController.monsterAni.SetBool("isWalk", false);
                yield break;
            }

            dir = (movePos - mController.transform.position).normalized;
            Debug.Log($"좌표값: {movePos}, 방향: {dir}");
            // dir 뱡향으로 바라보게 함
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            mController.transform.position += dir * mController.monster.moveSpeed * Time.deltaTime;

            float distance = Vector3.Distance(mController.transform.position, movePos);
            // 목표지점에 근접하면 대기 후 새로운 좌표로 이동
            if (distance <= 0.1f)
            {
                mController.monsterAni.SetBool("isWalk", false);
                yield return new WaitForSeconds(Random.Range(1f, 4f));
                mController.monsterAni.SetBool("isWalk", true);
                movePos.x = Random.Range(-5, 5);
                movePos.z = Random.Range(-5, 5);
            }
            yield return null;
        }
    } // RandomMovePos
}
