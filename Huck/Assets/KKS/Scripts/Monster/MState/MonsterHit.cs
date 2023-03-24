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
        Debug.Log($"Hit상태 시작 : {mController.monster.monsterName}");
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
        mController.isHit = false;
        aniClipName = default;
    } // StateExit

    //! 공격을 당했을 때 맞은 위치를 찾는 함수
    private string GetHitPoint()
    {
        // 타겟과의 방향벡터를 구한 다음에 forward 와 방향벡터 사이의 각도를 구함
        Vector3 dir = (mController.attacker.transform.position - mController.transform.position).normalized;
        float angle = Mathf.Acos(Vector3.Dot(mController.transform.forward, dir)) * Mathf.Rad2Deg;
        // 180 ~ 360 도를 표현하기위한 처리
        if (Vector3.Cross(mController.transform.forward, dir).y < 0)
        {
            angle = 360 - angle;
        }
        //Debug.Log($"타겟과의 각도 : {angle}");
        string clipName = default;
        // 몬스터의 forward 기준 90도 씩 4구역으로 나눠 타격된 방향을 찾음
        if (angle >= 45 && angle < 135)
        {
            //Debug.Log($"타겟이 오른쪽에 있다!!");
            clipName = "isHitRight";
        }
        else if (angle >= 135 && angle < 225)
        {
            //Debug.Log($"타겟이 뒤에 있다!!");
            clipName = "isHitBack";
        }
        else if (angle >= 225 && angle < 315)
        {
            //Debug.Log($"타겟이 왼쪽에 있다!!");
            clipName = "isHitLeft";
        }
        else
        {
            //Debug.Log($"타겟이 전방에 있다!!");
            clipName = "isHitFront";
        }
        return clipName;
    } // GetHitPoint

    //! 공격당한 방향에 따라 HIT모션 실행하는 함수
    private IEnumerator HitProcess(string _clipName)
    {
        mController.monsterAni.SetBool(_clipName, true);
        yield return null;
        Debug.Log(_clipName);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        // Hit 모션이 끝나면 Idle상태로 초기화
        mController.monsterAni.SetBool(_clipName, false);
        IMonsterState nextState = new MonsterIdle();
        mController.MStateMachine.onChangeState?.Invoke(nextState);
    } // HitProcess
} // MonsterHit
