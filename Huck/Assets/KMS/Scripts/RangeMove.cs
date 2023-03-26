using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMove : MonoBehaviour
{
    private GameObject camera_1p = default;

    void Start()
    {
        camera_1p = Camera.main.gameObject;
    }

    void Update()
    {
        // Follow camera
        gameObject.transform.position = camera_1p.transform.position;
        gameObject.transform.rotation = camera_1p.transform.rotation;
    }
}
