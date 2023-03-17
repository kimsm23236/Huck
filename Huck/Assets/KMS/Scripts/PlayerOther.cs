using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOther : MonoBehaviour
{
    public GameObject inven = default;
    bool isInvenOpen = false;
    bool isMapOpen = false;

    private void Start()
    {
        CursorSet();
    }

    private void Update() 
    {
        InvenOpen();
    }
    
    // { Player Inventory
    public void InvenOpen()
    {

        if(Input.GetKeyDown(KeyCode.Tab))
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
    // } Player Inventory 
      

    // { Player Interaction 
    public void Interaction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            
        }
    }
    // } Player Interaction

    // { Player Map
    public void MapOpen()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            isMapOpen = !isMapOpen;
            if(isMapOpen == true)
            {
                Cursor.visible = true;

            }
            if(isMapOpen == false)
            {

            }
        }
    }
    // } Player Map

    // { Cursor Setting
    private void CursorSet()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    // } Cursor Setting
}
