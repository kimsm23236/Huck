using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody playerRigid = default;
    private Animator playerAnim = default;
    private InHand playerInHand = default;

    public static bool isGrounded = default;
    public static bool isRunning = default;
    public static bool isDead = false;
    public static bool isJump = false;

    private float jumpForce = 150;
    private float moveSpeed = 5;
    private int curJumpCnt = 0;


    private void Start()
    {
        isDead = false;
        playerRigid = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerInHand = GetComponent<InHand>();
    }

    private void Update()
    {
        Input_();
        PlayerRotate();
        Eat();
    }

    private void FixedUpdate()
    {
        PlayerAction();
    }

    private void Input_()
    {
        if (PlayerOther.isMenuOpen == false)
        {
            MoveInput();
            JumpInput();
        }
    }

    private void PlayerAction()
    {
        if (isDead == false)
        {
            Move();
            Jump();
        }
        Die();
    }

    // { Player Move
    #region Move
    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move_H = transform.right * horizontal;
        Vector3 move_V = transform.forward * vertical;
        Vector3 velocity_P = (move_H + move_V).normalized * moveSpeed;
        playerRigid.MovePosition(transform.position + velocity_P
            * Time.deltaTime);

        if (isGrounded == true)
        {
            playerAnim.SetInteger("WalkFB", (int)vertical);
            playerAnim.SetInteger("WalkRL", (int)horizontal);
        }
    }

    private void MoveInput()
    {
        // { inclined plane issue
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            playerAnim.SetInteger("WalkFB", 0);
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            playerAnim.SetInteger("WalkRL", 0);
        }
        // } inclined plane issue

        // { Player Velocity Move to Behind
        if (Input.GetKey(KeyCode.S))
        {
            playerAnim.SetBool("isRunning", false);
            moveSpeed = 3;
        }
        // } Player Velocity Move to Behind

        // { Player Run
        if (Input.GetKey(KeyCode.LeftShift) && PlayerStat.curEnergy > 0)
        {
            if (Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.D))
            {
                isRunning = true;
                playerAnim.SetBool("isRunning", true);
                moveSpeed = 8;
                PlayerStat.curEnergy -= 10f * Time.deltaTime;
                if (PlayerStat.curEnergy < 1)
                {
                    isRunning = false;
                    playerAnim.SetBool("isRunning", false);
                    moveSpeed = 5;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W)
        || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)
        || Input.GetKeyUp(KeyCode.S))
        {
            isRunning = false;
            playerAnim.SetBool("isRunning", false);
            moveSpeed = 5;
        }

        // } Player Run
    }
    #endregion
    // } Player Move

    // { Player Rotation
    #region Rotate        
    private void PlayerRotate()
    {
        if (isDead == false)
        {
            float c_RotateY = Input.GetAxisRaw("Mouse X")
                * Time.deltaTime * CameraMove.sensitivity;
            Vector3 p_RotateY = new Vector3(0, c_RotateY, 0);
            playerRigid.MoveRotation(playerRigid.rotation
                * Quaternion.Euler(p_RotateY));
        }
    }
    #endregion
    // } Player Rotation

    // { Player Jump
    #region Jump
    private void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && PlayerStat.curEnergy > 10)
        {
            isJump = true;
        }
    }

    private void Jump()
    {
        if (curJumpCnt == 0 && isJump == true)
        {
            curJumpCnt++;
            playerAnim.SetTrigger("Jump");
            playerAnim.SetBool("isGround", false);
            playerRigid.AddForce(Vector3.up * jumpForce);
            PlayerStat.curEnergy -= 10;
            PlayerStat.curHungry -= 0.1666f;
            isJump = false;
            isGrounded = false;
            StartCoroutine(JumpDelay());
        }
    }

    private IEnumerator JumpDelay()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnim.SetTrigger("JumpCancel");
        playerAnim.SetBool("isGround", true);
        isGrounded = true;
    }
    #endregion
    // } Player Jump

    // { Player Die
    #region Die
    private void Die()
    {
        if (PlayerStat.curHp == 0)
        {
            isDead = true;
            playerAnim.SetTrigger("Dead");
            PlayerStat.curHp = -1;
            UIManager.Instance.Dead.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    #endregion
    // { Player Die

    // { Player Eat
    #region Eat
    private void Eat()
    {
        if (Input.GetMouseButtonDown(1) && playerInHand.inventorySlotItem[playerInHand.selectedQuitSlot].itemData != null)
        {
            if (PlayerOther.isInvenOpen == false && PlayerOther.isMapOpen == false && PlayerOther.isMenuOpen == false)
            {
                playerAnim.SetTrigger("Eat");
            }
        }
        if (PlayerAtk.isAttacking == true || PlayerOther.isInvenOpen == true ||
            PlayerOther.isMapOpen == true || PlayerOther.isMenuOpen == true)
        {
            playerAnim.SetTrigger("EatCancel");
        }
    }
    private void EatFood()
    {
        playerInHand.inventorySlotItem[playerInHand.selectedQuitSlot].itemUseDel(playerInHand.inventorySlotItem[playerInHand.selectedQuitSlot]);
        Debug.Log("먹음");
    }
    private void EatFin()
    {
        playerAnim.SetTrigger("EatCancel");
    }
    #endregion
    // } Player Eat

    // { Player Grounded Check
    #region Grounded
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            isGrounded = true;
            isJump = false;
            curJumpCnt = 0;
            playerAnim.SetBool("isGround", true);
        }

        if (other.gameObject.tag == "Enemy")
        {
            PlayerStat.curHp = 0;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Floor")
        {
            isGrounded = false;
            playerAnim.SetBool("isGround", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
        isJump = false;
        curJumpCnt = 0;
        playerAnim.SetBool("isGround", true);
    }
    #endregion
    // } Player Grounded Check
}
