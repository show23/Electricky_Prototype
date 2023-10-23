using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceEffect : MonoBehaviour
{
    [SerializeField]
    private Shader shader; // apply RainDrops.shader
    [SerializeField, Range(0, 10)]
    private float rainAmount = 0.5f;
    [SerializeField, Range(0, 1)]
    private float lightning = 1.0f;
    [SerializeField, Range(0, 1)]
    private float vignette = 1.0f;
    private Material mat;

    private void Awake()
    {
        mat = new Material(shader);
    }


    private void Update()
    {
        mat.SetFloat("_RainAmount", rainAmount);
        mat.SetFloat("_Lightning", lightning);
        mat.SetFloat("_Vignette", vignette);

    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }

}
