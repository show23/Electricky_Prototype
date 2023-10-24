using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [Range(0, 24)]
    public float timeOfDay;

    public float orbitSpeed = 1.0f;
    public Light sun;
    public Light moon;
    public MaterialShader materialShader;
    private bool isNight;

    public float rainyPercent = 1f;

    public float endTimer = 0.0f;
    public float EndRainTime = 30.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeOfDay += Time.deltaTime * orbitSpeed;
        if(timeOfDay > 24)
        {
            timeOfDay = 0;
        }
        UpdateTime();
        // ランダムなタイミングでmaterialShaderの値を変更する
        if (Random.Range(0f, 100f) < rainyPercent && materialShader.increasing == false) // 1%の確率で実行 (適切な確率に調整してください)
        {
            materialShader.increasing = true;
            
            Debug.Log("atari");
        }
        if(materialShader.increasing == true)
        {
            endTimer += Time.deltaTime;
        }
        if (endTimer > EndRainTime)
        {
            endTimer = 0.0f;
            materialShader.increasing = false;
           
        }

    }
    private void OnValidate()
    {
        UpdateTime();
    }
    private void  UpdateTime()
    {
        float alpha = timeOfDay / 24.0f;
        float sunRotation = Mathf.Lerp(-90, 270, alpha);
        float moonRotation = sunRotation - 180;
        sun.transform.rotation = Quaternion.Euler(sunRotation, -150.0f, 0);
        moon.transform.rotation = Quaternion.Euler(moonRotation, -150.0f, 0);

        CheckNightDayTransition();
    }

    private void CheckNightDayTransition()
    {
        if(isNight)
        {
            if(moon.transform.rotation.eulerAngles.x > 180)
            {
                StartDay();
            }
        }
        else
        {
            if (sun.transform.rotation.eulerAngles.x > 180)
            {
                StartNight();
            }
        }
    }

    private void StartDay()
    {
        isNight = false;
        sun.shadows = LightShadows.Soft;
        moon.shadows = LightShadows.None;
    }
    private void StartNight()
    {
        isNight = true;
        sun.shadows = LightShadows.None;
        moon.shadows = LightShadows.Soft;
    }
}
