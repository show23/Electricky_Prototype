using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour
{
    //点滅させる光を取得（インスペクター側でアタッチしてください）
    public GameObject startLight;
    //光の強さ取得するための変数を作る
    Light lightIntence;

    void Start()
    {
        //光の強さ(Intensity)を取得。
        lightIntence = startLight.GetComponent<Light>();
    }
    void Update()
    {
        //後述するフラッシュ関数をコルーチンで開始。
        //光の強さを0と1に0.7秒間隔で表示させる。
        StartCoroutine(Frash(0f, 1f));
    }


    //光の強さを決定できる関数（引数で調整できる）。
    private void LightIntencity(float intenceAmount)
    {
        lightIntence.intensity = intenceAmount;
    }


    //光の点滅関数。
    IEnumerator Frash(float intenceAmount, float intenceAmount2)
    {
        LightIntencity(intenceAmount);
        yield return new WaitForSeconds(0.7f);
        LightIntencity(intenceAmount2);
    }
}
