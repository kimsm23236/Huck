using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtk : MonoBehaviour
{
    private Animator atkAnim = default;
    private GameObject attackRange = default;

    public static bool isAttacking = false;
    private bool isAttack = false;

    private void Start()
    {
        atkAnim = GetComponent<Animator>();
        attackRange = gameObject.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        AtkInput();
    }

    private void FixedUpdate()
    {
        Attack();
    }

    // { Player Attack
    #region Player Attack
    private void AtkInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isAttack = true;
        }
    }
    private void Attack()
    {
        if (isAttack == true && PlayerOther.isInvenOpen == false
            && PlayerOther.isMapOpen == false && PlayerMove.isDead == false
                && PlayerOther.isStoveOpen == false)
        {
            if (isAttacking == false)
            {
                atkAnim.SetTrigger("Attack");
                isAttacking = true;
            }
        }
    }

    private void AttakCol_T()
    {
        attackRange.SetActive(true);
    }
    private void AttackCol_F()
    {
        attackRange.SetActive(false);
        atkAnim.SetTrigger("AtkCancel");
        isAttacking = false;
        isAttack = false;
    }
    #endregion
    // } Player Attack
}
