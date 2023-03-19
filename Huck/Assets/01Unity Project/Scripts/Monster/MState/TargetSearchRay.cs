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

    //! 감지범위 안에 타겟을 찾는 함수
    public void SearchTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, mController.monster.searchRange, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            hit = hits[0];
        }
    } // TagetCheckRay

    //! 몬스터의 감지범위를 Gizmo로 보여주는 함수
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (mController != null || mController != default)
        {
            Gizmos.DrawWireSphere(transform.position, mController.monster.searchRange);
        }
    } // OnDrawGizmos
} // TargetSearchRay
