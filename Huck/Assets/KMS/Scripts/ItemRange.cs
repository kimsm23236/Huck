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
    public StoveItem stoveItem = default;


    private float Range = 5;

    void Start()
    {
        UI = UIManager.Instance.UiObjs.transform.GetChild(1).gameObject;
        res_UI = UI.transform.GetChild(1).gameObject;
        ItemFound = UI.transform.GetChild(2).gameObject;

        find_T_panel = res_UI.transform.GetChild(0).gameObject;
        find_R_Hp = res_UI.transform.GetChild(2).gameObject;
        find_T_interect = res_UI.transform.GetChild(3).gameObject;
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
        RaycastHit[] hitinfo = default;
        RaycastHit hitItem = default;

        ItemFound.SetActive(false);
        res_UI.SetActive(false);

        ItemFound.SetActive(false);
        res_UI.SetActive(false);

        if (Physics.Raycast(transform.position,
        transform.TransformDirection(Vector3.forward),
        out hitItem, Range) && PlayerOther.isInvenOpen == false && PlayerOther.isMapOpen == false)
        {
            if (hitItem.transform.tag == "Item")
            {
                ItemFound.SetActive(true);
                getItem = hitItem.transform.gameObject;
            }
            else
            {
                getItem = default;
            }
            if (hitItem.transform.tag == "Stove" && PlayerOther.isAnvilOpen == false && PlayerOther.isInvenOpen == false
                && PlayerOther.isMapOpen == false && PlayerOther.isMenuOpen == false && PlayerOther.isWorkbenchOpen == false
                && PlayerMove.isWalk == false && PlayerMove.isRunning == false && PlayerMove.isJump == false
                && PlayerAtk.isAttacking == false && PlayerMove.isEating == false)
            {
                stoveItem = hitItem.transform.GetComponent<StoveItem>();
                var interactResObj = hitItem.transform.gameObject.GetComponentMust<BaseResourceObject>();
                int resObjMaxHp = interactResObj.ResourceConfig.HP;
                int resObjCurrentHP = interactResObj.HP;

                res_UI.SetActive(true);
                panel_T.text = "Stove";
                interect_T.text = "Stove";
                res_Hp.fillAmount = ((float)resObjCurrentHP / (float)resObjMaxHp);
                ItemFound.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerOther.isStoveOpen = !PlayerOther.isStoveOpen;
                    if (PlayerOther.isStoveOpen == true)
                    {
                        UIManager.Instance.stove.SetActive(true);
                        UI.transform.GetChild(0).gameObject.SetActive(false);
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        UIManager.Instance.stove.SetActive(false);
                        UI.transform.GetChild(0).gameObject.SetActive(true);
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
                else
                {
                    stoveItem = default;
                }
            }

            if (hitItem.transform.tag == "Workbench" && PlayerOther.isAnvilOpen == false && PlayerOther.isInvenOpen == false
                && PlayerOther.isMapOpen == false && PlayerOther.isMenuOpen == false && PlayerOther.isStoveOpen == false
                && PlayerMove.isWalk == false && PlayerMove.isRunning == false && PlayerMove.isJump == false
                && PlayerAtk.isAttacking == false && PlayerMove.isEating == false)
            {
                var interactResObj = hitItem.transform.gameObject.GetComponentMust<BaseResourceObject>();
                int resObjMaxHp = interactResObj.ResourceConfig.HP;
                int resObjCurrentHP = interactResObj.HP;
                res_UI.SetActive(true);
                panel_T.text = "Workbench";
                interect_T.text = "Workbench";
                res_Hp.fillAmount = ((float)resObjCurrentHP / (float)resObjMaxHp);
                ItemFound.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerOther.isWorkbenchOpen = !PlayerOther.isWorkbenchOpen;
                    if (PlayerOther.isWorkbenchOpen == true)
                    {
                        UIManager.Instance.workBench.SetActive(true);
                        UI.transform.GetChild(0).gameObject.SetActive(false);
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        UIManager.Instance.workBench.SetActive(false);
                        UI.transform.GetChild(0).gameObject.SetActive(true);
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }

            if (hitItem.transform.tag == "Anvil" && PlayerOther.isWorkbenchOpen == false && PlayerOther.isInvenOpen == false
                && PlayerOther.isMapOpen == false && PlayerOther.isMenuOpen == false && PlayerOther.isStoveOpen == false
                && PlayerMove.isWalk == false && PlayerMove.isRunning == false && PlayerMove.isJump == false
                && PlayerAtk.isAttacking == false && PlayerMove.isEating == false)
            {
                var interactResObj = hitItem.transform.gameObject.GetComponentMust<BaseResourceObject>();
                int resObjMaxHp = interactResObj.ResourceConfig.HP;
                int resObjCurrentHP = interactResObj.HP;
                res_UI.SetActive(true);
                panel_T.text = "Anvil";
                interect_T.text = "Anvil";
                res_Hp.fillAmount = ((float)resObjCurrentHP / (float)resObjMaxHp);
                ItemFound.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerOther.isAnvilOpen = !PlayerOther.isAnvilOpen;
                    if (PlayerOther.isAnvilOpen == true)
                    {
                        UIManager.Instance.anvil.SetActive(true);
                        UI.transform.GetChild(0).gameObject.SetActive(false);
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        UIManager.Instance.anvil.SetActive(false);
                        UI.transform.GetChild(0).gameObject.SetActive(true);
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
        }





        hitinfo = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), Range);
        if (hitinfo != null && PlayerOther.isWorkbenchOpen == false && PlayerOther.isInvenOpen == false
                && PlayerOther.isMapOpen == false && PlayerOther.isMenuOpen == false && PlayerOther.isStoveOpen == false)
        {
            for (int i = 0; i < hitinfo.Length; i++)
            {
                RaycastHit hit = hitinfo[i];

                if (hit.transform.tag == "Gather")
                {
                    res_UI.SetActive(true);
                    var interactResObj = hit.transform.gameObject.GetComponentMust<BaseResourceObject>();
                    string resName = interactResObj.ResourceConfig.ResourceName;
                    int resObjMaxHp = interactResObj.ResourceConfig.HP;
                    int resObjCurrentHP = interactResObj.HP;

                    panel_T.text = $"{resName}";
                    interect_T.text = $"{resName}";
                    res_Hp.fillAmount = ((float)resObjCurrentHP / (float)resObjMaxHp);
                }
            }
        }
        // } Found Item & Get Item
    }
}