using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterState
{
    void StateEnter(MonsterController mController);
    void StateFixedUpdate();
    void StateUpdate();
    void StateExit();
}
