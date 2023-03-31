using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkill : IMonsterState
{
    private MonsterController mController;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.SKILL;
        //Debug.Log($"스킬상태 시작 : {mController.monster.monsterName}");
        mController.monster.Skill();
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
        mController.monster.ExitAttack();
    }
} // MonsterSkill
