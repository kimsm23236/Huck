using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Parabola
{
    //! ������Ʈ�� ���� �������� ��ǥ�������� totalTime �ð���ŭ ������ �̵��ϴ� �Լ�
    public IEnumerator ParabolaMoveToTarget(Vector3 startPos, Vector3 targetPos, float totalTime, GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        // �߷� ���ӵ�
        float gravity = Physics.gravity.magnitude;
        // ���� ��ġ�� ��ǥ ��ġ�� �Ÿ��� xz��ǥ�� y��ǥ�� ���� ó���ϱ� ���� �и� 
        Vector3 toTarget = targetPos - startPos;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0f;
        float y = targetPos.y - startPos.y;

        // ���� ���� �ӵ�
        float velocityY = (y + 0.5f * gravity * totalTime * totalTime) / totalTime;
        // ���� ���� �ӵ�
        Vector3 velocityXZ = toTargetXZ / totalTime;
        // ����ð� ����
        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            // �ð��� �帧�� ���� ��ġ�� ���
            float posY = startPos.y + velocityY * elapsedTime - 0.5f * gravity * elapsedTime * elapsedTime;
            float posX = startPos.x + velocityXZ.x * elapsedTime;
            float posZ = startPos.z + velocityXZ.z * elapsedTime;
            // elapsedTime �ð��� ���� ��ġ�� ������Ʈ �̵�
            obj.transform.position = new Vector3(posX, posY, posZ);
            yield return null;
        }
    } // ParabolaMoveToTarget
} // Parabola

