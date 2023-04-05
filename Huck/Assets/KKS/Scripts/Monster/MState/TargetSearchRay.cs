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

    //! �������� �ȿ� Ÿ���� ã�� �Լ�
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
                else // �������� �ȿ� �÷��̾ ���� �� �÷��̾��� ���๰�� Ÿ��������
                {
                    // SqrMagnitude�� Ÿ�ٰ��� �Ÿ� ���ϱ� (SqrMagnitude�� ��Ʈ������ �����ʰ� ���� �Ÿ��� �������� �����ϹǷ� ����ӵ��� ������ ������)
                    // Ÿ�ٰ��� ��Ȯ�� �Ÿ��� ���� �ܼ��� �Ÿ��񱳸� �ϱ����� SqrMagnitude ���
                    float distance = Vector3.SqrMagnitude(mController.transform.position - _hit.transform.position);
                    // ���� ����� Ÿ���� ã�� ó��
                    if (nearTargetDistance > distance)
                    {
                        nearTargetDistance = distance;
                        nearTarget = _hit;
                    }
                }
            } // foreach
            // ���� ����� Ÿ���� hit�� ĳ�� (Ÿ���� �÷��̾ �ƴ� ���)
            hit = nearTarget;
        }
        else
        {
            // Ÿ���� ������ null
            hit = null;
        }
    } // TagetCheckRay

    //! ������ ���������� Gizmo�� �����ִ� �Լ�
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
