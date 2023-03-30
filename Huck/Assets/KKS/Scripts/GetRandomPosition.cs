using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRandomPosition
{
    //! 원범위 안에서 랜덤한 좌표 가져오는 함수
    public Vector3 GetRandomCirclePos(Vector3 center, int radius, int minDistance)
    {
        // 360도 범위에서 랜덤 각도 구하기
        float angle = Random.Range(0f, Mathf.PI * 2);
        // 거리 2 ~ 반지름길이에서 랜덤 거리 구하기
        float distance = Random.Range(minDistance, radius + 1);
        // 삼각함수 이용해서 x, z 좌표에 구한 거리 곱하기 각도 더해줌
        float x = center.x + distance * Mathf.Cos(angle);
        float z = center.z + distance * Mathf.Sin(angle);
        RaycastHit hit = default;
        Vector3 pos = center + Vector3.up * 5f;
        // 랜덤으로 구한 좌표 위에서 아래로 레이케스트쏴서 땅인지 체크
        if (Physics.Raycast(pos, Vector3.down, out hit, 15f, LayerMask.GetMask(GData.TERRAIN_MASK)) == true)
        {
            // 충돌한 땅의 높이를 좌표에 넣음 (좌표를 사용할 이펙트의 위치를 0.1f 높게 처리)
            center.y = hit.point.y + 0.1f;
        }
        else
        {
            // 구한 랜덤좌표에 장애물이 있으면 좌표 다시 구함 (재귀함수)
            GetRandomCirclePos(center, radius, minDistance);
        }
        return new Vector3(x, center.y, z);
    } // GetRandomPos
}
