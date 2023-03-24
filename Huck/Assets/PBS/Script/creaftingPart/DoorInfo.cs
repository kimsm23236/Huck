using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInfo : MonoBehaviour
{
    private bool IsOpen = false;
    private bool IsWalk = false;
    private GameObject doorObj;

    private void Awake()
    {
        doorObj = gameObject.transform.parent.gameObject;
    }
    private void Update()
    {
        if (IsWalk == true)
        {
            if (IsOpen == false)
            {
                if (doorObj.transform.localRotation.eulerAngles.y >= 120.0f)
                {
                    doorObj.transform.localRotation = Quaternion.Euler(0, 120.0f, 0);
                    IsOpen = true;
                    IsWalk = false;
                }
                else 
                {
                    doorObj.transform.Rotate(new Vector3(0, 60.0f, 0) * Time.deltaTime);
                }
            }
            else if (IsOpen == true)
            {
                if (doorObj.transform.localRotation.eulerAngles.y <= 5.0f)
                {
                    doorObj.transform.localRotation = Quaternion.Euler(0, 0.0f, 0);
                    IsOpen = false;
                    IsWalk = false;
                }
                else
                {
                    doorObj.transform.Rotate(new Vector3(0, -60.0f, 0) * Time.deltaTime);
                }
            }
        }
    }

    public void IsTrigger()
    {
        IsWalk = true;
    }
}
