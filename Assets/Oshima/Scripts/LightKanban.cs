using System.Collections;
using UnityEngine;

public class LightKanban : MonoBehaviour
{
    public Material material;
    public float changeInterval = 1.0f; // 色を変える間隔
    public float emissionIntensity = 1.0f; // エミッションの強度

    void Start()
    {
        if (material == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                material = renderer.material;
            }
            else
            {
                Debug.LogError("Material not assigned and no Renderer found on the GameObject.");
                return;
            }
        }

        // 初回の色変更
        StartCoroutine(RandomizeEmissionColor());
    }

    IEnumerator RandomizeEmissionColor()
    {
        while (true)
        {
            // ランダムな色を生成 (Intensityを固定している)
            Color randomColor = new Color(Random.value, Random.value, Random.value) * emissionIntensity;

            // Emission Mapの色を設定
            material.SetColor("_EmissiveColor", randomColor);

            yield return new WaitForSeconds(changeInterval);
        }
    }
}
