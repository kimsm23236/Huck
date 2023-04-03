using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPerception : MonoBehaviour
{
    // �÷��̾�� �浹 ������ ���̾ �����մϴ�.
    public LayerMask collidableLayer;

    // �浹�� ������Ʈ�� ������ ����Ʈ�Դϴ�.
    private List<GameObject> collidingObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� �÷��̾�� �浹 ������ ���̾ ���� �ִ��� Ȯ���մϴ�.
        if (collidableLayer == (collidableLayer | (1 << other.gameObject.layer)))
        {
            // �浹�� ������Ʈ�� ����Ʈ�� �߰��մϴ�.
            collidingObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // �浹�� ������Ʈ�� �÷��̾�� �浹 ������ ���̾ ���� �ִ��� Ȯ���մϴ�.
        if (collidableLayer == (collidableLayer | (1 << other.gameObject.layer)))
        {
            // �浹�� ������Ʈ�� ����Ʈ�� �߰��մϴ�.
            if (!collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �浹�� ������Ʈ�� ����Ʈ�� �ִ��� Ȯ���մϴ�.
        if (collidingObjects.Contains(other.gameObject))
        {
            // �浹�� ������Ʈ�� ����Ʈ���� �����մϴ�.
            collidingObjects.Remove(other.gameObject);
        }
    }

    // �浹�� ������Ʈ���� ��ȯ�ϴ� �Լ��Դϴ�.
    public List<GameObject> GetCollidingObjects()
    {
        return collidingObjects;
    }
}
