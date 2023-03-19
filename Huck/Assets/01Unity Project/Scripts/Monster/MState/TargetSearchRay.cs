using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class TargetSearchRay : MonoBehaviour
{
    private MonsterController mController;
    public Collider hit;

    // Start is called before the first frame update
    void Start()
    {
        mController = gameObject.GetComponent<MonsterController>();
    } // Start

    //! �������� �ȿ� Ÿ���� ã�� �Լ�
    public void SearchTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, mController.monster.searchRange, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            hit = hits[0];
        }
    } // TagetCheckRay

    //! ������ ���������� Gizmo�� �����ִ� �Լ�
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (mController != null || mController != default)
        {
            Gizmos.DrawWireSphere(transform.position, mController.monster.searchRange);
        }
    } // OnDrawGizmos
} // TargetSearchRay
