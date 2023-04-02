using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetRandomPosition
{
    //! ������ �ȿ��� ������ ��ǥ �������� �Լ�
    public Vector3 GetRandomCirclePos(Vector3 center, int radius, int minDistance)
    {
        // 360�� �������� ���� ���� ���ϱ�
        float angle = Random.Range(0f, Mathf.PI * 2);
        // �Ÿ� minDistance ~ ������ ���̿��� ���� �Ÿ� ���ϱ�
        float distance = Random.Range(minDistance, radius + 1);
        // �ﰢ�Լ� �̿��ؼ� x, z ��ǥ�� ���� �Ÿ� ���ϱ� ���� ������
        float x = center.x + distance * Mathf.Cos(angle);
        float y = center.y;
        float z = center.z + distance * Mathf.Sin(angle);

        Vector3 navMeshPos = new Vector3(x, y, z);
        NavMeshHit hit = default;
        // ���� ������ǥ�� ���� ����� �׺�Ž� ��ġ�� return
        if(NavMesh.SamplePosition(navMeshPos, out hit, 1f, NavMesh.AllAreas) == true)
        {
            Debug.DrawRay(hit.position, Vector3.up, Color.blue);
            return hit.position;
        }
        else
        {
            return GetRandomCirclePos(center, radius, minDistance);
        }
    } // GetRandomPos
} // GetRandomPosition
