using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorObjTrigger : MonoBehaviour
{
    public bool IsCursor = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IsCursor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
