using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : IMonsterState
{
    private MonsterController mController;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.ATTACK;
        Debug.Log($"공격상태 시작 : {mController.monster.monsterName}");
        SelectAttack();
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
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC_End", false);
        mController.isAttack = false;
    }

    private void SelectAttack()
    {
        if (mController.distance >= mController.monster.meleeAttackRange)
        {
            mController.monsterAni.SetBool("isAttackC_Start", true);
        }
        else
        {
            int number = Random.Range(0, 10);
            if (number <= 6)
            {
                mController.monsterAni.SetBool("isAttackA", true);
            }
            else
            {
                mController.monsterAni.SetBool("isAttackB", true);
            }
        }
        mController.isAttack = true;
    }
}
