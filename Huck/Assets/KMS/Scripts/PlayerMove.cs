using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody playerRigid = default;
    private Animator playerAnim = default;

    public static bool isGrounded = default;
    private bool isDead = false;
    private bool isJump = false;

    private float jumpForce = 150;
    private float moveSpeed = 5;
    private int curJumpCnt = 0;
    

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        Input_();
        PlayerRotate();
    }

    private void FixedUpdate() 
    {
        Move();        
        Jump();
    }

    private void Input_()
    {
        MoveInput();
        JumpInput();
    }

    // { Player Move
#region Move
    private void Move()
    {        
        if(isDead == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if(PlayerAtk.isAttacking == true)
            {
                horizontal = 0;
                vertical = 0;
            }

            Vector3 move_H = transform.right * horizontal;
            Vector3 move_V = transform.forward * vertical;
            Vector3 velocity_P = (move_H + move_V).normalized * moveSpeed;
            playerRigid.MovePosition(transform.position + velocity_P * Time.deltaTime);

            if(isGrounded == true)
            {
                playerAnim.SetInteger("WalkFB", (int)vertical);
                playerAnim.SetInteger("WalkRL", (int)horizontal);
            }
        }

        if(isDead == true)
        {
            Die();
        }
    }

    private void MoveInput()
    {
        // { inclined plane issue
        if(Input.GetKeyUp(KeyCode.W)||Input.GetKeyUp(KeyCode.S))
        {
            playerAnim.SetInteger("WalkFB", 0);
        }
        if(Input.GetKeyUp(KeyCode.A)||Input.GetKeyUp(KeyCode.D))
        {
            playerAnim.SetInteger("WalkRL", 0);
        }
        // } inclined plane issue

        // { Player Velocity Move to Behind
        if(Input.GetKey(KeyCode.S))
        {
            playerAnim.SetBool("isRunning", false);
            moveSpeed = 3;
        }
        if(Input.GetKeyUp(KeyCode.S))
        {
            moveSpeed = 5;
        }
        // } Player Velocity Move to Behind

        // { Player Run
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            playerAnim.SetBool("isRunning", true);
            moveSpeed = 8;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W))
        {
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
        float c_RotateY = Input.GetAxisRaw("Mouse X") * Time.deltaTime * CameraMove.sensitivity;
        Vector3 p_RotateY = new Vector3(0,c_RotateY,0);
        playerRigid.MoveRotation(playerRigid.rotation * Quaternion.Euler(p_RotateY));
    }
#endregion
    // } Player Rotation

    // { Player Jump
#region Jump
    private void JumpInput()
    {
        if(Input.GetKeyDown(KeyCode.Space) && PlayerAtk.isAttacking == false)
        {
            isJump = true;
        }
    }

    private void Jump()
    {
        if(curJumpCnt == 0 && isJump == true)
        {
            curJumpCnt++;
            playerAnim.SetTrigger("Jump");
            playerAnim.SetBool("isGround",false);
            playerRigid.AddForce(Vector3.up * jumpForce);
            isJump = false;
            isGrounded = false;
            StartCoroutine(JmupDelay());
        }
    }

    private IEnumerator JmupDelay()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnim.SetTrigger("JumpCancel");
        playerAnim.SetBool("isGround",true);
        isGrounded = true;
    }
#endregion
    // } Player Jump

    // { Player Die
#region Die
    private void Die()
    {
        playerAnim.SetTrigger("Dead");
    }
#endregion
    // { Player Die

#region Hit
    // { Player Hit
    private void Hit()
    {

    }
    // } Player Hit

#endregion
    // { Player Grounded Check
#region Grounded
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Floor")
        {
            isGrounded = true;
            isJump = false;
            curJumpCnt = 0;
            playerAnim.SetBool("isGround",true);
        }

        if(other.gameObject.tag == "Monster")
        {
            Debug.Log("맞음");
        }
    }
    
    private void OnCollisionExit(Collision other) 
    {
        if(other.gameObject.tag == "Floor")
        {
            isGrounded = false;
            playerAnim.SetBool("isGround",false);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        isGrounded = true;
        isJump = false;
        curJumpCnt = 0;
        playerAnim.SetBool("isGround",true); 
    }
#endregion
    // } Player Grounded Check
}
