using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField]
    private Item nearItem = null;

    private bool isInvenOpen = false;

    private Rigidbody playerRigid = default;
    public GameObject inven = default;
    [SerializeField]
    private InventoryArray invenSlot = default;

    private float jumpForce = 150;
    private float moveSpeed = 20;
    private bool isGrounded = default;
    private bool isDead = false;
    private int maxJumpCnt = 1;
    private int curJumpCnt = 0;


    private void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        invenSlot = inven.transform.GetChild(0).GetChild(0).GetComponent<InventoryArray>();
    }

    private void Update()
    {
        Move();
        InvenOpen();
        if (nearItem != null && Input.GetKeyDown(KeyCode.E))
        {
            invenSlot.AddItem(nearItem);
            Destroy(nearItem.gameObject);
        }


    }

    public void InvenOpen()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInvenOpen = !isInvenOpen;
            if (isInvenOpen == true)
            {
                Cursor.visible = true;
                inven.SetActive(true);
            }
            if (isInvenOpen == false)
            {
                Cursor.visible = false;
                inven.SetActive(false);
            }
        }
    }

    // { Player Move
    private void Move()
    {
        if (isDead == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (PlayerAtk.isAttacking == true)
            {
                horizontal = 0;
                vertical = 0;
            }

            Vector3 move_H = transform.right * horizontal;
            Vector3 move_V = transform.forward * vertical;
            Vector3 velocity_P = (move_H + move_V).normalized * moveSpeed;

            playerRigid.MovePosition(transform.position + velocity_P * Time.deltaTime);


            // { inclined plane issue
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
            }
            // } inclined plane issue

            // { Player Velocity Move to Behind
            if (Input.GetKey(KeyCode.S))
            {
                moveSpeed = 10;
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                moveSpeed = 20;
            }
            // } Player Velocity Move to Behind

            // { Player Run
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                moveSpeed = 30;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W))
            {
                moveSpeed = 20;
            }
            // } Player Run
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            nearItem = other.GetComponent<Item>();
        }
    }
}
