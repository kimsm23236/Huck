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

    //! ArrowPool 채우는 함수
    private void SetupArrowPool()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
            arrowPool.Enqueue(arrow);
            arrow.SetActive(false);
            arrow.transform.parent = transform;
        }
        Debug.Log($"화살풀{arrowPool.Count}");
    } // SetupArrowPool

    //! ArrowPool에서 화살 한발 가져오는 함수
    public void GetArrow(Vector3 _dir, Vector3 _weaponPos)
    {
        GameObject arrow = default;
        if (arrowPool.Count > 0)
        {
            arrow = arrowPool.Dequeue();
        }
        else
        {
            // ArrowPool에 화살이 0개 이하일 때 새로 한발 생성
            arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
            arrow.SetActive(false);
            arrow.transform.parent = transform;
        }
        arrow.transform.forward = _dir;
        arrow.transform.position = _weaponPos + _dir;
        //Debug.Log($"화살 방향 : {arrow.transform.forward}, 포지션 : {arrow.transform.position}");
        arrow.SetActive(true);
    } // GetArrow

    //! 발사한 화살 회수하는 함수
    public void ReturnArrow(GameObject _arrow)
    {
        arrowPool.Enqueue(_arrow);
        _arrow.SetActive(false);
    } // ReturnArrow
} // ArrowPool
