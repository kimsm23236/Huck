using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerObj = default;

    public GameObject procGenManager = default;

    //! { [�豤��] �÷��̾� ������Ʈ ���۽� ĳ��
    private void Awake()
    {
        //playerObj = GFunc.GetRootObj(GData.PLAYER_MASK);
        procGenManager = GFunc.GetRootObj("ProcGenManager");
    }
    // } [�豤��] �÷��̾� ������Ʈ ���۽� ĳ��
}
