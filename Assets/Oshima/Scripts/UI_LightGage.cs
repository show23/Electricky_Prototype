using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LightGage : MonoBehaviour
{
    [SerializeField]
    Image image1;
    Image image2;

    private float playerDistance;
    private bool gageIncrease = false;
    private bool gaugeVisible = false;
    private float distanceMoved;
    public float fillSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        image2 = GetComponent<Image>();
        image2.fillAmount = 0f; // ゲージをゼロからスタート
        image2.fillOrigin = (int)Image.OriginHorizontal.Left; // ゲージの増加を右回りに
        image1.color = new Color(1f, 1f, 1f, 0f); // 最初は透明

        // ゲージを最初は非表示に
        image2.enabled = false;
        image1.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            distanceMoved += 1;
        }
        

        // プレイヤーの距離に応じてimage2.fillAmountを増加
        float maxDistance = 100.0f; // 例として最大距離を100とします
        float fillAmount = distanceMoved / maxDistance;

        // ゲージが増加する速度を調整する係数を設定
        
        image2.fillAmount = fillAmount * fillSpeed;

        // ゲージが一定以上増加したら色を表示
        if (image2.fillAmount > 0.01f && !gaugeVisible)
        {
            gaugeVisible = true;
            image2.enabled = true;
            image1.enabled = true;
        }
    }
}
