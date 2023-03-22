using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResObjHPBar : MonoBehaviour
{
    private BaseResourceObject bindResouce;
    private TMP_Text resourceNameText;
    private Slider hpBarSlider;
    [SerializeField]
    private float hideRate = 5f;
    private float timer = 0f;
    private float ratio;

    

    public delegate void EventHandler_OneParam(float floatValue);
    public EventHandler_OneParam onResObjTakeDamage;
    private void Awake()
    {
        onResObjTakeDamage = new EventHandler_OneParam(ChangeHPRatio);
        resourceNameText = gameObject.FindChildComponent<TMP_Text>("Text_ResName");
        hpBarSlider = gameObject.FindChildComponent<Slider>("HPBar");

        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Billboard();
        HideTimer();
        SmoothSlideHPBar();
    }
    void Billboard()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    void SetPositioning()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.gameObject.GetComponent<Collider>().bounds.size.y);
    }
    void SmoothSlideHPBar()
    {
        hpBarSlider.value = Mathf.Lerp(hpBarSlider.value, ratio, Time.deltaTime * 10);
    }

    void HideTimer()
    {
        timer += Time.deltaTime;
        if (timer > hideRate)
            HideUI();   
    }
    void ChangeHPRatio(float ratio)
    {
        ShowUI();
        this.ratio = ratio;
    }

    void ShowUI()
    {
        gameObject.SetActive(true);
        timer = 0f;
    }

    void HideUI()
    {
        gameObject.SetActive(false);
    }
    public void BindResObj(GameObject resObj)
    {
        bindResouce = resObj.GetComponentMust<BaseResourceObject>();
        transform.parent = resObj.transform;
        SetPositioning();
        resourceNameText.text = bindResouce.ResourceConfig.ResourceName;
        hpBarSlider.value = 1f;
        ratio = 1f;
        gameObject.SetActive(false);
        Debug.Log("binding Success!");
    }
}
