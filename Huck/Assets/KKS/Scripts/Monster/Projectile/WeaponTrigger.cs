using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    [SerializeField] private MonsterController mController = default;
    private DamageMessage damageMessage = default;

    private void Start()
    {
        damageMessage = new DamageMessage(mController.gameObject, mController.monster.damage);
    } // Start

    private void OnEnable()
    {
        // Start 보다 먼저 실행하기 때문에 처음 활성화 시 null뜨는 오류 예외처리
        if (damageMessage != null || damageMessage != default)
        {
            // 활성화 될 때 변경된 몬스터 데미지를 캐싱 (몬스터의 공격타입에 따른 데미지 변경 적용)
            damageMessage.damageAmount = mController.monster.damage;
        }
    } // OnEnable

    //! 콜라이더 트리거
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GData.PLAYER_MASK || other.tag == GData.BUILD_MASK)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
        }
    } // OnTriggerEnter
} // WeaponTrigger
