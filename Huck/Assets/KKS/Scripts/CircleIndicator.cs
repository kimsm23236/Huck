using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleIndicator : MonoBehaviour
{
    private AttackIndicator attackIndicator = default; // 공격범위 지시자 pool에 접근할 변수
    [SerializeField] private GameObject attackRangeObj = default; // 공격범위가 커지는 자식오브젝트
    private float durationTime = default; // 최대지속시간

    private void Start()
    {
        attackIndicator = GFunc.GetRootObj("AttackIndicator").GetComponent<AttackIndicator>();
    } // Start

    //! 원 범위 공격 지시자 범위설정 Init함수
    public void InitCircleIndicator(float scale, float time)
    {
        transform.localScale = Vector3.one * scale;
        durationTime = time;
        StartCoroutine(StartCircleIndicator());
    } // InitCircleIndicator

    // 공격범위(원 범위) 지속시간동안 커지는 코루틴함수
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
