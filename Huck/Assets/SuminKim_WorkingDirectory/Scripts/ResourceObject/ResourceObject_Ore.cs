using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject_Ore : BaseResourceObject
{
    public override void TakeDamage(DamageMessage message)
    {
        // Ÿ�Կ� ���� �߰� ó��
        // ������ ���� Ÿ���� ����� ��� ������ ���� ó��
        // ������ ó��
        base.TakeDamage(message);
    }
}
