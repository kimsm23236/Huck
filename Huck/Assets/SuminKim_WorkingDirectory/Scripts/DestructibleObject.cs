using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    // �ӽ�
    public int HP = 3;
    //[SerializeField]
    //private List<GameObject> childPObj;

    private Collider[] colliders;
    private Rigidbody[] rigidbodies;
    // Start is called before the first frame update
    void Start()
    {
        colliders = gameObject.GetComponentsInChildren<Collider>();
        rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        // Debug.Log($"Rigid Count : {rigidbodies.Length}");

        SetGravityAllRigidbody(false);
        SetTriggerAllCollider(true);

    }

    void Destruction()
    {
        SetGravityAllRigidbody(true);
        SetTriggerAllCollider(false);
        Destroy(gameObject, 6f);
    }

    public virtual void TakeDamage(DamageMessage damageMessage)
    {
        if(damageMessage.causer.tag == "AttackRange")
        {
            return;
        }

        Destruction();
    }

    private void SetGravityAllRigidbody(bool value)
    {
        foreach (var rigid in rigidbodies)
        {
            rigid.useGravity = value;
        }
    }
    private void SetTriggerAllCollider(bool value)
    {
        foreach(var collider in colliders)
        {
            if(collider.gameObject == gameObject)
                continue;
            collider.isTrigger = value;
        }
    }
}
