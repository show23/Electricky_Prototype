using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialShader : MonoBehaviour
{
    public Material material; // マテリアルをアタッチするための公開変数
    public string floatPropertyName = "_kirikae"; // 制御したいfloatプロパティの名前
    public float speed = 0.1f;
    // floatプロパティを変更する関数
    public void SetFloatProperty(float value)
    {
        if (material != null)
        {
            material.SetFloat(floatPropertyName, value);
        }
    }
    void Update()
    {
        // floatプロパティを変更する例
        float newValue = Mathf.PingPong(Time.time * speed, 0.0f); // 0から1までの値を徐々に変化
        SetFloatProperty(newValue);

    }
}
