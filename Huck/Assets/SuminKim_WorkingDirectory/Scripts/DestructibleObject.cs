using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    // юс╫ц
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
        Debug.Log($"Collider Count : {colliders.Length}");
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
        HP--;
        Debug.Log("Pillar Take Damage");
        if(HP <= 0)
        {
            Destruction();
        }
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
            collider.isTrigger = value;
            
        }
    }
}
