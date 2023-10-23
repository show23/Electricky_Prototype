using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialShader : MonoBehaviour
{
    public Material material; // マテリアルをアタッチするための公開変数
    public GameObject rainPrefab;
    public Transform rainPoint;
    public string floatPropertyName = "_kirikae"; // 制御したいfloatプロパティの名前
    public float switchStartRainTime = 10.0f; // ゲーム内時間に到達したら切り替える時間
    public float switchEndRainTime = 10.0f;
    public float switchSpeed = 0.1f;
    private float lerpValue = 0.0f;
    private bool increasing = false;
    private float startTimer = 0.0f;
    private float endTimer = 0.0f;

    private bool hasSpawned = false;
    private GameObject spawnedRain;


    // floatプロパティを変更する関数
    public void SetFloatProperty(float value)
    {
        if (material != null)
        {
            material.SetFloat(floatPropertyName, value);
        }
    }
    private void Start()
    {

       
    }
    // ゲーム内時間に応じてfloatプロパティを切り替える
    void Update()
    {
        
        Debug.Log(lerpValue);
        startTimer += Time.deltaTime;
        if (startTimer >= switchStartRainTime)
        {
            increasing = true;
        }
     
        if (increasing)
        {
            lerpValue += switchSpeed * 0.1f;
            
            if (!hasSpawned)
            {
                // プレハブからインスタンスを生成
                spawnedRain = Instantiate(rainPrefab, rainPoint.position, rainPoint.rotation);
                hasSpawned = true;
            }
            if (lerpValue >= 1.0f)
            {
                
                lerpValue = 1.0f;
                endTimer += Time.deltaTime;
            }
        }
        SetFloatProperty(lerpValue);

        if (endTimer >= switchEndRainTime)
        {
            increasing = false;
            startTimer = 0.0f;
            
        }
        if (!increasing && endTimer >= switchEndRainTime)
        {
            if (hasSpawned)
            {
                Destroy(spawnedRain);
                hasSpawned = false;
            }
            lerpValue -= switchSpeed * 0.1f;
            if (lerpValue <= 0.0f)
            {
                lerpValue = 0.0f;
                endTimer = 0.0f;
            }
        }
    }
}
