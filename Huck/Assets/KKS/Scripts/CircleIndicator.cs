using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleIndicator : MonoBehaviour
{
    private AttackIndicator attackIndicator = default; // ���ݹ��� ������ pool�� ������ ����
    [SerializeField] private GameObject attackRangeObj = default; // ���ݹ����� Ŀ���� �ڽĿ�����Ʈ
    private float durationTime = default; // �ִ����ӽð�

    private void Start()
    {
        attackIndicator = GFunc.GetRootObj("AttackIndicator").GetComponent<AttackIndicator>();
    } // Start

    //! �� ���� ���� ������ �������� Init�Լ�
    public void InitCircleIndicator(float scale, float time)
    {
        transform.localScale = Vector3.one * scale;
        durationTime = time;
        StartCoroutine(StartCircleIndicator());
    } // InitCircleIndicator

    // ���ݹ���(�� ����) ���ӽð����� Ŀ���� �ڷ�ƾ�Լ�
    private IEnumerator StartCircleIndicator()
    {
        float elapsed = 0f;
        while (elapsed < durationTime)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / durationTime;
            Vector3 scale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            attackRangeObj.transform.localScale = scale;
            yield return null;
        }
        gameObject.SetActive(false);
        attackIndicator.EnqueueCircleIndicator(gameObject);
    } // StartCircleIndicator
} // CircleIndicator
