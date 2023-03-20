using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab = default;
    private Queue<GameObject> arrowPool = new Queue<GameObject>();
    private static ArrowPool instance = default;
    public static ArrowPool Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetupArrowPool();
    } // Start

    //! ArrowPool ä��� �Լ�
    private void SetupArrowPool()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
            arrowPool.Enqueue(arrow);
            arrow.SetActive(false);
            arrow.transform.parent = transform;
        }
        Debug.Log($"ȭ��Ǯ{arrowPool.Count}");
    } // SetupArrowPool

    //! ArrowPool���� ȭ�� �ѹ� �������� �Լ�
    public void GetArrow(Vector3 _dir, Vector3 _weaponPos)
    {
        GameObject arrow = default;
        if (arrowPool.Count > 0)
        {
            arrow = arrowPool.Dequeue();
        }
        else
        {
            // ArrowPool�� ȭ���� 0�� ������ �� ���� �ѹ� ����
            arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
            arrow.SetActive(false);
            arrow.transform.parent = transform;
        }
        arrow.transform.forward = _dir;
        arrow.transform.position = _weaponPos + _dir;
        //Debug.Log($"ȭ�� ���� : {arrow.transform.forward}, ������ : {arrow.transform.position}");
        arrow.SetActive(true);
    } // GetArrow

    //! �߻��� ȭ�� ȸ���ϴ� �Լ�
    public void ReturnArrow(GameObject _arrow)
    {
        arrowPool.Enqueue(_arrow);
        _arrow.SetActive(false);
    } // ReturnArrow
} // ArrowPool
