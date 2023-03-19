using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab = default;
    private Queue<GameObject> arrowPool = new Queue<GameObject>();
    public static ArrowPool instance = default;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetupArrowPool();
    }
    
    private void SetupArrowPool()
    {
        for(int i=0;i< 5;i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
            arrowPool.Enqueue(arrow);
            arrow.SetActive(false);
            arrow.transform.parent = transform;
        }
    }

    public GameObject GetArrow()
    {
        GameObject arrow = arrowPool.Dequeue();
        return arrow;
    }
    public void ReturnArrow(GameObject _arrow)
    {
        arrowPool.Enqueue(_arrow);
        _arrow.SetActive(false);
    }
}
