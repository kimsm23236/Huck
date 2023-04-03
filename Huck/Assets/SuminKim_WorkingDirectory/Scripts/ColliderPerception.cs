using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPerception : MonoBehaviour
{
    // 플레이어와 충돌 가능한 레이어를 설정합니다.
    public LayerMask collidableLayer;

    // 충돌한 오브젝트를 저장할 리스트입니다.
    private List<GameObject> collidingObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 플레이어와 충돌 가능한 레이어에 속해 있는지 확인합니다.
        if (collidableLayer == (collidableLayer | (1 << other.gameObject.layer)))
        {
            // 충돌한 오브젝트를 리스트에 추가합니다.
            collidingObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 충돌한 오브젝트가 플레이어와 충돌 가능한 레이어에 속해 있는지 확인합니다.
        if (collidableLayer == (collidableLayer | (1 << other.gameObject.layer)))
        {
            // 충돌한 오브젝트를 리스트에 추가합니다.
            if (!collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 충돌한 오브젝트가 리스트에 있는지 확인합니다.
        if (collidingObjects.Contains(other.gameObject))
        {
            // 충돌한 오브젝트를 리스트에서 제거합니다.
            collidingObjects.Remove(other.gameObject);
        }
    }

    // 충돌한 오브젝트들을 반환하는 함수입니다.
    public List<GameObject> GetCollidingObjects()
    {
        return collidingObjects;
    }
}
