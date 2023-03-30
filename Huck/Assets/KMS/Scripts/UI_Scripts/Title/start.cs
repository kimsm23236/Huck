using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start : MonoBehaviour
{
    public Transform startBtn = default;

    public void OnStartBtn()
    {
        GFunc.LoadScene("[KMS]SampleTestScene");
        //GFunc.LoadScene("SampleTestScene");
    }

    public void OnEnterMouseThis_S()
    {
        startBtn.position += new Vector3(0, 0, 0.2f);
    }

    public void OnExitMouseThis_S()
    {
        startBtn.position += new Vector3(0, 0, -0.2f);
    }
}
