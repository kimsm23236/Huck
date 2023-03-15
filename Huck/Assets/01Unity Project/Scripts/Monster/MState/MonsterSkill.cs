using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkill : MonoBehaviour
{
    private MonsterController mController;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.Skill;

        if (mController.distance > mController.monster.meleeAttackRange)
        {
            SelectSkill_Range();
        }
        else
        {
            SelectSkill_Melee();
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
        /*Do Nothing*/
    }

    private void SelectSkill_Melee()
    {
        mController.monsterAni.SetBool("isMeleeSkillA", true);
    }

    private void SelectSkill_Range()
    {

    }
}
