using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] GameObject circlePrefab = default;
    [SerializeField] GameObject rectanglePrefab = default;
    [SerializeField] private int maxCirclePool = default;
    [SerializeField] private int maxRectanglePool = default;
    private Queue<GameObject> circlePool = new Queue<GameObject>();
    private Queue<GameObject> rectanglePool = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetupCircleIndicatorPool(maxCirclePool);
        SetupRectangIndicatorPool(maxRectanglePool);
    }

    private void OnDisable()
    {
        // ��Ȱ��ȭ�� �� Pool�� ����� ������Ʈ ����
        foreach (var _circle in circlePool)
        {
            Destroy(_circle);
        }
        foreach (var _rectangle in rectanglePool)
        {
            Destroy(_rectangle);
        }
    } // OnDisable

    // { ���������� ���� ����
    #region CircleIndicator
    //! CirclePool ä��� �Լ�
    private void SetupCircleIndicatorPool(int _number)
    {
        for (int i = 0; i < _number; i++)
        {
            GameObject circle = Instantiate(circlePrefab, Vector3.zero, circlePrefab.transform.rotation);
            circle.SetActive(false);
            circlePool.Enqueue(circle);
            circle.transform.parent = transform;
        }
    } // SetupCircleIndicatorPool

    //! CirclePool���� CircleIndicator �ϳ� �������� �Լ�
    public GameObject GetCircleIndicator(Vector3 pos, float scale, float time)
    {
        GameObject circleIndicator = default;
        if (circlePool.Count > 0)
        {
            circleIndicator = circlePool.Dequeue();
        }
        else
        {
            // circlePool�ȿ� ������Ʈ�� �������� ���� �� ���� ����
            circleIndicator = Instantiate(circlePrefab, Vector3.zero, circlePrefab.transform.rotation);
            circleIndicator.SetActive(false);
            circleIndicator.transform.parent = transform;
        }
        circleIndicator.transform.position = pos;
        circleIndicator.SetActive(true);
        circleIndicator.GetComponent<CircleIndicator>().InitCircleIndicator(scale, time);
        return circleIndicator;
    } // GetCircleIndicator

    //! ����� CircleIndicator ȸ���ϴ� �Լ�
    public void EnqueueCircleIndicator(GameObject circleIndicator)
    {
        circleIndicator.SetActive(false);
        circlePool.Enqueue(circleIndicator);
    } // EnqueueCircleIndicator
    #endregion // CircleIndicator

    #region RectangIndicator
    //! RectanglePool ä��� �Լ�
    private void SetupRectangIndicatorPool(int _number)
    {
        for (int i = 0; i < _number; i++)
        {
            GameObject rectangle = Instantiate(rectanglePrefab, Vector3.zero, circlePrefab.transform.rotation);
            rectangle.SetActive(false);
            rectanglePool.Enqueue(rectangle);
            rectangle.transform.parent = transform;
        }
    } // SetupRectangIndicatorPool

    //! rectanglePool���� RectangIndicator �ϳ� �������� �Լ�
    public GameObject GetRectangIndicator(bool isAttackerDead, Vector3 pos, float horizontalityRange, float attackLength, float time)
    {
        GameObject rectangleIndicator = default;
        if (rectanglePool.Count > 0)
        {
            rectangleIndicator = rectanglePool.Dequeue();
        }
        else
        {
            // rectanglePool�ȿ� ������Ʈ�� �������� ���� �� ���� ����
            rectangleIndicator = Instantiate(rectanglePrefab, Vector3.zero, rectanglePrefab.transform.rotation);
            rectangleIndicator.SetActive(false);
            rectangleIndicator.transform.parent = transform;
        }
        rectangleIndicator.transform.position = pos + (Vector3.up * 0.2f);
        rectangleIndicator.SetActive(true);
        rectangleIndicator.GetComponent<RectangIndicator>().InitRectangIndicator(isAttackerDead, horizontalityRange, attackLength, time);
        return rectangleIndicator;
    } // GetCircleIndicator

    //! ����� RectangleIndicator ȸ���ϴ� �Լ�
    public void EnqueueRectangIndicator(GameObject rectangleIndicator)
    {
        rectangleIndicator.SetActive(false);
        rectanglePool.Enqueue(rectangleIndicator);
    } // EnqueueRectangIndicator
    #endregion // RectangIndicator
    // } ���������� ���� ����
} // AttackIndicator
