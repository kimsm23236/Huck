using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtk : MonoBehaviour
{
    private Animator atkAnim = default;
    public static bool isAttacking = false;

    private void Start() 
    {
        atkAnim = GetComponent<Animator>();
    }

    private void Update() 
    {        
        Attack();
    }

    // { Player Attack
    public void Attack()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            if(isAttacking == false)
            {
                atkAnim.SetBool("isAtk",true);
                isAttacking = true;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            StartCoroutine(AtkDelay());
        }

        if(Input.GetMouseButtonDown(1))
        {
            
        }
        
    }

    private IEnumerator AtkDelay()
    {
        yield return new WaitForSeconds(1.2f);
        atkAnim.SetBool("isAtk",false);
        isAttacking = false;
    }
    // } Player Attack
}
