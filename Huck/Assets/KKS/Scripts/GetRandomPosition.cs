using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetRandomPosition
{
    private int maxLoopCount = 50;
    //! 원범위 안에서 랜덤한 좌표 가져오는 함수
    public Vector3 GetRandomCirclePos(Vector3 center, int radius, int minDistance, int loopCount = 0)
    {
        while (true)
        {
            if (loopCount >= 100)
            {
                return center;
            }
            // 360도 범위에서 랜덤 각도 구하기
            float angle = Random.Range(0f, Mathf.PI * 2);
            // 거리 minDistance ~ 반지름 길이에서 랜덤 거리 구하기
            float distance = Random.Range(minDistance, radius + 1);
            // 삼각함수 이용해서 x, z 좌표에 구한 거리 곱하기 각도 더해줌
            float x = center.x + distance * Mathf.Cos(angle);
            float y = center.y;
            float z = center.z + distance * Mathf.Sin(angle);

            Vector3 navMeshPos = new Vector3(x, y, z);
            NavMeshHit hit = default;
            // 구한 랜덤좌표와 가장 가까운 네비매쉬 위치를 return
            if (NavMesh.SamplePosition(navMeshPos, out hit, 1f, NavMesh.AllAreas) == true)
            {
                return hit.position;
            }
            loopCount++;
        }
    } // GetRandomPos
} // GetRandomPosition
