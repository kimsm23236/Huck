using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exit : MonoBehaviour
{
    public Transform ExitBtn = default;

    public void OnEnterMouseThis_E()
    {
        ExitBtn.position += new Vector3(0, 0, 0.2f);
    }

    public void OnExitMouseThis_E()
    {
        ExitBtn.position += new Vector3(0, 0, -0.2f);
    }

    public void OnExitBtn()
    {
        GFunc.QuitThisGame();
    }
}
