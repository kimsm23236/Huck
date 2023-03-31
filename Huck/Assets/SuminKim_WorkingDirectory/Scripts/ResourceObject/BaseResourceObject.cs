using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class BaseResourceObject : MonoBehaviour, IDamageable, IDropable
{
    [SerializeField] 
    protected ResourceObjectSO resConfig;

    [SerializeField] 
    protected List<DropItemConfig> dropItems;

    // Status
    protected int maxHealthPoint;
    protected int currentHealthPoint;

    public int HP
    {
        get { return currentHealthPoint; }
    }

    protected EResourceType resType;
    protected EResourceLevel resLevel;
    // Effect Variable
    protected GameObject childMeshObj;
    protected Vector3 defaultScale;
    
    public ResourceObjectSO ResourceConfig
    {
        get { return resConfig; }
        set { resConfig = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealthPoint = resConfig.HP;
        currentHealthPoint = resConfig.HP;
        resType = resConfig.ResourceType;
        resLevel = resConfig.ResourceLevel;
        dropItems = resConfig.DropConfigs;

        childMeshObj = gameObject.GetComponentInChildren<MeshFilter>().gameObject;
        defaultScale = childMeshObj.transform.localScale;
    }

    // 체력 0 일 때 파괴 함수
    protected virtual void Die()
    {
        // 아이템 오브젝트 스폰 처리
        DropItem(dropItems, transform);
        Destroy(gameObject, 0.1f);
        
    }

    // 데미지 처리 함수
    public virtual void TakeDamage(DamageMessage message)
    {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint - message.damageAmount, 0, maxHealthPoint);
        Debug.Log($"{currentHealthPoint}, {gameObject.name}");
        if (currentHealthPoint <= 0)
            Die();

        StartCoroutine(HitEffect());

    }

    // 공격 당할 시 작아졌다가 커지는 이펙트
    IEnumerator HitEffect()
    {
        childMeshObj.SetLocalScale(defaultScale * 0.8f);
        while(true)
        {
            yield return null;
            childMeshObj.SetLocalScale(Vector3.Lerp(childMeshObj.transform.localScale, defaultScale, Time.deltaTime * 10f));

            if (childMeshObj.transform.localScale == defaultScale)
                break;
        }
    }

    public void DropItem(List<DropItemConfig> dropItems, Transform targetTransform)
    {
        
        float yOffset = 1f;
        float maxPositionJitter = 0.5f;
        
        Vector3 spawnLocation = targetTransform.position;
        foreach (var item in dropItems)
        {
            if(item == null || item == default)
                continue;

            // 드랍 확률 검사
            int dropPercentage = Random.Range(0, 100);
            if (dropPercentage > item.dropPercentage)
                continue;


            // 스폰 위치, 각도 값 셋팅
            Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 positionOffset = new Vector3(Random.Range(-maxPositionJitter, maxPositionJitter),
                                                            yOffset,
                                                            Random.Range(-maxPositionJitter, maxPositionJitter));

            // 스폰 및 이후 처리
            var spawnedGO = Instantiate(item.prefab, spawnLocation + positionOffset, spawnRotation);
            Item dropedItem = spawnedGO.GetComponent<Item>();
            if(dropedItem != null)
            {
                // 아이템 갯수 처리
                dropedItem.itemCount = Random.Range(item.minDropCount, item.maxDropCount);
            }
        }
    }
}
