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
        if (other.tag == GData.PLAYER_MASK || other.tag == GData.BUILD_MASK)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
            Debug.Log($"파티클 충돌! {damageMessage.causer.name}이 {other.tag}에게 {damageMessage.damageAmount}피해 줌!");
        }
    } // OnParticleCollision
} // ParticleTrigger
