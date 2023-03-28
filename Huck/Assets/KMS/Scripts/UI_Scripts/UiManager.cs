using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private GameObject UiObjs = default;
    private GameObject settingMenu = default;

    private GameObject Sen_goldot = default;
    private RectTransform goldotPos = default;

    private GameObject sha_CheckBox = default;
    private GameObject clo_CheckBox = default;
    private GameObject sunFind = default;
    private GameObject moonFind = default;
    private GameObject cloud = default;
    private Light theSun = default;
    private Light theMoon = default;

    public static bool isResumeOn = false;
    public static bool isSetMenuOpen = false;
    private bool is_C_CheckBox = true;
    private bool is_S_CheckBox = true;

    private void Start()
    {
        UiObjs = GetComponent<GameObject>();
        UiObjs = GameObject.Find("UiObjs");
        settingMenu = GFunc.FindChildObj(UiObjs, "SettingMenu");
        sha_CheckBox = GFunc.FindChildObj(UiObjs, "S_Check");
        clo_CheckBox = GFunc.FindChildObj(UiObjs, "C_Check");

        Sen_goldot = GFunc.FindChildObj(UiObjs, "gold");
        goldotPos = Sen_goldot.GetComponent<RectTransform>();

        sunFind = GameObject.Find("Directional Light");
        theSun = sunFind.GetComponent<Light>();
        moonFind = GameObject.Find("MoonLight");
        theMoon = moonFind.GetComponent<Light>();
        cloud = GameObject.Find("Clouds");
    }
    // { Game Menu Btn
    public void OnResume_Btn()
    {
        isResumeOn = true;
    }

    public void OnSetting_Btn()
    {
        settingMenu.SetActive(true);
        isSetMenuOpen = true;
    }

    public void OnExitGame_Btn()
    {
        GFunc.QuitThisGame();
    }
    // } Game Menu Btn

    // { Setting Menu Btn
    public void On_S_MenuBtn()
    {
        settingMenu.SetActive(false);
        isSetMenuOpen = false;
    }

    public void OnSensitivity50()
    {
        CameraMove.sensitivity = 50;
        goldotPos.anchoredPosition = new Vector3(-140, 0, 0);
    }
    public void OnSensitivity70()
    {
        CameraMove.sensitivity = 70;
        goldotPos.anchoredPosition = new Vector3(-70, 0, 0);
    }
    public void OnSensitivity100()
    {
        CameraMove.sensitivity = 100;
        goldotPos.anchoredPosition = new Vector3(0, 0, 0);
    }
    public void OnSensitivity150()
    {
        CameraMove.sensitivity = 150;
        goldotPos.anchoredPosition = new Vector3(70, 0, 0);
    }
    public void OnSensitivity200()
    {
        CameraMove.sensitivity = 200;
        goldotPos.anchoredPosition = new Vector3(140, 0, 0);
    }

    public void OnPlus_Volum()
    {

    }
    public void OnMinus_Volum()
    {

    }

    public void OnCloudCheck()
    {
        is_C_CheckBox = !is_C_CheckBox;
        if (is_C_CheckBox == true)
        {
            cloud.SetActive(true);
            clo_CheckBox.SetActive(true);
        }
        else
        {
            cloud.SetActive(false);
            clo_CheckBox.SetActive(false);
        }
    }

    public void OnShadowCheck()
    {
        is_S_CheckBox = !is_S_CheckBox;
        if (is_S_CheckBox == true)
        {
            theSun.shadows = LightShadows.Soft;
            theMoon.shadows = LightShadows.Soft;
            sha_CheckBox.SetActive(true);
        }
        else
        {
            theSun.shadows = LightShadows.None;
            theMoon.shadows = LightShadows.None;
            sha_CheckBox.SetActive(false);
        }
    }
    // } Setting Menu Btn
}
