using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseResourceObject : MonoBehaviour, IDamageable
{
    [SerializeField] 
    protected ResourceObjectSO resConfig;

    // Status
    protected float maxHealthPoint;
    protected float currentHealthPoint;
    protected EResourceType resType;
    protected EResourceLevel resLevel;
    // UI
    protected GameObject hpBarPrefab;
    protected ResObjHPBar hpBarUI;
    protected GameObject damageTextPrefab;
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

        string hpBarPrefabFileName = "ResObjHPBar UI";
        string damageTextPrefabFileName = "DamageText";
        string prefabPath = GData.PREFAB_PATH + GData.UI_PATH;
        hpBarPrefab = Resources.Load<GameObject>(prefabPath+ hpBarPrefabFileName);
        damageTextPrefab = Resources.Load<GameObject>(prefabPath + damageTextPrefabFileName);
        childMeshObj = gameObject.GetComponentInChildren<MeshFilter>().gameObject;
        defaultScale = childMeshObj.transform.localScale;

        GameObject UIObj = Instantiate(hpBarPrefab);
        hpBarUI = UIObj.GetComponentMust<ResObjHPBar>();
        if(hpBarUI != null && hpBarUI != default)
            hpBarUI.BindResObj(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 체력 0 일 때 파괴 함수
    protected virtual void Die()
    {
        Debug.Log($"{resConfig.ResourceName} Die");
        Destroy(gameObject, 0.3f);
    }

    // 데미지 처리 함수
    public virtual void TakeDamage(DamageMessage message)
    {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint - message.damageAmount, 0, maxHealthPoint);

        if (currentHealthPoint <= 0)
            Die();

        StartCoroutine(HitEffect());

        hpBarUI.onResObjTakeDamage(currentHealthPoint / maxHealthPoint);
        GameObject hudText = Instantiate(damageTextPrefab); // 생성할 텍스트 오브젝트
        hudText.transform.SetParent(transform);
        hudText.transform.position = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f)); // 표시될 위치
        hudText.GetComponent<DamageText>().damage = (int)message.damageAmount; // 데미지 전달
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
}
