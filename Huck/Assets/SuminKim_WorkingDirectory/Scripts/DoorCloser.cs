using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloser : MonoBehaviour
{
    public Door bindingDoor = default;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == GData.PLAYER_MASK)
        {
            bindingDoor.CloseDoor();
            Destroy(gameObject);
        }
    }
}
