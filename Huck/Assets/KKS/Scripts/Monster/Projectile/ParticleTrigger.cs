using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    private DamageMessage damageMessage = default;

    //! �������޽����� ��ü�� �޾ƿ� �Լ�
    public void InitDamageMessage(GameObject attacker, int damage)
    {
        damageMessage = new DamageMessage(attacker, damage);
    } // InitDamageMessage

    //! ��ƼŬ �浹 ó��
    private void OnParticleCollision(GameObject other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null && other.tag != GData.ENEMY_MASK)
        {
            damageable.TakeDamage(damageMessage);
        }
    } // OnParticleCollision
} // ParticleTrigger
