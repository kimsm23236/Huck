using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = default;
    private ProjectilePool arrowPool = default;
    private DamageMessage damageMessage = default;
    private Rigidbody arrowRb = default;
    private IEnumerator runningCoroutine = default; // ȸ��Ÿ�̸� �ڷ�ƾ ���� ����
    private bool isHit = false; // �浹üũ ����

    private void OnEnable()
    {
        // Ȱ��ȭ�� �ʱ�ȭ
        gameObject.transform.parent = null;
        arrowRb = GetComponent<Rigidbody>();
        arrowRb.AddForce(transform.forward * speed, ForceMode.Impulse);
        // �浹������ ���� ȸ���ڷ�ƾ�� �����ϱ� ���� ĳ��
        runningCoroutine = EnqueueArrow();
        StartCoroutine(runningCoroutine);
    } // OnEnable

    private void OnDisable()
    {
        arrowRb.velocity = Vector3.zero;
        arrowRb.isKinematic = false;
        isHit = false;
    } // OnDisable

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != GData.ENEMY_MASK && other.tag != "AttackRange")
        {
            isHit = true;
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageMessage);
            }
            arrowRb.velocity = Vector3.zero;
            // �浹������ �پ��ְ� ����� ó��
            arrowRb.isKinematic = true;
            gameObject.transform.parent = other.transform;
            StopCoroutine(runningCoroutine);
            StartCoroutine(EnqueueArrow());
        }
    } // OnTriggerEnter

    //! �������޽����� ��ü�� �޾ƿ� �Լ�
    public void InitDamageMessage(ProjectilePool _arrowPool, GameObject attacker, int damage)
    {
        damageMessage = new DamageMessage(attacker, damage);
        arrowPool = _arrowPool;
    } // InitDamageMessage

    //! �߻��� ȭ�� ȸ���ϴ� �Լ�
    private IEnumerator EnqueueArrow()
    {
        if (isHit == false)
        {
            yield return new WaitForSeconds(5f);
        }
        else
        {
            // ȭ���� �浹�ϸ� 2�ʵڿ� ȸ��
            yield return new WaitForSeconds(2f);
        }
        // pool�� ������ �ı�
        if (damageMessage.causer == null || damageMessage.causer == default)
        {
            Destroy(gameObject);
        }
        arrowPool.EnqueueProjecttile(gameObject);
    } // EnqueueArrow
}
