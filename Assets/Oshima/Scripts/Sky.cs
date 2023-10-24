using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Sky : MonoBehaviour
{
    private HDRISky sky;
    public MaterialShader materialShader;
    public Light directionalLight;
    public float decreaseRate = 1.0f;
    public float minDecreaseRate = 0.1f;
    public Volume globalVolume;
   
    public float rateChangeSpeed = 0.01f;


    //天気の変化
    public float initialIntensity = 100000.0f;
    public float initialDesiredLuxValue = 20000.0f;
    public float targetIntensity = 0.0f;
    public float targetLuxValue = 0.0f;
    [Range(24, 0)]
    public float timeOfDay;
    public float orbitSpeed = 1.0f;


    private bool isNight;


    void Start()
    {

        if (globalVolume.profile.TryGet(out sky))
        {
            directionalLight.intensity = initialIntensity;
            sky.desiredLuxValue.value = initialDesiredLuxValue;
        }
    }

    void Update()
    {
        //timeOfDay += Time.deltaTime * orbitSpeed;
        
        //if (timeOfDay > 24)
        //{
        //    timeOfDay = 0;
        //}
        //var value = Mathf.PingPong(Time.time, timeOfDay);
        UpdateDayTime();
        
    }

    private void UpdateDayTime()
    {
       
       
        
        //timeOfDay += Time.deltaTime * orbitSpeed;
        var value = Mathf.PingPong(Time.time, timeOfDay);
        float dayLength = 24.0f;

        float intensityChangeRate = (targetIntensity - initialIntensity) / dayLength;
        float luxValueChangeRate = (targetLuxValue - initialDesiredLuxValue) / dayLength;

        float currentTimeOfDay = value % dayLength;

        directionalLight.intensity = initialIntensity + (intensityChangeRate * currentTimeOfDay);
        sky.desiredLuxValue.value = initialDesiredLuxValue + (luxValueChangeRate * currentTimeOfDay);
        Debug.Log(directionalLight.intensity);
    }

   


    // 曇り
    public void CloudyWeather()
    {
        materialShader.increasing = true;

        if (directionalLight.intensity > targetIntensity && sky.desiredLuxValue.value > targetLuxValue)
        {
            if (decreaseRate > minDecreaseRate)
            {

                decreaseRate -= (decreaseRate - minDecreaseRate) * rateChangeSpeed * Time.deltaTime;
            }
            float intensityDelta = initialIntensity - targetIntensity;
            float luxValueDelta = initialDesiredLuxValue - targetLuxValue;

            float step = Time.deltaTime * decreaseRate;

            if (directionalLight.intensity - step * (intensityDelta / luxValueDelta) > targetIntensity)
            {
                directionalLight.intensity -= step * (intensityDelta / luxValueDelta);
            }
            else
            {
                directionalLight.intensity = targetIntensity;
            }

            if (sky.desiredLuxValue.value - step > targetLuxValue)
            {
                sky.desiredLuxValue.value -= step;
            }
            else
            {
                sky.desiredLuxValue.value = targetLuxValue;
            }

            Debug.Log("Directional Light's Intensity: " + directionalLight.intensity);
            Debug.Log("HDRISky's DesiredLuxValue: " + sky.desiredLuxValue.value);
        }


    }

    public void ClearSky()
    {
        materialShader.increasing = false;

        if (directionalLight.intensity < initialIntensity && sky.desiredLuxValue.value < initialDesiredLuxValue)
        {
            if (decreaseRate > minDecreaseRate)
            {
                decreaseRate -= (decreaseRate - minDecreaseRate) * rateChangeSpeed * Time.deltaTime;
            }
            float intensityDelta = initialIntensity - directionalLight.intensity;
            float luxValueDelta = initialDesiredLuxValue - sky.desiredLuxValue.value;

            float step = Time.deltaTime * decreaseRate;

            if (directionalLight.intensity + step * (intensityDelta / luxValueDelta) < initialIntensity)
            {
                directionalLight.intensity += step * (intensityDelta / luxValueDelta);
            }
            else
            {
                directionalLight.intensity = initialIntensity; // ターゲットに達するように設定
            }

            if (sky.desiredLuxValue.value + step < initialDesiredLuxValue)
            {
                sky.desiredLuxValue.value += step;
            }
            else
            {
                sky.desiredLuxValue.value = initialDesiredLuxValue; // ターゲットに達するように設定
            }

            Debug.Log("Directional Light's Intensity: " + directionalLight.intensity);
            Debug.Log("HDRISky's DesiredLuxValue: " + sky.desiredLuxValue.value);
        }

    }

    public void NoonEvening()
    {


        //if (directionalLight.intensity != eveningIntensity || sky.desiredLuxValue.value != eveningLuxValue)
        //{
        //    if (decreaseRate > minDecreaseRate)
        //    {
        //        decreaseRate -= (decreaseRate - minDecreaseRate) * rateChangeSpeed * Time.deltaTime;
        //    }
        //    float intensityDelta = directionalLight.intensity - eveningIntensity;
        //    float luxValueDelta = sky.desiredLuxValue.value - eveningLuxValue;

        //    float step = Time.deltaTime * decreaseRate;

        //    if (directionalLight.intensity - step * (intensityDelta / luxValueDelta) > eveningIntensity)
        //    {
        //        directionalLight.intensity -= step * (intensityDelta / luxValueDelta);
        //    }
        //    else
        //    {
        //        directionalLight.intensity = eveningIntensity;
        //    }

        //    if (sky.desiredLuxValue.value - step > eveningLuxValue)
        //    {
        //        sky.desiredLuxValue.value -= step;
        //    }
        //    else
        //    {
        //        sky.desiredLuxValue.value = eveningLuxValue;
        //    }

        //    Debug.Log("Directional Light's Intensity: " + directionalLight.intensity);
        //    Debug.Log("HDRISky's DesiredLuxValue: " + sky.desiredLuxValue.value);
        //}
        //if (directionalLight.intensity < 80000)
        //{
        //    decreaseRate = 1500;
        //}
        //if (directionalLight.intensity < 50000)
        //{
        //    decreaseRate = 1000;
        //}
        //if (directionalLight.intensity < 10000)
        //{
        //    decreaseRate = 500;
        //}
        //if (directionalLight.intensity <1000 )
        //{
        //    decreaseRate = 100;
        //}
        //if (directionalLight.intensity < 500)
        //{
        //    decreaseRate = 50;
        //}
        //if (directionalLight.intensity < 100)
        //{
        //    decreaseRate = 10;
        //}
        //if (directionalLight.intensity <10 )
        //{
        //    decreaseRate = 1;
        //}
    }


}