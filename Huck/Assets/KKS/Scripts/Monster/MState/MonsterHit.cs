using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHit : IMonsterState
{
    private MonsterController mController;
    private string aniClipName = default;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.HIT;
        //Debug.Log($"Hit���� ���� : {mController.monster.monsterName}");
        aniClipName = GetHitPoint();
        mController.CoroutineDeligate(HitProcess(aniClipName));
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
        mController.monsterAni.SetBool(aniClipName, false);
        mController.isHit = false;
        aniClipName = default;
    } // StateExit

    //! ������ ������ �� ���� ��ġ�� ã�� �Լ�
    private string GetHitPoint()
    {
        // Ÿ�ٰ��� ���⺤�͸� ���� ������ forward �� ���⺤�� ������ ������ ����
        Vector3 dir = (mController.attacker.transform.position - mController.transform.position).normalized;
        float angle = Mathf.Acos(Vector3.Dot(mController.transform.forward, dir)) * Mathf.Rad2Deg;
        // 180 ~ 360 ���� ǥ���ϱ����� ó��
        if (Vector3.Cross(mController.transform.forward, dir).y < 0)
        {
            angle = 360 - angle;
        }
        //Debug.Log($"Ÿ�ٰ��� ���� : {angle}");
        string clipName = default;
        // ������ forward ���� 90�� �� 4�������� ���� Ÿ�ݵ� ������ ã��
        if (angle >= 45 && angle < 135)
        {
            //Debug.Log($"Ÿ���� �����ʿ� �ִ�!!");
            clipName = "isHitRight";
        }
        else if (angle >= 135 && angle < 225)
        {
            //Debug.Log($"Ÿ���� �ڿ� �ִ�!!");
            clipName = "isHitBack";
        }
        else if (angle >= 225 && angle < 315)
        {
            //Debug.Log($"Ÿ���� ���ʿ� �ִ�!!");
            clipName = "isHitLeft";
        }
        else
        {
            //Debug.Log($"Ÿ���� ���濡 �ִ�!!");
            clipName = "isHitFront";
        }
        return clipName;
    } // GetHitPoint

    //! ���ݴ��� ���⿡ ���� HIT��� �����ϴ� �Լ�
    private IEnumerator HitProcess(string _clipName)
    {
        mController.monsterAni.SetBool(_clipName, true);
        yield return null;
        //Debug.Log(_clipName);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        // Hit ����� ������ Idle���·� �ʱ�ȭ
        mController.monsterAni.SetBool(_clipName, false);
        IMonsterState nextState = new MonsterIdle();
        mController.MStateMachine.onChangeState?.Invoke(nextState);
    } // HitProcess
} // MonsterHit
