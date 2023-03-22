using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody arrowRb = default;
    private CapsuleCollider arrowCollider = default;
    private MonsterController mController = default;
    private GameObject target = default;
    private bool isHit = false;
    private float gravityScale = 2f;
    void Awake()
    {
        arrowRb = gameObject.GetComponent<Rigidbody>();
        arrowCollider = gameObject.GetComponent<CapsuleCollider>();
        mController = transform.parent.gameObject.GetComponent<MonsterController>();
        target = mController.targetSearch.hit.gameObject;
    } // Awake

    private void OnEnable()
    {
        // 부모오브젝트의 포지션을 따라가는걸 방지하기 위한 처리
        transform.parent = default;
        // 직접 설정한 중력을 적용하기 위해 기본중력 false
        arrowRb.useGravity = false;
        arrowRb.velocity = Vector3.zero;
        isHit = false;
        arrowCollider.isTrigger = true;
        arrowRb.AddForce(transform.forward * 20f, ForceMode.VelocityChange);
        StartCoroutine(EnqueueArrow());
    } // OnEnable

    private void FixedUpdate()
    {
        // 날아가는 중에는 직접 설정한 중력 적용
        if (arrowRb.useGravity == false)
        {
            arrowRb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
        }
        // 화살의 방향이 힘을 받는 방향으로 향하도록 하는 처리
        if (isHit == false && transform.forward != Vector3.zero)
        {
            transform.forward = arrowRb.velocity;
        }
    } // FixedUpdate

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.tag == GData.PLAYER_MASK || other.tag == GData.BUILD_MASK)
            {
                Debug.Log($"플레이어 맞춤! {other.tag}");
            }
            isHit = true;
            arrowCollider.isTrigger = false;
            arrowRb.velocity = Vector3.zero;
            arrowRb.useGravity = true;
        }
    } // OnTriggerEnter

    //! 발사한 화살 회수하는 함수
    private IEnumerator EnqueueArrow()
    {
        yield return new WaitForSeconds(5f);
        ArrowPool.Instance.ReturnArrow(gameObject);
    } // EnqueueArrow

    //private void SetForce(Vector3 force)
    //{
    //    GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    //}

    //Vector3 GetVelocity(Vector3 currentPos, Vector3 targetPos, float initialAngle)
    //{
    //    float gravity = Physics.gravity.magnitude;
    //    float angle = initialAngle * Mathf.Deg2Rad;

    //    Vector3 planarTarget = new Vector3(targetPos.x, 0, targetPos.z);
    //    Vector3 planarPosition = new Vector3(currentPos.x, 0, currentPos.z);

    //    float distance = Vector3.Distance(planarTarget, planarPosition);
    //    float yOffset = currentPos.y - targetPos.y;

    //    float initialVelocity = (1 / Mathf.Cos(angle)) *
    //        Mathf.Sqrt(
    //            (0.5f * gravity * Mathf.Pow(distance, 2)) /
    //            (distance * Mathf.Tan(angle) + yOffset));

    //    Vector3 velocity = new Vector3(0f,
    //        initialVelocity * Mathf.Sin(angle),
    //        initialVelocity * Mathf.Cos(angle));

    //    float angleBetweenObjects = Vector3.Angle(Vector3.forward,
    //        planarTarget - planarPosition) * (targetPos.x > currentPos.x ? 1 : -1);
    //    Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

    //    return finalVelocity;
    //}
}
