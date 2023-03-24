using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentSlot : MonoBehaviour
{
    public RectTransform curSlot_ = default;

    void Start()
    {

    }

    void Update()
    {
        curSlot();
    }

    private void curSlot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            curSlot_.anchoredPosition = new Vector3(-909, -488, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            curSlot_.anchoredPosition = new Vector3(-909 + 110, -488, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            curSlot_.anchoredPosition = new Vector3(-909 + 220, -488, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            curSlot_.anchoredPosition = new Vector3(-909 + 330, -488, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            curSlot_.anchoredPosition = new Vector3(-909 + 440, -488, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            curSlot_.anchoredPosition = new Vector3(-909 + 550, -488, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            curSlot_.anchoredPosition = new Vector3(-909 + 660, -488, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            curSlot_.anchoredPosition = new Vector3(-909 + 770, -488, 0);
        }
    }
}
