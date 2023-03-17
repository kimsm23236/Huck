using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOther : MonoBehaviour
{
    public GameObject inven = default;
    public GameObject map = default;
    public static bool isInvenOpen = false;
    public static bool isMapOpen = false;

    private void Start()
    {
        CursorSet();
    }

    private void Update() 
    {
        InvenOpen();
        MapOpen();
    }
    
    // { Player Inventory
#region Inven
    public void InvenOpen()
    {

        if(Input.GetKeyDown(KeyCode.Tab) && isMapOpen == false)
        {
            isInvenOpen = !isInvenOpen;
            if(isInvenOpen == true)
            {
                Cursor.visible = true;
                inven.SetActive(true);
            }
            if(isInvenOpen == false)
            {
                Cursor.visible = false;
                inven.SetActive(false);
            }
        }
    }
#endregion
    // } Player Inventory 
      

    // { Player Interaction 
#region Interact
    public void Interaction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            
        }
    }
#endregion
    // } Player Interaction

    // { Player Map
#region Map
    public void MapOpen()
    {
        if(Input.GetKeyDown(KeyCode.M) && isInvenOpen == false)
        {
            isMapOpen = !isMapOpen;
            if(isMapOpen == true)
            {
                Cursor.visible = true;
                map.SetActive(true);
            }
            if(isMapOpen == false)
            {
                Cursor.visible = false;
                map.SetActive(false);
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
        Cursor.lockState = CursorLockMode.Confined;
    }
#endregion
    // } Cursor Setting
}
