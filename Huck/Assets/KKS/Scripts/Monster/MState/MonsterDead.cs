using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDead : IMonsterState
{
    private MonsterController mController;
    private CapsuleCollider monsterCollider = default;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.DEAD;
        //Debug.Log($"죽음상태 시작 : {mController.monster.monsterName}");
        monsterCollider = mController.gameObject.GetComponent<CapsuleCollider>();
        mController.isDelay = false;
        // 몬스터시체 충돌을 막기위한 트리거 true
        monsterCollider.isTrigger = true;
        // 몬스터의 타입별 죽음처리
        switch (mController.monster.monsterType)
        {
            case Monster.MonsterType.BOSS:
                // 보스몬스터는 죽음 로직이 달라서 따로 처리
                mController.monster.BossDead();
                break;
            case Monster.MonsterType.NAMEED:
                mController.CoroutineDeligate(Dead());
                break;
            case Monster.MonsterType.NOMAL:
                mController.CoroutineDeligate(Dead());
                break;
        }
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
        monsterCollider.isTrigger = false;
    } // StateExit

    //! 몬스터 죽음 처리 함수
    private IEnumerator Dead()
    {
        mController.monsterAni.SetBool("isDead", true);
        yield return null;
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(1.5f);
        // 밑으로 시체가 내려가게 하기위해 네비매쉬 비활성화
        mController.mAgent.enabled = false;
        // 4초에 걸쳐 총 2f만큼 밑으로 내려간 뒤에 디스트로이
        float deadTime = 0f;
        while (deadTime < 4f)
        {
            deadTime += Time.deltaTime;
            float deadSpeed = Time.deltaTime * 0.5f;
            mController.transform.position += Vector3.down * deadSpeed;
            yield return null;
        }
        mController.DestroyObj(mController.gameObject);
    } // Dead
} // MonsterDead
