using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOther : MonoBehaviour
{
    [SerializeField]
    private InventoryArray invenSlot = default;

    [SerializeField]
    private Item itemInfo = default;

    public GameObject menu = default;
    public GameObject inven = default;
    public GameObject map = default;
    public GameObject GUI = default;
    public GameObject invenCam = default;

    public static bool isInvenOpen = false;
    public static bool isMapOpen = false;
    public static bool isMenuOpen = false;

    private Vector3 enableScale = new Vector3(0.00001f, 0.00001f, 0.00001f);
    private Vector3 ableScale = new Vector3(1f, 1f, 1f);


    private void Start()
    {
        CursorSet();
        inven.SetLocalScale(enableScale);
        invenSlot = inven.transform.GetChild(0)
            .GetChild(1).GetComponent<InventoryArray>();
    }

    private void Update()
    {
        InvenOpen();
        MapOpen();
        MenuOpen();
        Interaction();
    }

    // { Player Inventory
    #region Inven
    public void InvenOpen()
    {

        if (Input.GetKeyDown(KeyCode.Tab) && isMapOpen == false)
        {
            isInvenOpen = !isInvenOpen;
            if (isInvenOpen == true)
            {
                gameObject.GetComponent<PlayerAtk>().enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                invenCam.SetActive(true);
                inven.SetLocalScale(ableScale);
                GUI.SetActive(false);
            }
            if (isInvenOpen == false)
            {
                gameObject.GetComponent<PlayerAtk>().enabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                invenCam.SetActive(false);
                inven.SetLocalScale(enableScale);
                GUI.SetActive(true);
            }
        }
    }
    #endregion
    // } Player Inventory 

    // { Player Interaction 
    #region Interact
    public void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

        }
        RootItem();
    }
    #endregion
    // } Player Interaction

    // { Player Map
    #region Map
    public void MapOpen()
    {
        if (Input.GetKeyDown(KeyCode.M) && isInvenOpen == false)
        {
            isMapOpen = !isMapOpen;
            if (isMapOpen == true)
            {
                gameObject.GetComponent<PlayerAtk>().enabled = false;
                map.SetActive(true);
                GUI.SetActive(false);
            }
            if (isMapOpen == false)
            {
                gameObject.GetComponent<PlayerAtk>().enabled = true;
                map.SetActive(false);
                GUI.SetActive(true);
            }
        }
    }
    #endregion
    // } Player Map

    // { Cursor Setting
    #region Cursor
    private void CursorSet()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion
    // } Cursor Setting

    //{ Game Menu
    #region Menu
    private void MenuOpen()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || UiManager.isResumeOn == true)
        {
            isMenuOpen = !isMenuOpen;
            if (isMenuOpen == true)
            {
                gameObject.GetComponent<PlayerAtk>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                menu.SetActive(true);
                Time.timeScale = 0;
            }
            if (isMenuOpen == false)
            {
                gameObject.GetComponent<PlayerAtk>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                UiManager.isResumeOn = false;
                menu.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
    #endregion
    //} Game Menu

    // { Item Root
    #region  RootItem
    private void RootItem()
    {
        if (Camera.main.GetComponent<ItemRange>().getItem != null)
        {
            itemInfo = Camera.main.GetComponent<ItemRange>().getItem.GetComponent<Item>();
        }
        if (itemInfo != null && Input.GetKeyDown(KeyCode.E))
        {
            invenSlot.AddItem(itemInfo);
            Destroy(itemInfo.gameObject);
        }
    }
    #endregion
    // } Item Root
}