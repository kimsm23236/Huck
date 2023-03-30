using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = default;
    private DamageMessage damageMessage = default;
    private Rigidbody arrowRb = default;
    private IEnumerator runningCoroutine = default; // ȸ��Ÿ�̸� �ڷ�ƾ ���� ����
    private bool isHit = false; // �浹üũ ����

    private void OnEnable()
    {
        // Ȱ��ȭ�� �ʱ�ȭ
        arrowRb = GetComponent<Rigidbody>();
        transform.parent = default;
        arrowRb.isKinematic = false;
        isHit = false;
        arrowRb.AddForce(transform.forward * speed, ForceMode.Impulse);
        // �浹������ ���� ȸ���ڷ�ƾ�� �����ϱ� ���� ĳ��
        runningCoroutine = EnqueueArrow();
        StartCoroutine(runningCoroutine);
    } // OnEnable

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != GData.ENEMY_MASK)
        {
            isHit = true;
            if (other.tag == GData.PLAYER_MASK || other.tag == GData.BUILD_MASK)
            {
                other.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
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
    public void InitDamageMessage(GameObject attacker, int damage)
    {
        damageMessage = new DamageMessage(attacker, damage);
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
        arrowRb.velocity = Vector3.zero;
        ProjectilePool.Instance.EnqueueProjecttile(gameObject);
    } // EnqueueArrow
}
