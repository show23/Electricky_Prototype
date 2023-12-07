using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : MonoBehaviour
{
    public static Player_Bullet instance;
    private Transform target; // ビームの追尾対象
    public float beamSpeed = 100f; // ビームの速度
    public float curveStrength = 1f; // カーブの強度

    private float curveDirectionUp = 0; // カーブ方向を制御する変数 (0: 上, 1: 左, 2: 右, 3: 上向きカーブ)
    private float curveDirectionRight = 0;
    private float curveDirectionLeft = 0;
    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    // 敵の追跡対象を設定
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Start()
    {
        // カーブ方向をランダムに選択
        curveDirectionUp = Random.Range(0.0f, 3.0f);
        curveDirectionRight = Random.Range(-3.0f, 3.0f);
        curveDirectionLeft = Random.Range(-3.0f, 3.0f);

    }

    void Update()
    {
        // 追尾対象が存在する場合
        if (target != null)
        {
            // ビームの方向を設定（追尾）
            Vector3 direction = (target.position - transform.position).normalized;

            // カーブを適用
            Vector3 curve = Vector3.zero;
            curve = new Vector3(curveDirectionLeft, curveDirectionUp, curveDirectionRight) * curveStrength;
           

            GetComponent<Rigidbody>().velocity = (direction + curve) * beamSpeed;
        }
    }
   
}
