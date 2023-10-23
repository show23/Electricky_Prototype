using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialShader : MonoBehaviour
{
    public Material material; // �}�e���A�����A�^�b�`���邽�߂̌��J�ϐ�
    public string floatPropertyName = "_kirikae"; // ���䂵����float�v���p�e�B�̖��O
    public float speed = 0.1f;
    // float�v���p�e�B��ύX����֐�
    public void SetFloatProperty(float value)
    {
        if (material != null)
        {
            material.SetFloat(floatPropertyName, value);
        }
    }
    void Update()
    {
        // float�v���p�e�B��ύX�����
        float newValue = Mathf.PingPong(Time.time * speed, 0.0f); // 0����1�܂ł̒l�����X�ɕω�
        SetFloatProperty(newValue);

    }
}
