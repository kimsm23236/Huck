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
        if (other.tag == GData.PLAYER_MASK || other.tag == GData.BUILD_MASK)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
            Debug.Log($"��ƼŬ �浹! {damageMessage.causer.name}�� {other.tag}���� {damageMessage.damageAmount}���� ��!");
        }
    } // OnParticleCollision
} // ParticleTrigger
