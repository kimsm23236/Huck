using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseResourceObject : MonoBehaviour
{
    [SerializeField] 
    protected ResourceObjectSO resConfig;

    public GameObject hpBar;
    
    public ResourceObjectSO ResourceConfig
    {
        set { resConfig = value; }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
