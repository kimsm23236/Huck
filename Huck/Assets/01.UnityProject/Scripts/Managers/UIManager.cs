using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("드래그앤 드롭 X")]
    public GameObject inventory = default;
    public GameObject settingMenu = default;
    public GameObject clo_CheckBox = default;
    public GameObject sha_CheckBox = default;
    public GameObject workBench = default;
    public GameObject stove = default;
    public GameObject anvil = default;

    // [KMS] Dead UI
    public GameObject Dead = default;
    public Image dead_1 = default;
    public Text dead_2 = default;
    public Image dead_3 = default;
    public Text dead_4 = default;
    // [KMS] Dead UI

    [Header("드래그앤 드롭 O")]
    public GameObject UiObjs = default;
    public GameObject Sen_goldot = default;
    public GameObject sunFind = default;
    public GameObject moonFind = default;
    public GameObject cloud = default;
    [Header("다른 곳에서 쓸 Bool 변수")]
    public bool isResumeOn = false;
    public bool isSetMenuOpen = false;


    private GameObject dead1 = default;
    private GameObject dead2 = default;
    private GameObject dead3 = default;
    private GameObject dead4 = default;

    private RectTransform goldotPos = default;
    private bool is_C_CheckBox = true;
    private bool is_S_CheckBox = true;
    private Light theSun = default;
    private Light theMoon = default;

    private void Start()
    {
        // [KMS] Dead UI
        Dead = UiObjs.transform.GetChild(3).gameObject;
        dead1 = Dead.transform.GetChild(0).gameObject;
        dead2 = Dead.transform.GetChild(1).gameObject;
        dead3 = Dead.transform.GetChild(2).gameObject;
        dead4 = Dead.transform.GetChild(3).gameObject;

        dead_1 = dead1.GetComponent<Image>();
        dead_2 = dead2.GetComponent<Text>();
        dead_3 = dead3.GetComponent<Image>();
        dead_4 = dead4.GetComponent<Text>();
        // [KMS] Dead UI

        settingMenu = UiObjs.transform.GetChild(1).GetChild(5).gameObject;
        clo_CheckBox = settingMenu.transform.GetChild(5).GetChild(0).GetChild(0).gameObject;
        sha_CheckBox = settingMenu.transform.GetChild(6).GetChild(0).GetChild(0).gameObject;
        goldotPos = Sen_goldot.GetComponent<RectTransform>();

        theSun = sunFind.GetComponent<Light>();
        theMoon = moonFind.GetComponent<Light>();
    }

    protected override void Init()
    {
        workBench = UiObjs.transform.GetChild(2).GetChild(0).gameObject;
        stove = UiObjs.transform.GetChild(2).GetChild(1).gameObject;
        anvil = UiObjs.transform.GetChild(2).GetChild(2).gameObject;
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

    public void GoToTitle()
    {
        Debug.Log("GoToTitleClick");
        GFunc.LoadScene(GData.SCENENAME_TITLE);
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
