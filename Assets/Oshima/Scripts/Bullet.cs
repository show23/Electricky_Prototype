using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static Bullet instance;
    private Transform target; // ビームの追尾対象
    public float beamSpeed = 100f; // ビームの速度
    public float curveStrength = 1f; // カーブの強度

    private int curveDirection = 0; // カーブ方向を制御する変数 (0: 上, 1: 左, 2: 右, 3: 上向きカーブ)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // 敵の追跡対象を設定
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Start()
    {
        // カーブ方向をランダムに選択
        curveDirection = Random.Range(0, 6);
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

            // カーブ方向に応じてカーブを適用
            switch (curveDirection)
            {
                case 0: // 上向きカーブ
                    curve = Vector3.up * curveStrength;
                    break;
                case 1: // 左向きカーブ
                    curve = -Vector3.Cross(Vector3.up, direction) * curveStrength;
                    break;
                case 2: // 右向きカーブ
                    curve = Vector3.Cross(Vector3.up, direction) * curveStrength;
                    break;
                case 3: // 上向きカーブ
                    curve = Vector3.up * curveStrength;
                    break;
                case 4:
                    curve = Vector3.up * curveStrength + Vector3.Cross(direction, Vector3.up) * curveStrength;
                    break;
                case 5:
                    curve = Vector3.up * curveStrength - Vector3.Cross(direction, Vector3.up) * curveStrength;
                    break;

                default:
                    break;
            }

            GetComponent<Rigidbody>().velocity = (direction + curve) * beamSpeed;
        }
    }
}
