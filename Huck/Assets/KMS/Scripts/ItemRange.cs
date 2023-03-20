using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRange : MonoBehaviour
{
    public GameObject camera_1p = default;
    public GameObject ItemFound = default;
    private float Range = 5;

    void Start()
    {
        
    }

    void Update()
    {
        ItemGet();
    }

    void ItemGet()
    {
        ItemFound.SetActive(false);
        RaycastHit hitinfo = default;

        if(Physics.Raycast(transform.position, 
        transform.TransformDirection(Vector3.forward), 
        out hitinfo, Range))
        {
            if(hitinfo.transform.tag == "Item")
            {
                ItemFound.SetActive(true);
            }
        }
    }
}
