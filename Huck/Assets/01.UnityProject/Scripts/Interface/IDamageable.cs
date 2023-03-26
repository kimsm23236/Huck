using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMessage
{
    public GameObject causer;
    public float damageAmount;
    public Item item;
    public DamageMessage(GameObject causer, float damageAmount, Item item = default)
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
