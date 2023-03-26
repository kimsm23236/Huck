using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdle : IMonsterState
{
    private MonsterController mController;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.IDLE;
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
        /*Do Nothing*/
    } // StateExit
} // MonsterIdle
