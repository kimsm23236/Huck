using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRange : MonoBehaviour
{
    private GameObject ItemFound = default;
    private GameObject resourceFound = default;

    private GameObject res_UI = default;
    private GameObject find_R_Hp = default;
    private GameObject find_T_panel = default;
    private GameObject find_T_interect = default;

    private Image res_Hp = default;
    private Text panel_T = default;
    private Text interect_T = default;

    public GameObject getItem = default;

    private float Range = 5;

    void Start()
    {
        //Find Object & Cashing
        GameObject UiObjs = GameObject.Find("UiObjs");
        ItemFound = UiObjs.FindChildObj("ItemFound");
        res_UI = UiObjs.FindChildObj("Res_UI");

        find_R_Hp = res_UI.FindChildObj("Hp_Bar");
        find_T_panel = res_UI.FindChildObj("PanelText");
        find_T_interect = res_UI.FindChildObj("InteractText");
        res_Hp = find_R_Hp.GetComponent<Image>();
        panel_T = find_T_panel.GetComponent<Text>();
        interect_T = find_T_interect.GetComponent<Text>();
    }

    void Update()
    {
        ItemGet();
    }

    // { Found Item & Get Item
    void ItemGet()
    {
        ItemFound.SetActive(false);
        res_UI.SetActive(false);

        RaycastHit hitinfo = default;

        if (Physics.Raycast(transform.position,
        transform.TransformDirection(Vector3.forward),
        out hitinfo, Range) && PlayerOther.isInvenOpen == false && PlayerOther.isMapOpen == false)
        {
            if (hitinfo.transform.tag == "Item")
            {
                ItemFound.SetActive(true);
                getItem = hitinfo.transform.gameObject;
            }
            else
            {
                getItem = default;
            }

            if (hitinfo.transform.tag == "Gather")
            {
                res_UI.SetActive(true);
                var interactResObj = hitinfo.transform.gameObject.GetComponentMust<BaseResourceObject>();
                string resName = interactResObj.ResourceConfig.ResourceName;
                int resObjMaxHp = (int)interactResObj.ResourceConfig.HP;
                int resObjCurrentHP = (int)interactResObj.ResourceConfig.HP;

                panel_T.text = $"{resName}";
                interect_T.text = $"{resName}";
                res_Hp.fillAmount = ((float)resObjCurrentHP / (float)resObjMaxHp);
            }
        }
    }
    // } Found Item & Get Item
}