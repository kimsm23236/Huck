using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float defaultTimeMultiplier;
    [SerializeField]
    [Range(0, 5000)]
    private float timeMultiplier;

    [SerializeField]
    [Range(0, 23)]
    private int iStartHour;

    private float fStartHour;
    [SerializeField]
    private TMP_Text timeText;
    
    [SerializeField]
    private float sunriseHour;
    [SerializeField]
    private float sunsetHour;

    [SerializeField]
    private Color dayAmbientLight;
    [SerializeField]
    private Color nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve;
    [SerializeField]
    private AnimationCurve skyChangeCurve;

    [SerializeField]
    private Light sunLight;
    [SerializeField]
    private float maxSunLightIntensity;

    [SerializeField]
    private Light moonLight;
    [SerializeField]
    private float maxMoonLightIntensity;

    private Material skybox;

    //  private bool isPlayingTransition = false;


    private DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    private bool isNowDayTime;
    private bool isNowNight;

    public delegate void EventHandler();
    public EventHandler onStartDaytime;
    public EventHandler onStartNight;

    // Start is called before the first frame update
    void Start()
    {
        onStartDaytime = new EventHandler(() => Debug.Log("낮 진입"));
        onStartNight = new EventHandler(() => Debug.Log("밤 진입"));
        onStartNight += GameManager.Instance.SpawnMonster;

        timeMultiplier = defaultTimeMultiplier;

        fStartHour = Mathf.Round(iStartHour);
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(fStartHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

        skybox = RenderSettings.skybox;
        if (currentTime.Hour >= sunriseHour && currentTime.Hour < sunsetHour)
        {
            // skybox.SetFloat("_Blend", 0f);
            isNowDayTime = true;
        }
        else
        {
            // skybox.SetFloat("_Blend", 1f);
            isNowDayTime = false; 
        }
        isNowNight = !isNowDayTime;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        Cheat_ChangeTimeMultiplier();
        // TransitionCheck();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        if(timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
        EnterDaytimeCheck(currentTime.Hour);
        EnterNightCheck(currentTime.Hour);
    }

    private void EnterDaytimeCheck(int curHour)
    {
        // 낮이면 종료
        if (isNowDayTime)
            return;

        // 현재 시간이 일출 시간이 되었을 때 실행
        if (curHour >= sunriseHour && curHour < sunsetHour)
        {
            // 낮 시간 진입 프로세스 실행
            onStartDaytime();

            isNowDayTime = true;
            isNowNight = false;
        }  
    }
    private void EnterNightCheck(int curHour)
    {
        // 밤이면 종료
        if (isNowNight)
            return;

        // 현재 시간이 일몰 시간이 되었을 때 실행
        if (curHour >= sunsetHour)
        {
            // 밤 시간 진입 프로세스 실행
            onStartNight();

            isNowDayTime = false;
            isNowNight = true;
        }
    }

    private void RotateSun()
    {
        float sunLightRotation;
        // float moonLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            // 낮 시간 처리
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
            // moonLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        else
        {
            // 밤 시간 처리
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
            // moonLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
        // moonLight.transform.rotation = Quaternion.AngleAxis(moonLightRotation, Vector3.right);
    }

    private void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan diffTime = toTime - fromTime;

        if(diffTime.TotalSeconds < 0)
        {
            diffTime += TimeSpan.FromHours(24);
        }

        return diffTime;
    }

    // 보스전 전용

    public void SetTime_Boss(int hour)
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(hour);
        timeMultiplier = 0f;
    }

    #region Debugging Cheat
    // 디버깅용 치트 멤버

    private int multiplierIndex = 0;
    private float[] cheatTimeMultipliers = { 2000, 3500, 5000 };
    private float prevTimeMultiplier = default;
    void Cheat_ChangeTimeMultiplier()
    {
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            prevTimeMultiplier = timeMultiplier;
            if(multiplierIndex < cheatTimeMultipliers.Length)
            {
                timeMultiplier = cheatTimeMultipliers[multiplierIndex];
                multiplierIndex++;
            }
            else
            {
                timeMultiplier = defaultTimeMultiplier;
                multiplierIndex = 0;
            }
        }
    }

    #endregion

    /*
    void TransitionCheck()
    {
        if (isPlayingTransition)
            return;

        if(currentTime.Hour == sunriseHour - 1f)
        {
            Debug.Log($"낮으로");
            isPlayingTransition = true;
            StartCoroutine(TransitionSkybox(false));
        }
        else if(currentTime.Hour == sunsetHour - 1f)
        {
            Debug.Log($"밤으로");
            isPlayingTransition = true;
            StartCoroutine(TransitionSkybox(true));
        }
    }

    IEnumerator TransitionSkybox(bool toNight)
    {
        float from = 0f;
        float to = 1f;
        float time = 0f;
        float blendValue = 0f;
        float timeMultiplier = 0.1f;
        if (!toNight) 
        {
            from = 1f;
            to = 0f;
            blendValue = 1f;
        }
        while(true)
        {
            yield return null;
            time += Time.deltaTime*timeMultiplier;
            blendValue = Mathf.Lerp(from, to, skyChangeCurve.Evaluate(time));
            Debug.Log($"blendValue : {blendValue}");
            skybox.SetFloat("_Blend", blendValue);
            if (blendValue == to)
            {
                isPlayingTransition = false;
                break;
            }
        }
    }
    */
}
