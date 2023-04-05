using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSearchRay : MonoBehaviour
{
    private MonsterController mController = default;
    public Collider hit = default;

    // Start is called before the first frame update
    void Start()
    {
        mController = gameObject.GetComponent<MonsterController>();
    } // Start

    //! 감지범위 안에 타겟을 찾는 함수
    public void SearchTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, mController.monster.searchRange, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
        Collider nearTarget = default;
        if (hits.Length > 0)
        {
            float nearTargetDistance = Mathf.Infinity;
            foreach (var _hit in hits)
            {
                if (_hit.tag == GData.PLAYER_MASK)
                {
                    hit = _hit;
                    return;
                }
                else // 감지범위 안에 플레이어가 없을 때 플레이어의 건축물을 타겟으로함
                {
                    // SqrMagnitude로 타겟과의 거리 구하기 (SqrMagnitude는 루트연산을 하지않고 계산된 거리의 제곱값을 리턴하므로 연산속도가 빠르고 가벼움)
                    // 타겟과의 정확한 거리비교 보단 단순한 거리비교를 하기위해 SqrMagnitude 사용
                    float distance = Vector3.SqrMagnitude(mController.transform.position - _hit.transform.position);
                    // 가장 가까운 타겟을 찾는 처리
                    if (nearTargetDistance > distance)
                    {
                        nearTargetDistance = distance;
                        nearTarget = _hit;
                    }
                }
            } // foreach
            // 가장 가까운 타겟을 hit에 캐싱 (타겟이 플레이어가 아닐 경우)
            hit = nearTarget;
        }
        else
        {
            // 타겟이 없으면 null
            hit = null;
        }
    } // TagetCheckRay

    //! 몬스터의 감지범위를 Gizmo로 보여주는 함수
    private void OnDrawGizmos()
    {
        if (hit != null)
        {
            Gizmos.color = Color.red;
            if (mController != null && mController != default)
            {
                Gizmos.DrawWireSphere(transform.position, mController.monster.searchRange);
            }
        }
    } // OnDrawGizmos
} // TargetSearchRay
