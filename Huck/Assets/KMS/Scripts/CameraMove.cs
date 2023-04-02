using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject head = default;
    private Transform player = default;

    public static float sensitivity = 100f;
    private float limitAngle = 80;
    private float cameraX, cameraY, rotateX, rotateY;

    private void Start()
    {
        GameObject player_ = GameManager.Instance.playerObj;
        player = player_.transform;
    }

    private void LateUpdate()
    {
        CameraMoving();
    }

    private void CameraMoving()
    {
        // if Player Alive
        if (PlayerMove.isDead == false && PlayerOther.isMenuOpen == false && PlayerOther.isStoveOpen == false
            && PlayerOther.isAnvilOpen == false && PlayerOther.isWorkbenchOpen == false
            && LoadingManager.Instance.isLoadingEnd == true)
        {
            CameraPos();
            CameraRotate();
        }

        // if Player Dead
        if (PlayerStat.curHp == 0)
        {
            this.transform.position =
                transform.position + new Vector3(0, 2, -2) * Time.deltaTime;

            this.transform.LookAt(player);
        }
    }

    // { Camera Position
    #region Position
    private void CameraPos()
    {
        gameObject.transform.position = head.transform.position;
    }
    #endregion
    // } Camera Position

    // { Camera Rotation
    #region Rotate
    private void CameraRotate()
    {
        cameraX = -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity / 2;
        cameraY = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;

        rotateX += cameraX;
        rotateX = Mathf.Clamp(rotateX, -limitAngle, limitAngle);

        rotateY = transform.eulerAngles.y + cameraY;

        transform.eulerAngles = new Vector3(rotateX, rotateY, 0);
    }
    #endregion
    // } Camera Rotation
}
