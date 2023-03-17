using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody playerRigid = default;
    private Animator playerAnim = default;

    private float jumpForce = 150;
    private float moveSpeed = 20;
    private bool isGrounded = default;
    private bool isDead = false;
    private int maxJumpCnt = 1;
    private int curJumpCnt = 0;

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        Jump();
        PlayerRotate();
    }

    // { Player Move
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
                moveSpeed = 10;
            }
            if(Input.GetKeyUp(KeyCode.S))
            {
                moveSpeed = 20;
            }
            // } Player Velocity Move to Behind

            // { Player Run
            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                playerAnim.SetBool("isRunning", true);
                moveSpeed = 30;
            }
            if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W))
            {
                playerAnim.SetBool("isRunning", false);
                moveSpeed = 20;
            }
            // } Player Run
        }
    }
    // } Player Move

    // { Player Rotation
    private void PlayerRotate()
    {
        float c_RotateY = Input.GetAxisRaw("Mouse X") * Time.deltaTime * CameraMove.sensitivity;
        Vector3 p_RotateY = new Vector3(0,c_RotateY,0);
        playerRigid.MoveRotation(playerRigid.rotation * Quaternion.Euler(p_RotateY));
    }
    // } Player Rotation

    // { Player Jump
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && PlayerAtk.isAttacking == false)
        {
            if(curJumpCnt == 0)
            {
                playerAnim.SetBool("isGround",false);
                playerRigid.AddForce(Vector3.up * jumpForce);
                curJumpCnt++;
            }
        }
        if(curJumpCnt == 1 && isGrounded == true)
        {
            curJumpCnt = 0;
        }
        if(curJumpCnt > maxJumpCnt)
        {
            curJumpCnt = 1;
        }
    }   
    // } Player Jump

    // { Player Die
    // { Player Die

    // { Player Grounded Check
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Floor")
        {
            isGrounded = true;
            playerAnim.SetBool("isGround",true);
        }
    }
    
    private void OnCollisionExit(Collision other) 
    {
        if(other.gameObject.tag == "Floor")
        {
            isGrounded = false;
        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Floor")
        {
            Debug.Log("뒤에 감지!");
            isGrounded = true;
            playerAnim.SetBool("isGround",true);
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.tag == "Floor")
        {
            Debug.Log("떨어졌다!");
            isGrounded = false;
        }
    }
    // } Player Grounded Check
}
