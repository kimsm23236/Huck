using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPointCon : MonoBehaviour
{
    private GameObject TopLeft;
    private GameObject TopRight;
    
    private GameObject Left;
    private GameObject Right;
    private GameObject Top;
    private GameObject Bottom;

    /**
    [0] Top
    [1] Bottom
    [2] Left
    [3] Right
    [4] TopLeft
    [5] TopRight
    */
    private bool[] IsOn = new bool[6];
    private List<string>[] IsName = new List<string>[6];

    public bool IsBuild;

    void Start()
    {
        for(int i = 0; i < 6; i++)
        {
            IsName = new List<string>[i];
        }

        GameObject RootTemp = transform.parent.gameObject.FindChildObj("BuildPart").FindChildObj("WallPoint");
        Top = RootTemp.transform.GetChild(0).gameObject;
        Bottom = RootTemp.transform.GetChild(1).gameObject;
        Left = RootTemp.transform.GetChild(2).gameObject;
        Right = RootTemp.transform.GetChild(3).gameObject;

        RootTemp = transform.parent.gameObject.FindChildObj("BuildPart").FindChildObj("FloorPoint");
        TopLeft = RootTemp.transform.GetChild(0).gameObject;
        TopRight = RootTemp.transform.GetChild(1).gameObject;
    }

    void Update()
    {
        CallingCollider();
        CheckUpdate();
    }

    public void CheckIsBuild(bool Isbuild_)
    {
        IsBuild = Isbuild_;
    }

    private void CheckUpdate()
    {
        if (IsBuild)
        {
            if (Top.activeSelf != IsOn[0])
            {
                Top.SetActive(IsOn[0]);
            }

            if (Bottom.activeSelf != IsOn[1])
            {
                Bottom.SetActive(IsOn[1]);
            }

            if (Left.activeSelf != IsOn[2])
            {
                Left.SetActive(IsOn[2]);
            }

            if (Right.activeSelf != IsOn[3])
            {
                Right.SetActive(IsOn[3]);
            }

            if (TopLeft.activeSelf != IsOn[4])
            {
                TopLeft.SetActive(IsOn[4]);
            }

            if (TopRight.activeSelf != IsOn[5])
            {
                TopRight.SetActive(IsOn[5]);
            }
        }
    }

    private void CallingCollider()
    {
        if (Top.activeSelf == true)
        {
            if (Top.GetComponent<CheckTrigger>().IsOnCollider == true)
            {
                IsOn[0] = false;
            }
        }

        if (Bottom.activeSelf == true)
        {
            if (Bottom.GetComponent<CheckTrigger>().IsOnCollider == true)
            {
                IsOn[1] = false;
            }
        }

        if (Left.activeSelf == true)
        {
            if (Left.GetComponent<CheckTrigger>().IsOnCollider == true)
            {
                IsOn[2] = false;
            }
        }

        if (Right.activeSelf == true)
        {
            if (Right.GetComponent<CheckTrigger>().IsOnCollider == true)
            {
                IsOn[3] = false;
            }
        }

        if (TopLeft.activeSelf == true)
        {
            if (TopLeft.GetComponent<CheckTrigger>().IsOnCollider == true)
            {
                IsOn[4] = false;
            }
        }

        if (TopRight.activeSelf == true)
        {
            if (TopRight.GetComponent<CheckTrigger>().IsOnCollider == true)
            {
                IsOn[5] = false;
            }
        }
    }
}
