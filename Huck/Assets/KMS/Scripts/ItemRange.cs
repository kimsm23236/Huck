using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRange : MonoBehaviour
{
    private GameObject ItemFound = default;
    private GameObject resourceFound = default;

    private GameObject res_UI = default;
    public Image res_Hp = default;
    public Text panel_T = default;
    public Text interRect_T = default;

    public GameObject getItem = default;

    private float Range = 5;

    void Start()
    {
        //Find Object & Cashing
        GameObject UiObjs = GameObject.Find("UiObjs");
        ItemFound = UiObjs.FindChildObj("ItemFound");
        res_UI = UiObjs.FindChildObj("Res_UI");
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
                int resObjMaxHp = interactResObj.ResourceConfig.HP;
                int resObjCurrentHP = interactResObj.HP;

                panel_T.text = $"{resName}";
                interRect_T.text = $"{resName}";
                res_Hp.fillAmount = ((float)resObjCurrentHP / (float)resObjMaxHp);
            }
        }
    }
    // } Found Item & Get Item
}