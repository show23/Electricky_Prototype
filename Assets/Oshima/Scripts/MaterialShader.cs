using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialShader : MonoBehaviour
{
    public Material material; // �}�e���A�����A�^�b�`���邽�߂̌��J�ϐ�
    public GameObject rainPrefab;
    public Transform rainPoint;
    public string floatPropertyName = "_kirikae"; // ���䂵����float�v���p�e�B�̖��O
    public float switchStartRainTime = 10.0f; // �Q�[�������Ԃɓ��B������؂�ւ��鎞��
    public float switchEndRainTime = 10.0f;
    public float switchSpeed = 0.1f;
    private float lerpValue = 0.0f;
    public bool increasing = false;
    private float startTimer = 0.0f;
    private float endTimer = 0.0f;

    private bool hasSpawned = false;
    private GameObject spawnedRain;


    // float�v���p�e�B��ύX����֐�
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
    // �Q�[�������Ԃɉ�����float�v���p�e�B��؂�ւ���
    void Update()
    {
        Rain();
    }

    public void Rain()
    {
       

        if (increasing)
        {
            lerpValue += switchSpeed * 0.2f;

            if (!hasSpawned)
            {
                // �v���n�u����C���X�^���X�𐶐�
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

        if (!increasing)
        {
            if (hasSpawned)
            {
                Destroy(spawnedRain);
                hasSpawned = false;
            }
            lerpValue -= switchSpeed * 0.4f;
            if (lerpValue <= 0.0f)
            {
                lerpValue = 0.0f;
                endTimer = 0.0f;
            }
        }
    }

}
