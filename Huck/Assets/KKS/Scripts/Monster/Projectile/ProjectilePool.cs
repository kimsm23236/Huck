using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projecttilePrefab = default;
    [SerializeField] private int maxPoolNumber = default;
    private Queue<GameObject> projecttilePool = new Queue<GameObject>();
    private static ProjectilePool instance = default;
    public static ProjectilePool Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetupProjecttilePool(maxPoolNumber);
    } // Start

    private void OnDisable()
    {
        // 비활성화할 때 projecttilePool안에 저장된 투사체 제거
        foreach (var _projecttile in projecttilePool)
        {
            Destroy(_projecttile);
        }
    } // OnDisable

    //! ProjecttilePool 채우는 함수
    private void SetupProjecttilePool(int _number)
    {
        for (int i = 0; i < _number; i++)
        {
            GameObject projecttile = Instantiate(projecttilePrefab, Vector3.zero, Quaternion.identity);
            projecttilePool.Enqueue(projecttile);
            projecttile.SetActive(false);
        }
    } // SetupProjecttilePool

    //! ProjecttilePool 투사체 한발 가져오는 함수
    public GameObject GetProjecttile()
    {
        GameObject projecttile = default;
        if (projecttilePool.Count > 0)
        {
            projecttile = projecttilePool.Dequeue();
        }
        else
        {
            // ProjecttilePool에 투사체가 존재하지 않을 때 새로 생성
            projecttile = Instantiate(projecttilePrefab, Vector3.zero, Quaternion.identity);
            projecttile.SetActive(false);
        }
        return projecttile;
    } // GetArrow

    //! 발사한 투사체 회수하는 함수
    public void EnqueueProjecttile(GameObject _projecttile)
    {
        projecttilePool.Enqueue(_projecttile);
        _projecttile.SetActive(false);
    } // ReturnArrow
} // ProjectilePool
