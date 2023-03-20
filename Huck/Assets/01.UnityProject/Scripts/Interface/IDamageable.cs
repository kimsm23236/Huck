using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageMessage
{
    public GameObject causer;
    public float damageAmount;

    // �� �ʿ��Ҹ��Ѱ� �˾Ƽ� �߰��ϱ�
}
public interface IDamageable
{
    public virtual void TakeDamage(DamageMessage message)
    {
        /* virtual method */
    }
}
