using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMove : MonoBehaviour
{
    public GameObject camera_1p = default;
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.position = camera_1p.transform.position;
        gameObject.transform.rotation = camera_1p.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("뭔가 충돌");
        Debug.Log($"{other.name} 충돌");
        IDamageable damagePrc = other.GetComponent<IDamageable>();
        
        if (damagePrc != null)
        {
            Debug.Log("데미지 메시지 생성");
            DamageMessage dm = new DamageMessage(gameObject, 10f);
            damagePrc.TakeDamage(dm);
        }
    }
}
