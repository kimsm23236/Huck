using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    private Transform cameraTr;
    // Start is called before the first frame update
    void Start()
    {
        cameraTr = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        cameraTr.LookAt(player);
    }
}
