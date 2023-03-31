using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangIndicator : MonoBehaviour
{
    private AttackIndicator attackIndicator = default; // 공격범위 지시자 pool에 접근할 변수
    [SerializeField] private GameObject attackRangeObj = default; // 공격범위가 커지는 자식오브젝트
    private float durationTime = default; //최대지속시간

    // Start is called before the first frame update
    void Start()
    {
        attackIndicator = GFunc.GetRootObj("AttackIndicator").GetComponent<AttackIndicator>();
    } // Start

    //! 직사각형 범위 공격 지시자 범위설정 Init함수
    public void InitRectangIndicator(float horizontalityRange, float attackLength, float time)
    {
        // 매개변수로 입력받은 공격범위의 좌우폭 설정
        transform.localScale = new Vector3(horizontalityRange, 1f, 1f);
        durationTime = time;
        StartCoroutine(StartRectangIndicator(attackLength));
    } // InitRectangIndicator

    // 공격범위(직사각형 범위) 지속시간동안 커지는 코루틴함수
    private IEnumerator StartRectangIndicator(float attackLength)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < durationTime)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / durationTime;
            // 직사각형 공격지시자의 최대범위 길이를 시간에 따라 늘릴려면 Scale을 한쪽방향으로 늘려야하기 때문에
            // 앞방향의 Scale만 커지게 하기위해 Y축의 Scale을 사용 (X축의 회전값이 90이기 때문)
            // Scale크기 변경시 원점기준 양방향의 길이가 늘어나기때문에 늘어난 길이의 절반만큼 앞쪽으로 포지션을 이동시킴
            float yScale = Mathf.Lerp(0, attackLength, time);
            // y축을 기준으로 회전한 오브젝트의 앞방향을 구함 (오브젝트의 forward방향으로 이동하기 때문에 회전시 각도처리, 대각선등 의도치않은 이동 방지)
            Vector3 dir = Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.forward;
            transform.position = startPos + (dir * (((startPos.z + yScale) - startPos.z) * 0.5f));
            transform.localScale = new Vector3(transform.localScale.x, yScale, 1);

            Vector3 scale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            attackRangeObj.transform.localScale = scale;
            yield return null;
        }
        gameObject.SetActive(false);
        attackIndicator.EnqueueRectangIndicator(gameObject);
    } // StartCircleIndicator
} // RectangIndicator
