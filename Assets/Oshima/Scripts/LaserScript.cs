using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public GameObject laserVFX; // レーザーVFXのPrefab
    public Transform emitterTransform; // レーザーの発射位置
    public float laserRange = 100f; // レーザーの射程距離

    private LineRenderer laserLine; // レーザーライン
    private RaycastHit hitInfo; // レイキャストが当たったオブジェクトの情報

    void Start()
    {
        // レーザーラインの初期化
        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false; // 初期状態では非表示
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            FireLaser();
        }

        // レーザーを非表示にする
        laserLine.enabled = false;
    }

    void FireLaser()
    {
        // レーザーを表示
        laserLine.enabled = true;

        // レーザーVFXを生成
        GameObject currentLaser = Instantiate(laserVFX, emitterTransform.position, emitterTransform.rotation);

        // レーザーの方向を計算
        Vector3 direction = emitterTransform.forward;

        // レイキャストを実行して敵に当たるかどうかを判定
        if (Physics.Raycast(emitterTransform.position, direction, out hitInfo, laserRange))
        {
            // レーザーラインの位置を設定
            laserLine.SetPosition(0, emitterTransform.position);
            laserLine.SetPosition(1, hitInfo.point);

            // ヒットしたオブジェクトに対する処理をここで実行
            GameObject hitObject = hitInfo.collider.gameObject;
            if (hitObject.CompareTag("Enemy"))
            {
                // ここで敵にダメージを与えるか、破壊するなどのアクションを実行
                // 例: hitObject.GetComponent<EnemyScript>().TakeDamage(damageAmount);
            }
        }
        else
        {
            // レーザーラインの位置を設定（射程距離まで伸ばす）
            laserLine.SetPosition(0, emitterTransform.position);
            laserLine.SetPosition(1, emitterTransform.position + direction * laserRange);
        }

        // レーザーVFXを少し遅延して破棄
        Destroy(currentLaser, 0.1f);
    }
}
