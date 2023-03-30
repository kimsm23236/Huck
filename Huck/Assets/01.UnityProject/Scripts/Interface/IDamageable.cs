using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMessage
{
    public GameObject causer;
    public int damageAmount;
    public ItemData item;
    public DamageMessage(GameObject causer, int damageAmount, ItemData item = default)
    {
        this.causer = causer;
        this.damageAmount = damageAmount;
        this.item = item;
    }
}
public interface IDamageable
{
    public void TakeDamage(DamageMessage message)
    {
        /* virtual method */
    }
}
