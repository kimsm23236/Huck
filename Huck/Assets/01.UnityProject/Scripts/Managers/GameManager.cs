using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerObj = default;

    public GameObject procGenManager = default;
    public BuildSystem buildSystem = default;

    public TimeController timeController = default;

    //! { [KKS] 몬스터 스폰관련 변수
    [Header("스폰할 몬스터 Prefab")]
    public List<GameObject> nomalMonsterPrefab = default;
    public GameObject nameedMonsterPrefab = default;
    public GameObject bossMonsterPrefab = default;
    public Transform bossPos = default;
    private int count = 0;
    //! } [KKS] 몬스터 스폰관련 변수

    //! 게임매니져 초기화 함수
    protected override void Init()
    {
        procGenManager = GFunc.GetRootObj("ProcGenManager");
    }
    //! { [KKS] 몬스터 소환 함수
    public void SpawnMonster()
    {
        int nomalTypeCount = default;
        switch (count)
        {
            case 0:
                nomalTypeCount = 0;
                break;
            case 1:
                nomalTypeCount = 1;
                break;
            case 2:
                nomalTypeCount = 2;
                break;
            default:
                nomalTypeCount = 2;
                break;
        }

        int spawnNumber = 3 + count;
        if (spawnNumber > 10)
        {
            spawnNumber = 10;
        }

        List<Vector3> spawnPointList = new List<Vector3>();
        GetRandomPosition getRandomPosition = new GetRandomPosition();
        while (spawnPointList.Count < spawnNumber)
        {
            Vector3 point = getRandomPosition.GetRandomCirclePos(playerObj.transform.position, 30, 25);
            if (!spawnPointList.Contains(point))
            {
                spawnPointList.Add(point);
            }
        }

        if (count % 5 == 0 && count != 0)
        {
            Vector3 point = getRandomPosition.GetRandomCirclePos(playerObj.transform.position, 30, 25) + Vector3.up;
            Vector3 dirToTarget = (playerObj.transform.position - point).normalized;
            GameObject nomalMonster = Instantiate(nameedMonsterPrefab, point, Quaternion.LookRotation(dirToTarget));
        }
        else
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                int randomIndex = Random.Range(0, nomalTypeCount + 1);
                Vector3 dirToTarget = (playerObj.transform.position - spawnPointList[i]).normalized;
                GameObject nomalMonster = Instantiate(nomalMonsterPrefab[randomIndex], spawnPointList[i] + Vector3.up, Quaternion.LookRotation(dirToTarget));
            }
        }
        count += 1;
    } // SpawnMonster

    public void BossSpwan()
    {
        GameObject boss = Instantiate(bossMonsterPrefab, bossPos.position, bossPos.rotation);
        boss.SetActive(false);
    } // BossSpwan
    //! } [KKS] 몬스터 소환 함수
}
