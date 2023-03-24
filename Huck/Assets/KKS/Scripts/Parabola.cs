using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Parabola
{
    //! 오브젝트를 시작 지점에서 목표지점까지 totalTime 시간만큼 포물선 이동하는 함수
    public IEnumerator ParabolaMoveToTarget(Vector3 startPos, Vector3 targetPos, float totalTime, GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        // 중력 가속도
        float gravity = Physics.gravity.magnitude;
        // 시작 위치와 목표 위치의 거리의 xz좌표와 y좌표를 따로 처리하기 위해 분리 
        Vector3 toTarget = targetPos - startPos;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0f;
        float y = targetPos.y - startPos.y;

        // 수직 방향 속도
        float velocityY = (y + 0.5f * gravity * totalTime * totalTime) / totalTime;
        // 수평 방향 속도
        Vector3 velocityXZ = toTargetXZ / totalTime;
        // 경과시간 변수
        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            // 시간이 흐름에 따라 위치를 계산
            float posY = startPos.y + velocityY * elapsedTime - 0.5f * gravity * elapsedTime * elapsedTime;
            float posX = startPos.x + velocityXZ.x * elapsedTime;
            float posZ = startPos.z + velocityXZ.z * elapsedTime;
            // elapsedTime 시간일 때의 위치로 오브젝트 이동
            obj.transform.position = new Vector3(posX, posY, posZ);
            yield return null;
        }
    } // ParabolaMoveToTarget
} // Parabola

