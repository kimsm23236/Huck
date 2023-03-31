using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuildObject : BaseResourceObject
{
    [SerializeField]
    private BuildSystem Bsystem;

    private void Awake()
    {
        Bsystem = GameManager.Instance.buildSystem;
    }

    public override void TakeDamage(DamageMessage message)
    {
        base.TakeDamage(message);
    }

    protected override void Die()
    {
        Bsystem.FindAndDestory(gameObject.transform.parent.name);
    }
}
