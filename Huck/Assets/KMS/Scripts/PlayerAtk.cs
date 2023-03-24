using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtk : MonoBehaviour
{
    private Animator atkAnim = default;
    public GameObject attackRange = default;

    public static bool isAttacking = false;
    private bool isAttack = false;

    private void Start()
    {
        atkAnim = GetComponent<Animator>();
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
            && PlayerOther.isMapOpen == false)
        {
            if (isAttacking == false)
            {
                atkAnim.SetTrigger("Attack");
                isAttacking = true;
                StartCoroutine(AtkDelay());
            }
        }
    }


    private IEnumerator AtkDelay()
    {
        yield return new WaitForSeconds(0.4f);
        attackRange.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackRange.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        atkAnim.SetTrigger("AtkCancel");
        isAttacking = false;
        isAttack = false;
    }
    #endregion
    // } Player Attack
}
