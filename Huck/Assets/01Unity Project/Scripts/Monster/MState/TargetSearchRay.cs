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

    // Update is called once per frame
    void Update()
    {
        SearchTarget();
    } // Update

    //! �������� �ȿ� Ÿ���� ã�� �Լ�
    private void SearchTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, mController.monster.searchRange, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            foreach (var _hit in hits)
            {
                hit = _hit;
                //Debug.Log($"{hit.tag}, {hit.name} ã��");
            }
        }
    } // TagetCheckRay

    ////! ������ ���������� Gizmo�� �����ִ� �Լ�
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if (mController != null || mController != default)
    //    {
    //        Gizmos.DrawSphere(transform.position, mController.monster.searchRange);
    //    }
    //} // OnDrawGizmos
}