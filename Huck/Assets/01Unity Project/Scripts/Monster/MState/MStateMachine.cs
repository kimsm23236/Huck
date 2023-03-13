using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MStateMachine : MonoBehaviour
{
    MonsterController mController;
    public IMonsterState currentState
    {
        get;
        private set;
    } // IMonsterState

    public MStateMachine(IMonsterState defaultState, MonsterController _mController)
    {
        currentState = defaultState;
        mController = _mController;
        SetState(currentState);
    } // MStateMachine ������

    //! ������Ʈ�ѷ����� �Է� ���� ���¸� ó���ϴ� �Լ�
    public void SetState(IMonsterState state)
    {
        if (currentState == state)
        {
            return;
        } // if : �Է¹��� ���°� ������¿� ������ return

        currentState.StateExit();
        currentState = state;
        currentState.StateEnter(mController);
    } // SetState

    //! �� ������ FixedUpdate�� ��� �����ϴ� �Լ�
    public void DoFixedUpdate()
    {
        currentState.StateFixedUpdate();
    } // DoFixedUpdate

    //! �� ������ Update�� ��� �����ϴ� �Լ�
    public void DoUpdate()
    {
        currentState.StateUpdate();
    } // DoUpdate
}
