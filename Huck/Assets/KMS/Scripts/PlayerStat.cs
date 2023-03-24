using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static int curHp = default;
    public static float curHungry = default;
    public static float curEnergy = default;

    public int maxHp = 100;
    public float maxHungry = 100;
    public float maxEnergy = 100;

    public int damage = 10;
    public int armor = 0;
    public float critical = 5;

    private bool isEgFull = default;
    private bool isHgEmpty = default;

    private void Start()
    {
        curHp = maxHp;
        curHungry = maxHungry;
        curEnergy = maxEnergy;
    }

    private void Update()
    {
        Hungry();
        Energe();
        Hp();
    }

    // { Hp, Hungry, Energy
    #region Hp, Hungry, Energy
    private void Hp()
    {
        if (curHp != -1 && curHp < 0)
        {
            curHp = 0;
        }
        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
    }

    private void Hungry()
    {
        // Nature decrease
        if (curHungry > 0)
        {
            isHgEmpty = false;
            if (isHgEmpty == false)
            {
                curHungry -= 0.1666f * Time.deltaTime;
            }
        }
        if (curHungry <= 0)
        {
            curHungry = 0;
            isHgEmpty = true;
        }
        if (curHungry > maxHungry)
        {
            curHungry = maxHungry;
        }

        // +Act decrease
        if (PlayerMove.isRunning == true)
        {
            curHungry -= 0.1666f * Time.deltaTime;
        }
    }

    private void Energe()
    {
        // Nature increase
        if (curEnergy < maxEnergy)
        {
            isEgFull = false;
            if (isEgFull == false
            && Input.GetKey(KeyCode.LeftShift) == false
            && PlayerMove.isGrounded == true)
            {
                curEnergy += 10f * Time.deltaTime;
            }
        }
        if (curEnergy >= maxEnergy)
        {
            curEnergy = maxEnergy;
            isEgFull = true;
        }
        if (curEnergy < 0)
        {
            curEnergy = 0;
        }
        if (curEnergy < 1 && Input.GetKey(KeyCode.LeftShift))
        {
            curEnergy = 0;
        }
    }
    #endregion
    // } Hp, Hungry, Energy
}
