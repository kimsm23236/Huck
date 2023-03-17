using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private float offsetY = 10f;
    private float offsetZ = -10f;
    private float speed = 10f;
    public GameObject target;
    private Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPos = new Vector3(target.transform.position.x, target.transform.position.y + offsetY, target.transform.position.z + offsetZ);
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
    }
}
