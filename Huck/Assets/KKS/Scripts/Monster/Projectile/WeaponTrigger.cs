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
        // Start ���� ���� �����ϱ� ������ ó�� Ȱ��ȭ �� null�ߴ� ���� ����ó��
        if (damageMessage != null || damageMessage != default)
        {
            // Ȱ��ȭ �� �� ����� ���� �������� ĳ�� (������ ����Ÿ�Կ� ���� ������ ���� ����)
            damageMessage.damageAmount = mController.monster.damage;
        }
    } // OnEnable

    //! �ݶ��̴� Ʈ����
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageMessage);
        }
    } // OnTriggerEnter
} // WeaponTrigger
