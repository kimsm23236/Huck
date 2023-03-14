using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : IMonsterState
{
    private MonsterController mController;
    private Vector3 dir; // �̵��� ���� ����
    private bool exitState; // �ڷ�ƾ while�� ����
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.MOVE;
        Debug.Log($"������� ���� : {mController.monster.monsterName}");
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
            // Move���� ���� �� while Ż��
            if (exitState == true)
            {
                mController.monsterAni.SetBool("isWalk", false);
                yield break;
            }

            dir = (movePos - mController.transform.position).normalized;
            Debug.Log($"��ǥ��: {movePos}, ����: {dir}");
            // dir �������� �ٶ󺸰� ��
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            mController.transform.position += dir * mController.monster.moveSpeed * Time.deltaTime;

            float distance = Vector3.Distance(mController.transform.position, movePos);
            // ��ǥ������ �����ϸ� ��� �� ���ο� ��ǥ�� �̵�
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
