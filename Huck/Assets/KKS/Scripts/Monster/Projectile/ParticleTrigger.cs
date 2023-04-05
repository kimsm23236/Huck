using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    private DamageMessage damageMessage = default;

    //! 데미지메시지의 주체를 받아올 함수
    public void InitDamageMessage(GameObject attacker, int damage)
    {
        damageMessage = new DamageMessage(attacker, damage);
    } // InitDamageMessage

    //! 파티클 충돌 처리
    private void OnParticleCollision(GameObject other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && other.tag != GData.ENEMY_MASK)
        {
            damageable.TakeDamage(damageMessage);
        }
    } // OnParticleCollision
} // ParticleTrigger
