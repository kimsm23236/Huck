using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillA : MonoBehaviour
{
    private MonsterController mController;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.SkillA;

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
}
