using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerSpawn_P = default;
    public GameObject playerObj = default;
    //! { [�豤��] �÷��̾� ������Ʈ ���۽� ĳ��
    private void Awake()
    {
        playerSpawn_P.SetActive(true);
        playerObj = GFunc.GetRootObj(GData.PLAYER_MASK);
    }
    // } [�豤��] �÷��̾� ������Ʈ ���۽� ĳ��
}
