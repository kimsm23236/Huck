using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float timeMultiplier;
    [SerializeField]
    private float startHour;
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

    private bool isPlayingTransition = false;


    private DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

        skybox = RenderSettings.skybox;
        if(currentTime.Hour < sunsetHour)
            skybox.SetFloat("_Blend", 0f);
        else
            skybox.SetFloat("_Blend", 1f);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        // TransitionCheck();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        Debug.Log($"currentTime : {currentTime.Hour}");
        if(timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun()
    {
        float sunLightRotation;
        float moonLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            // ³· ½Ã°£ Ã³¸®
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
            moonLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        else
        {
            // ¹ã ½Ã°£ Ã³¸®
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
            moonLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
        moonLight.transform.rotation = Quaternion.AngleAxis(moonLightRotation, Vector3.right);
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

    void TransitionCheck()
    {
        if (isPlayingTransition)
            return;

        if(currentTime.Hour == sunriseHour - 1f)
        {
            Debug.Log($"³·À¸·Î");
            isPlayingTransition = true;
            StartCoroutine(TransitionSkybox(false));
        }
        else if(currentTime.Hour == sunsetHour - 1f)
        {
            Debug.Log($"¹ãÀ¸·Î");
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
}
