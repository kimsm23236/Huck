using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloser : MonoBehaviour
{
    public Door bindingDoor = default;
    public BossRoomLightControl bossRoomLightControl = default;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == GData.PLAYER_MASK)
        {
            bindingDoor.CloseDoor();
            // bossRoomLightControl.onCloseDoor();
            Destroy(gameObject);
        }
    }
}
