using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.SpawnMonster();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.count = 4;
            GameManager.Instance.SpawnMonster();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameManager.Instance.BossSpwan();
        }
    }
}
