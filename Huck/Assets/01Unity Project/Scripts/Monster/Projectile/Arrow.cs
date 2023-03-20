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
        // 활성화될 때 초기화
        arrowRb.useGravity = false;
        isHit = false;
        arrowCollider.isTrigger = true;
        Vector3 targetPos = target.transform.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.forward = dir;
        arrowRb.AddForce(dir * 20f, ForceMode.VelocityChange);
        //Vector3 velocity = GetVelocity(transform.position, targetPos, 10f);
        //SetForce(velocity);
        StartCoroutine(EnqueueArrow());
    } // OnEnable

    private void FixedUpdate()
    {
        // 화살의 방향이 힘을 받는 방향으로 향하도록 하는 처리
        if (isHit == false && arrowRb.velocity != Vector3.zero)
        {
            transform.forward = arrowRb.velocity;
        }
    } // FixedUpdate

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Player")
            {
                Debug.Log($"플레이어 맞춤! {other.tag}");
            }
            isHit = true;
            arrowCollider.isTrigger = false;
            arrowRb.velocity = Vector3.zero;
            arrowRb.useGravity = true;
        }
    } // OnTriggerEnter

    private IEnumerator EnqueueArrow()
    {
        yield return new WaitForSeconds(5f);
        arrowRb.velocity = Vector3.zero;
        ArrowPool.instance.ReturnArrow(gameObject);
    }

    private void SetForce(Vector3 force)
    {
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    Vector3 GetVelocity(Vector3 currentPos, Vector3 targetPos, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(targetPos.x, 0, targetPos.z);
        Vector3 planarPosition = new Vector3(currentPos.x, 0, currentPos.z);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        float yOffset = currentPos.y - targetPos.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) *
            Mathf.Sqrt(
                (0.5f * gravity * Mathf.Pow(distance, 2)) /
                (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0f,
            initialVelocity * Mathf.Sin(angle),
            initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects = Vector3.Angle(Vector3.forward,
            planarTarget - planarPosition) * (targetPos.x > currentPos.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }
}
