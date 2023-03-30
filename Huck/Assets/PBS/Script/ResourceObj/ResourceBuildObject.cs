using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuildObject : BaseResourceObject
{
    private BuildSystem Bsystem;

    private void Awake()
    {
        Bsystem = GFunc.GetRootObj("BuildSystem").GetComponent<BuildSystem>();
    }

    public override void TakeDamage(DamageMessage message)
    {

        // Ÿ�Կ� ���� �߰� ó��
        // ������ ���� Ÿ���� ����� ��� ������ ���� ó��
        // ������ ó��
        base.TakeDamage(message);
    }

    protected override void Die()
    {
        Bsystem.FindAndDestory(gameObject.transform.parent.name);
    }
}
