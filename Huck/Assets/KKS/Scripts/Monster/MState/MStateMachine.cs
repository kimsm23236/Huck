using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MStateMachine : MonoBehaviour
{
    private MonsterController mController;
    public Action<IMonsterState> onChangeState;

    public IMonsterState currentState
    {
        get;
        private set;
    } // IMonsterState

    public MStateMachine(IMonsterState defaultState, MonsterController _mController)
    {
        onChangeState += SetState;
        currentState = defaultState;
        mController = _mController;
        SetState(currentState);
    } // MStateMachine 생성자

    //! 몬스터컨트롤러에서 입력 받은 상태를 처리하는 함수
    public void SetState(IMonsterState state)
    {
        if (currentState == state)
        {
            return;
        } // if : 입력받은 상태가 현재상태와 같으면 return

        currentState.StateExit();
        currentState = state;
        currentState.StateEnter(mController);
    } // SetState

    //! 각 상태의 FixedUpdate를 대신 실행하는 함수
    public void DoFixedUpdate()
    {
        currentState.StateFixedUpdate();
    } // DoFixedUpdate

    //! 각 상태의 Update를 대신 실행하는 함수
    public void DoUpdate()
    {
        currentState.StateUpdate();
    } // DoUpdate
}
