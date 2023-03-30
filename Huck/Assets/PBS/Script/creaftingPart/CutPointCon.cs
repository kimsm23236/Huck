using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPointCon : MonoBehaviour
{
    private CheckTrigger Left;
    private CheckTrigger Right;

    void Start()
    {
        Left = transform.GetChild(0).GetComponent<CheckTrigger>();
        Right = transform.GetChild(1).GetComponent<CheckTrigger>();
    }

    void Update()
    {
        CallingCollider();
    }

    public void CheckDestoryName(string name)
    {
        // if (Left.otherName == name)
        // {
        //     transform.GetChild(0).gameObject.SetActive(true);
        // }

        // if (Right.otherName == name)
        // {
        //     transform.GetChild(1).gameObject.SetActive(true);
        // }
    }

    private void CallingCollider()
    {
        if (transform.GetChild(0).gameObject.activeSelf == true)
        {
            if (Left.IsOnCollider == true)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        if (transform.GetChild(1).gameObject.activeSelf == true)
        {
            if (Right.IsOnCollider == true)
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}
