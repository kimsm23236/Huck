using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerObj;
    public void PlayerObj()
    {
        Debug.Log($"���ӸŴ��� ���۵�? {playerObj.transform.position}");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerObj();
        }
    }
}
