using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRange : MonoBehaviour
{
    private GameObject ItemFound = default;
    private GameObject UI = default;
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
        UI = UIManager.Instance.UiObjs.transform.GetChild(1).gameObject;
        res_UI = UI.transform.GetChild(1).gameObject;
        ItemFound = UI.transform.GetChild(2).gameObject;

        find_T_panel = res_UI.transform.GetChild(0).gameObject;
        find_R_Hp = res_UI.transform.GetChild(2).gameObject;
        find_T_interect = res_UI.transform.GetChild(3).gameObject;

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
        RaycastHit hitinfo = default;

        ItemFound.SetActive(false);
        res_UI.SetActive(false);

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
                interect_T.text = $"{resName}";
                res_Hp.fillAmount = ((float)resObjCurrentHP / (float)resObjMaxHp);
            }

            if (hitinfo.transform.name == "10.Stove(Clone)" && PlayerOther.isAnvilOpen == false && PlayerOther.isInvenOpen == false
                && PlayerOther.isMapOpen == false && PlayerOther.isMenuOpen == false && PlayerOther.isWorkbenchOpen == false)
            {
                res_UI.SetActive(true);
                panel_T.text = "Stove";
                interect_T.text = "Stove";
                ItemFound.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerOther.isStoveOpen = !PlayerOther.isStoveOpen;
                    if (PlayerOther.isStoveOpen == true)
                    {
                        UIManager.Instance.stove.SetActive(true);
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        UIManager.Instance.stove.SetActive(false);
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
        }
    }
    // } Found Item & Get Item
}