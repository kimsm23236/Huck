using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("��������");
            //StartCoroutine(MoveToTargetCoroutine(transform.position, target.transform.position, 1.5f));
            float distance = 1.0f;

            // Ÿ���� ���� ��ġ�� �ٶ󺸴� ���� ����
            Vector3 targetPosition = target.transform.position;
            Vector3 targetDirection = -(target.transform.position - transform.position).normalized;

            // Ÿ�� ���� ��ġ ���ϱ�
            Vector3 targetFrontPosition = targetPosition + targetDirection * distance;
            Parabola a = new Parabola();
            StartCoroutine(a.ParabolaMoveToTarget(transform.position, targetFrontPosition, 2f, gameObject));
        }
    }

    public IEnumerator MoveToTarget(Vector3 start, Vector3 target, float totalTime)
    {
        // �߷� ���ӵ�
        float gravity = Physics.gravity.magnitude;

        // ���� ��ġ�� ��ǥ ��ġ�� �Ÿ�
        float distance = Vector3.Distance(start, target);

        // ���� ���� �ʱ� �ӵ�
        float initialVerticalSpeed = (distance / totalTime + 0.5f * gravity * totalTime) / totalTime;

        // ���� ���� �ʱ� �ӵ�
        Vector3 initialHorizontalVelocity = (target - start) / totalTime;

        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            // �ð��� �帧�� ���� ��ġ�� ���
            float verticalPosition = start.y + initialVerticalSpeed * elapsedTime - 0.5f * gravity * elapsedTime * elapsedTime;
            float horizontalPosition = start.x + initialHorizontalVelocity.x * elapsedTime;
            float depthPosition = start.z + initialHorizontalVelocity.z * elapsedTime;

            // ��ġ ������Ʈ
            transform.position = new Vector3(horizontalPosition, verticalPosition, depthPosition);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveToTargetCoroutine(Vector3 start, Vector3 target, float totalTime)
    {
        // �߷� ���ӵ�
        float gravity = Physics.gravity.magnitude;

        // ���� ��ġ�� ��ǥ ��ġ�� �Ÿ�
        float distance = Vector3.Distance(start, target);
        Vector3 toTarget = target - start;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0f;
        float y = target.y - start.y;
        float xzDistance = toTargetXZ.magnitude;

        // ���� ���� �ʱ� �ӵ�
        //float initialVerticalSpeed = (y / totalTime + 0.5f * gravity * totalTime) / totalTime;
        float initialVerticalSpeed = (y + 0.5f * gravity * totalTime * totalTime) / totalTime;

        // ���� ���� �ʱ� �ӵ�
        Vector3 initialHorizontalVelocity = toTargetXZ / totalTime;

        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            // �ð��� �帧�� ���� ��ġ�� ���
            float verticalPosition = start.y + initialVerticalSpeed * elapsedTime - 0.5f * gravity * elapsedTime * elapsedTime;
            float horizontalPosition = start.x + initialHorizontalVelocity.x * elapsedTime;
            float depthPosition = start.z + initialHorizontalVelocity.z * elapsedTime;

            // ��ġ ������Ʈ
            transform.position = new Vector3(horizontalPosition, verticalPosition, depthPosition);

            yield return null;
        }
    }
}
