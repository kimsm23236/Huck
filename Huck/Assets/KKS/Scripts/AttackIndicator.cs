using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private GameObject attackRangeObj = default;
    private float durationTime = default;

    public void InitAttackIndicator(float scale, float time)
    {
        transform.localScale = Vector3.one * scale;
        durationTime = time;
        StartCoroutine(StartIndicator());
    } // InitAttackIndicator

    // 공격범위 
    private IEnumerator StartIndicator()
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
        Destroy(gameObject);
    } // StartIndicator
}
