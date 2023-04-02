using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = default;
    private ProjectilePool arrowPool = default;
    private DamageMessage damageMessage = default;
    private Rigidbody arrowRb = default;
    private IEnumerator runningCoroutine = default; // 회수타이머 코루틴 저장 변수
    private bool isHit = false; // 충돌체크 변수

    private void OnEnable()
    {
        // 활성화시 초기화
        gameObject.transform.parent = null;
        arrowRb = GetComponent<Rigidbody>();
        arrowRb.AddForce(transform.forward * speed, ForceMode.Impulse);
        // 충돌유무에 따라 회수코루틴을 종료하기 위해 캐싱
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
            // 충돌지점에 붙어있게 만드는 처리
            arrowRb.isKinematic = true;
            gameObject.transform.parent = other.transform;
            StopCoroutine(runningCoroutine);
            StartCoroutine(EnqueueArrow());
        }
    } // OnTriggerEnter

    //! 데미지메시지의 주체를 받아올 함수
    public void InitDamageMessage(ProjectilePool _arrowPool, GameObject attacker, int damage)
    {
        damageMessage = new DamageMessage(attacker, damage);
        arrowPool = _arrowPool;
    } // InitDamageMessage

    //! 발사한 화살 회수하는 함수
    private IEnumerator EnqueueArrow()
    {
        if (isHit == false)
        {
            yield return new WaitForSeconds(5f);
        }
        else
        {
            // 화살이 충돌하면 2초뒤에 회수
            yield return new WaitForSeconds(2f);
        }
        // pool이 없으면 파괴
        if (damageMessage.causer == null || damageMessage.causer == default)
        {
            Destroy(gameObject);
        }
        arrowPool.EnqueueProjecttile(gameObject);
    } // EnqueueArrow
}
