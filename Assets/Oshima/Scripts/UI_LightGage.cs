using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LightGage : MonoBehaviour
{
    //PlayerManager playerManager;
    //[SerializeField]
    //Image image1;
    //Image image2;

    //private float playerDistance;
    //private bool gageIncrease = false;
    //private bool gaugeVisible = false;
    //private float distanceMoved;
    //public float fillSpeed = 1f;

    //private float _maxHp = 100;     //最大体力
    //// Start is called before the first frame update
    //void Start()
    //{
    //    playerManager = GetComponent<PlayerManager>();
    //    image2 = GetComponent<Image>();
    //    image2.fillAmount = 0f; // ゲージをゼロからスタート
    //    image2.fillOrigin = (int)Image.OriginHorizontal.Left; // ゲージの増加を右回りに
    //    image1.color = new Color(1f, 1f, 1f, 0f); // 最初は透明

    //    // ゲージを最初は非表示に
    //    image2.enabled = false;
    //    image1.enabled = false;
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
    //    {
    //        distanceMoved += 1;
    //    }



    //    // プレイヤーの距離に応じてimage2.fillAmountを増加
    //    float maxDistance = 1.0f; // 例として最大距離を100とします
    //    float fillAmount = distanceMoved / maxDistance;

    //    // ゲージが増加する速度を調整する係数を設定

    //    image2.fillAmount = fillAmount * fillSpeed;
    //    //if (!Input.GetKey(KeyCode.P))
    //    //{

    //    //}
    //    //else
    //    //{
    //    //    //image2.fillAmount -= fillSpeed;
    //    //}

    //    // ゲージが一定以上増加したら色を表示
    //    if (image2.fillAmount > 0.01f && !gaugeVisible)
    //    {
    //        gaugeVisible = true;
    //        image2.enabled = true;
    //        image1.enabled = true;
    //    }
    //}
    //public float FillAmount
    //{
    //    get { return image2.fillAmount; }
    //    set { image2.fillAmount = value; }
    //}
    
}
