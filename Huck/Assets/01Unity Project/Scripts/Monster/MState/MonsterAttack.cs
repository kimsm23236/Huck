using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : IMonsterState
{
    private MonsterController mController;
    private float longRangeCool = 0f;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.ATTACK;
        //Debug.Log($"공격상태 시작 : {mController.monster.monsterName}");

        if (mController.monster.monsterType == Monster.MonsterType.MELEE)
        {
            SelectAttack_Melee();
        }
        else if (mController.monster.monsterType == Monster.MonsterType.RANGE)
        {

        }
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

    }

    //! 타겟과의 거리에 따라 공격방식을 선택하는 함수
    private void SelectAttack_Melee()
    {
        if (mController.distance > mController.monster.meleeAttackRange && longRangeCool == 0f)
        {
            mController.monsterAni.SetBool("isAttackC_Start", true);
            mController.CoroutineDeligate(LongRangeCooldown());
        }

        if (mController.distance <= mController.monster.meleeAttackRange)
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
    } // SelectAttack

    private IEnumerator LongRangeCooldown()
    {
        mController.isAttack = true;
        while (true)
        {
            longRangeCool += Time.deltaTime;
            if (longRangeCool >= 15f)
            {
                longRangeCool = 0f;
                mController.isAttack = false;
                yield break;
            }
            yield return null;
        }
    }
}
