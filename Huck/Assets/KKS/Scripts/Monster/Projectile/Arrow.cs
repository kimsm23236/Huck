using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = default;
    private DamageMessage damageMessage = default;
    private Rigidbody arrowRb = default;
    private IEnumerator runningCoroutine = default; // 회수타이머 코루틴 저장 변수
    private bool isHit = false; // 충돌체크 변수

    private void OnEnable()
    {
        // 활설화시 초기화
        arrowRb = GetComponent<Rigidbody>();
        transform.parent = default;
        arrowRb.isKinematic = false;
        isHit = false;
        arrowRb.AddForce(transform.forward * speed, ForceMode.Impulse);
        // 충돌유무에 따라 회수코루틴을 종료하기 위해 캐싱
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
            // 충돌지점에 붙어있게 만드는 처리
            arrowRb.isKinematic = true;
            gameObject.transform.parent = other.transform;
            StopCoroutine(runningCoroutine);
            StartCoroutine(EnqueueArrow());
        }
    } // OnTriggerEnter

    //! 데미지메시지의 주체를 받아올 함수
    public void InitDamageMessage(GameObject attacker, int damage)
    {
        damageMessage = new DamageMessage(attacker, damage);
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
        arrowRb.velocity = Vector3.zero;
        ProjectilePool.Instance.EnqueueProjecttile(gameObject);
    } // EnqueueArrow
}
