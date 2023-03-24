using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("점프시작");
            //StartCoroutine(MoveToTargetCoroutine(transform.position, target.transform.position, 1.5f));
            float distance = 1.0f;

            // 타겟의 현재 위치와 바라보는 방향 벡터
            Vector3 targetPosition = target.transform.position;
            Vector3 targetDirection = -(target.transform.position - transform.position).normalized;

            // 타겟 앞의 위치 구하기
            Vector3 targetFrontPosition = targetPosition + targetDirection * distance;
            Parabola a = new Parabola();
            StartCoroutine(a.ParabolaMoveToTarget(transform.position, targetFrontPosition, 2f, gameObject));
        }
    }

    public IEnumerator MoveToTarget(Vector3 start, Vector3 target, float totalTime)
    {
        // 중력 가속도
        float gravity = Physics.gravity.magnitude;

        // 시작 위치와 목표 위치의 거리
        float distance = Vector3.Distance(start, target);

        // 수직 방향 초기 속도
        float initialVerticalSpeed = (distance / totalTime + 0.5f * gravity * totalTime) / totalTime;

        // 수평 방향 초기 속도
        Vector3 initialHorizontalVelocity = (target - start) / totalTime;

        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            // 시간이 흐름에 따라 위치를 계산
            float verticalPosition = start.y + initialVerticalSpeed * elapsedTime - 0.5f * gravity * elapsedTime * elapsedTime;
            float horizontalPosition = start.x + initialHorizontalVelocity.x * elapsedTime;
            float depthPosition = start.z + initialHorizontalVelocity.z * elapsedTime;

            // 위치 업데이트
            transform.position = new Vector3(horizontalPosition, verticalPosition, depthPosition);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveToTargetCoroutine(Vector3 start, Vector3 target, float totalTime)
    {
        // 중력 가속도
        float gravity = Physics.gravity.magnitude;

        // 시작 위치와 목표 위치의 거리
        float distance = Vector3.Distance(start, target);
        Vector3 toTarget = target - start;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0f;
        float y = target.y - start.y;
        float xzDistance = toTargetXZ.magnitude;

        // 수직 방향 초기 속도
        //float initialVerticalSpeed = (y / totalTime + 0.5f * gravity * totalTime) / totalTime;
        float initialVerticalSpeed = (y + 0.5f * gravity * totalTime * totalTime) / totalTime;

        // 수평 방향 초기 속도
        Vector3 initialHorizontalVelocity = toTargetXZ / totalTime;

        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            // 시간이 흐름에 따라 위치를 계산
            float verticalPosition = start.y + initialVerticalSpeed * elapsedTime - 0.5f * gravity * elapsedTime * elapsedTime;
            float horizontalPosition = start.x + initialHorizontalVelocity.x * elapsedTime;
            float depthPosition = start.z + initialHorizontalVelocity.z * elapsedTime;

            // 위치 업데이트
            transform.position = new Vector3(horizontalPosition, verticalPosition, depthPosition);

            yield return null;
        }
    }
}
