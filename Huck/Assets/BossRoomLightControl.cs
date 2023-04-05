using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomLightControl : MonoBehaviour
{
    private Light light;
    private TimeController timeController;

    [SerializeField]
    private Color currentColor;
    [SerializeField]
    private Color destColor;
    [SerializeField]
    private Color roomEnterColor;
    [SerializeField]
    private Color battleStartColor;

    private float lerpSpeed = 0.3f;
    public float destIntensity = 1f;

    private bool isLerpColor = false;
    private bool isLerpIntensity = false;

    public delegate void EventHandler();
    public EventHandler onCloseDoor;
    public EventHandler onStartBattle;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        onCloseDoor = new EventHandler(() => StartCoroutine(PRC_CloseDoor()));
        onStartBattle = new EventHandler(PRC_StartBattle);
        GameManager.Instance.bossRoomLightController = this;
    }
    private void Update()
    {
        LerpColor();
        LerpIntensity();
    }
    void LerpColor()
    {
        if (!isLerpColor)
            return;

        currentColor = Color.Lerp(currentColor, destColor, lerpSpeed * Time.deltaTime);
        light.color = currentColor;
        if (currentColor == destColor)
        {
            Debug.Log("Lerp Color Exit");
            isLerpColor = false;
        }
    }
    void LerpIntensity()
    {
        if (!isLerpIntensity)
            return;

        light.intensity = Mathf.Lerp(light.intensity, destIntensity, lerpSpeed * Time.deltaTime);

        if (light.intensity == destIntensity)
        {
            Debug.Log("Lerp Intersity Exit");
            isLerpIntensity = false;
        }
    }

    IEnumerator PRC_CloseDoor()
    {
        timeController = GFunc.GetRootObj("TimeController").GetComponent<TimeController>();
        timeController.SetTime_Boss(0);
        yield return new WaitForSeconds(1f);

        destColor = roomEnterColor;
        destIntensity = 200f;

        isLerpColor = true;
        isLerpIntensity = true;
    }
    void PRC_StartBattle()
    {
        destColor = battleStartColor;

        destIntensity = 50f;
        isLerpColor = true;
        isLerpIntensity = true;
    }

}
