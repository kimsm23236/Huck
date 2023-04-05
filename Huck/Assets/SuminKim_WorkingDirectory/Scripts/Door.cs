using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDoorState
{
    NONE = -1,
    OPEN, CLOSE
}
public class Door : MonoBehaviour, IInteractable
{
    private Animator doorAnimator = default;
    private EDoorState doorState = default;
    public BossRoomLightControl bossRoomLightControl = default;

    private bool isOnceProcess = true;
    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = gameObject.GetComponentMust<Animator>();
        doorState = EDoorState.CLOSE;
    }
    public void Execute()
    {
        OpenDoor();
    }
    private void OpenDoor()
    {
        if(doorState == EDoorState.OPEN)
            return;
        if (!GameManager.Instance.IsMidBossClear)
            return;

        if(isOnceProcess)
        {
            bossRoomLightControl.onCloseDoor();
            isOnceProcess = false;
        }
        doorState = EDoorState.OPEN;
        doorAnimator.SetTrigger("DoorOpen");
        GameManager.Instance.BossSpwan();
    }
    public void CloseDoor()
    {
        if(doorState == EDoorState.CLOSE)
            return;
        doorState = EDoorState.CLOSE;
        doorAnimator.SetTrigger("DoorClose");
    }
}
