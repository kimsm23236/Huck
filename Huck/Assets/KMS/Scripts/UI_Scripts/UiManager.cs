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
    }

    public void OnSetting_Btn()
    {
        GameObject settingMenu = GetComponent<GameObject>();
        settingMenu = GameObject.Find("SettingMenu");
        settingMenu.SetActive(true);
    }

    public void OnExitGame_Btn()
    {
        GFunc.QuitThisGame();
    }
    // } Game Menu Btn
}
