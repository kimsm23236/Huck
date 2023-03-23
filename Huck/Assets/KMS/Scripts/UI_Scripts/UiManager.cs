using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static bool isResumeOn = false;

    // { Game Menu Btn
    public void OnResume_Btn()
    {
        isResumeOn = true;
        Debug.Log("1버튼 눌림");
    }

    public void OnSetting_Btn()
    {
        Debug.Log("2버튼 눌림");
    }

    public void OnExitGame_Btn()
    {
        GFunc.QuitThisGame();
        Debug.Log("3버튼 눌림");
    }
    // } Game Menu Btn
}
