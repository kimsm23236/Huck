using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangIndicator : MonoBehaviour
{
    private AttackIndicator attackIndicator = default; // ���ݹ��� ������ pool�� ������ ����
    [SerializeField] private GameObject attackRangeObj = default; // ���ݹ����� Ŀ���� �ڽĿ�����Ʈ
    private bool isAttackerDead = false;
    private float durationTime = default; //�ִ����ӽð�

    // Start is called before the first frame update
    void Start()
    {
        attackIndicator = GFunc.GetRootObj("AttackIndicator").GetComponent<AttackIndicator>();
    } // Start

    //! ���簢�� ���� ���� ������ �������� Init�Լ�
    public void InitRectangIndicator(bool _isAttackerDead, float horizontalityRange, float attackLength, float time)
    {
        isAttackerDead = _isAttackerDead;
        // �Ű������� �Է¹��� ���ݹ����� �¿��� ����
        transform.localScale = new Vector3(horizontalityRange, 1f, 1f);
        durationTime = time;
        StartCoroutine(StartRectangIndicator(attackLength));
    } // InitRectangIndicator

    // ���ݹ���(���簢�� ����) ���ӽð����� Ŀ���� �ڷ�ƾ�Լ�
    private IEnumerator StartRectangIndicator(float attackLength)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < durationTime)
        {
            elapsed += Time.deltaTime;
            if (isAttackerDead == true)
            {
                // �����ڰ� ������ ��Ȱ��ȭ
                gameObject.SetActive(false);
                yield break;
            }
            float time = elapsed / durationTime;
            // ���簢�� ������������ �ִ���� ���̸� �ð��� ���� �ø����� Scale�� ���ʹ������� �÷����ϱ� ������
            // �չ����� Scale�� Ŀ���� �ϱ����� Y���� Scale�� ��� (X���� ȸ������ 90�̱� ����)
            // Scaleũ�� ����� �������� ������� ���̰� �þ�⶧���� �þ ������ ���ݸ�ŭ �������� �������� �̵���Ŵ
            float yScale = Mathf.Lerp(0, attackLength, time);
            // y���� �������� ȸ���� ������Ʈ�� �չ����� ���� (������Ʈ�� forward�������� �̵��ϱ� ������ ȸ���� ����ó��, �밢���� �ǵ�ġ���� �̵� ����)
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
