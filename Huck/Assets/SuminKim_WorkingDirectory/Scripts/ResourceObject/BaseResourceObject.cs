using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseResourceObject : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected ResourceObjectSO resConfig;

    // Status
    protected int maxHealthPoint;
    protected int currentHealthPoint;

    //
    public int HP
    {
        get { return currentHealthPoint; }
    }
    // 

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
        hpBarPrefab = Resources.Load<GameObject>(prefabPath + hpBarPrefabFileName);
        damageTextPrefab = Resources.Load<GameObject>(prefabPath + damageTextPrefabFileName);
        childMeshObj = gameObject.GetComponentInChildren<MeshFilter>().gameObject;
        defaultScale = childMeshObj.transform.localScale;

        GameObject UIObj = Instantiate(hpBarPrefab);
        hpBarUI = UIObj.GetComponentMust<ResObjHPBar>();
        if (hpBarUI != null && hpBarUI != default)
            hpBarUI.BindResObj(gameObject);

    }

    // Update is called once per frame
    void Update()
    {

    }

    // ü�� 0 �� �� �ı� �Լ�
    protected virtual void Die()
    {
        Debug.Log($"{resConfig.ResourceName} Die");
        Destroy(gameObject, 0.3f);
    }

    // ������ ó�� �Լ�
    public virtual void TakeDamage(DamageMessage message)
    {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint - message.damageAmount, 0, maxHealthPoint);

        if (currentHealthPoint <= 0)
            Die();

        StartCoroutine(HitEffect());

        hpBarUI.onResObjTakeDamage(currentHealthPoint / maxHealthPoint);
        GameObject hudText = Instantiate(damageTextPrefab); // ������ �ؽ�Ʈ ������Ʈ
        hudText.transform.SetParent(transform);
        hudText.transform.position = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f)); // ǥ�õ� ��ġ
        hudText.GetComponent<DamageText>().damage = (int)message.damageAmount; // ������ ����
    }

    // ���� ���� �� �۾����ٰ� Ŀ���� ����Ʈ
    IEnumerator HitEffect()
    {
        childMeshObj.SetLocalScale(defaultScale * 0.8f);
        while (true)
        {
            yield return null;
            childMeshObj.SetLocalScale(Vector3.Lerp(childMeshObj.transform.localScale, defaultScale, Time.deltaTime * 10f));

            if (childMeshObj.transform.localScale == defaultScale)
                break;
        }
    }
}
