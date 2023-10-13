using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    PlayerManager playerManager;
    // public float beamSpeed = 10f; // ビームの速度
    //private float interactionRange = 0.0f; // PlayerとEnemyの間の許容距離
   

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        // キーが押されている間
        if (Input.GetKey(playerManager.attackKey))
        {
            // interactionRangeを増加させる
            playerManager.interactionRange += playerManager.increasedSpeed * Time.deltaTime;
            // interactionRangeが最大許容距離を超えないように制限
            playerManager.interactionRange = Mathf.Min(playerManager.interactionRange, playerManager.maxInteractionRange);

        }
        else
        {
            InteractWithEnemy();
            // キーが離されたらinteractionRangeを元に戻す
            playerManager.interactionRange = 0.0f; // 初期値に戻す、必要に応じて変更

        }

        // スペースキーが押されたらビームを発射
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 一番近くのカメラ内の敵を探す
            GameObject nearestEnemy = FindNearestEnemyInCamera();

            // ターゲットが設定されている場合
            if (nearestEnemy != null)
            {
                // ビームを発射
                ShootBeam(nearestEnemy);
            }
        }
    }

    void InteractWithEnemy()
    {
        // Playerの位置
        Vector3 playerPosition = transform.position;

        // 範囲内の全てのEnemyを検出
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            // EnemyとPlayerの距離を計算
            float distance = Vector3.Distance(playerPosition, enemy.transform.position);

            // 許容距離内にいるかどうかを確認
            if (distance <= playerManager.interactionRange)
            {
                // Enemyを破壊
                Destroy(enemy);
            }
        }
    }

    GameObject FindNearestEnemyInCamera()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // 敵のタグを設定しておく

        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerManager.mainCamera); // カメラの視錘台（frustum）のプレーンを取得

        foreach (GameObject enemy in enemies)
        {
            if (GeometryUtility.TestPlanesAABB(planes, enemy.GetComponent<Collider>().bounds))
            {
                // カメラの視錘台内にいる敵だけを対象にする
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestEnemy = enemy;
                    nearestDistance = distance;
                }
            }
        }

        return nearestEnemy;
    }

    void ShootBeam(GameObject target)
    {
        // ビームをプレファブからインスタンス化
        GameObject beamInstance = Instantiate(playerManager.beamPrefab, playerManager.firePoint.position, playerManager.firePoint.rotation);

        // ビームのスクリプトを取得
        Bullet beamScript = beamInstance.GetComponent<Bullet>();

        // ビームのターゲットを設定
        beamScript.SetTarget(target.transform);

        // 一定時間後にビームを破壊する（例：5秒後）
        Destroy(beamInstance, 5f);
    }

  
}