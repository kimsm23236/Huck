using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamageable
{
    public static int curHp = default;
    public static float curHungry = default;
    public static float curEnergy = default;

    public int maxHp = 100;
    public float maxHungry = 100;
    public float maxEnergy = 100;

    public int damage = 1;
    public int armor = 0;
    public float critical = 5;

    private bool isEgFull = default;
    private bool isHgEmpty = default;
    private bool isHit = default;

    public delegate void EventHandler();
    public EventHandler onPlayerDead;

    private void Start()
    {
        curHp = maxHp;
        isHit = false;
        curHungry = maxHungry;
        curEnergy = maxEnergy;

        onPlayerDead = new EventHandler(() => Debug.Log("Player Dead"));
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
            && PlayerMove.isGrounded == true
            && curHungry != 0)
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

    // { TakeDamage
    public void TakeDamage(DamageMessage message)
    {
        if (isHit == false)
        {
            curHp -= message.damageAmount;
            StartCoroutine(WaitHitTime());
        }
        if (curHp <= 0f)
        {
            onPlayerDead();
            Die();
        }
    }

    private IEnumerator WaitHitTime()
    {
        isHit = true;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        isHit = false;
    } // WaitHitTime
    // } TakeDamage
    // { Player Die
    #region Die
    private void Die()
    {
        UIManager.Instance.Dead.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    #endregion
    // { Player Die
}
