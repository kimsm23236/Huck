using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerObj = default;
    //! { [김광성] 플레이어 오브젝트 시작시 캐싱
    private void Awake()
    {
        playerObj = GFunc.GetRootObj(GData.PLAYER_MASK);
    }
    // } [김광성] 플레이어 오브젝트 시작시 캐싱
}
