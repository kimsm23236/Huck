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

    // ü�� 0 �� �� �ı� �Լ�
    protected virtual void Die()
    {
        // ������ ������Ʈ ���� ó��
        DropItem(dropItems, transform);
        Destroy(gameObject, 0.1f);
        
    }

    // ������ ó�� �Լ�
    public virtual void TakeDamage(DamageMessage message)
    {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint - message.damageAmount, 0, maxHealthPoint);
        Debug.Log($"{currentHealthPoint}, {gameObject.name}");
        if (currentHealthPoint <= 0)
            Die();

        StartCoroutine(HitEffect());

    }

    // ���� ���� �� �۾����ٰ� Ŀ���� ����Ʈ
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

            // ��� Ȯ�� �˻�
            int dropPercentage = Random.Range(0, 100);
            if (dropPercentage > item.dropPercentage)
                continue;


            // ���� ��ġ, ���� �� ����
            Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 positionOffset = new Vector3(Random.Range(-maxPositionJitter, maxPositionJitter),
                                                            yOffset,
                                                            Random.Range(-maxPositionJitter, maxPositionJitter));

            // ���� �� ���� ó��
            var spawnedGO = Instantiate(item.prefab, spawnLocation + positionOffset, spawnRotation);
            Item dropedItem = spawnedGO.GetComponent<Item>();
            if(dropedItem != null)
            {
                // ������ ���� ó��
                dropedItem.itemCount = Random.Range(item.minDropCount, item.maxDropCount);
            }
        }
    }
}
