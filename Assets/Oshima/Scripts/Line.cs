using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    PlayerManager playerManager;
    //シングルトン
   
   
    private List<Transform> touchedEnemies = new List<Transform>(); // プレイヤーが触れた敵を記録するリスト
    private float speed;
    private float originalSpeed; // オリジナルの速度を保存する変数
    private bool isDrawingLine = false;
    private float buttonPressTime = 0f; // ボタンが押された時間
   
    private Transform lastTouchedEnemy; // 最後に触れた敵のTransformを記録する変数

  
    
    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        originalSpeed = playerManager.moveSpeed; // ゲーム開始時の速度を保存
    }

    void Update()
    {
        speed = playerManager.dashSpeed;
       

        // 左クリックが押されたかをチェック
        if (Input.GetMouseButtonDown(0))
        {
            if (!isDrawingLine)
            {
                //時間
                buttonPressTime = Time.time;
            }
            isDrawingLine = true;
            originalSpeed = speed;
        }

        // 左クリックが離された場合、線を非表示にし、リストをクリア
        if (Input.GetMouseButtonUp(0) || (isDrawingLine && Time.time - buttonPressTime >= playerManager.buttonPressDuration))
        {
            isDrawingLine = false; // 線を描画中でない状態にする
            playerManager.lineRenderer.positionCount = 0; // 線を非表示にする
            speed = originalSpeed; // オリジナルの速度に戻す
            touchedEnemies.Clear(); // リストを空にする
        }

        if (isDrawingLine)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(playerManager.enemyTag); // タグが"Enemy"の敵オブジェクトを取得

            foreach (GameObject enemy in enemies)
            {
                // プレイヤーと敵の距離を計算
                float distance = Vector3.Distance(playerManager.player.position, enemy.transform.position);

                if (distance < playerManager.maxDistance)
                {
                    touchedEnemies.Add(enemy.transform);
                    lastTouchedEnemy = enemy.transform;
                }
                //Debug.Log($"Playerと{enemy.name}の距離: {distance}");
            }

            // 線を描画中かつボタンが押されている場合、線を描画
            // ボタンが押されているときだけ敵との距離をチェック
            if (touchedEnemies.Count > 0)
            {
                // 線の長さを設定
                playerManager.lineRenderer.positionCount = touchedEnemies.Count + 2; // +2 はPlayerと最後に触れた敵への線分を追加

                // Playerから最後に触れた敵への線を描画
                playerManager.lineRenderer.SetPosition(0, transform.position);

                // 最後に触れた敵への線を描画
                if (lastTouchedEnemy != null)
                {
                    Vector3 lastEnemyPosition = lastTouchedEnemy.position;
                    playerManager.lineRenderer.SetPosition(1, lastEnemyPosition);
                }

                // リストから破棄されたオブジェクトを削除しながら線を描画
                int currentIndex = 2; // インデックス2から敵との線を描画
                for (int i = 0; i < touchedEnemies.Count;)
                {
                    Transform enemyTransform = touchedEnemies[i];

                    // Transformオブジェクトが破棄されているかどうかをチェック
                    if (enemyTransform != null)
                    {
                        Vector3 enemyPosition = enemyTransform.position;
                        playerManager.lineRenderer.SetPosition(currentIndex, enemyPosition);
                        currentIndex++;
                        i++; // 次の敵をチェック
                    }
                    else
                    {
                        // 破棄されたオブジェクトをリストから削除
                        touchedEnemies.RemoveAt(i);
                    }
                }

                // 破棄されたオブジェクトをリストから削除した後、余分な頂点をクリア
                for (int i = currentIndex; i < playerManager.lineRenderer.positionCount; i++)
                {
                    playerManager.lineRenderer.SetPosition(i, Vector3.zero);
                }
            }
        }
    }
    //void Update()
    //{
    //    PlayerMove playermove = GetComponent<PlayerMove>();
    //    speed = playermove.moveSpeed;

    //    // 左クリックが押されたかをチェック
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (!isDrawingLine)
    //        {
    //            //時間
    //            buttonPressTime = Time.time;
    //        }
    //        isDrawingLine = true;
    //        playermove.moveSpeed = 50;
    //    }

    //    // 左クリックが離された場合、線を非表示にし、リストをクリア
    //    if (Input.GetMouseButtonUp(0) || (isDrawingLine && Time.time - buttonPressTime >= buttonPressDuration))
    //    {
    //        isDrawingLine = false; // 線を描画中でない状態にする
    //        lineRenderer.positionCount = 0; // 線を非表示にする
    //        playermove.moveSpeed = originalSpeed; // オリジナルの速度に戻す
    //        touchedEnemies.Clear(); // リストを空にする
    //    }

    //    if (isDrawingLine)
    //    {
    //        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // タグが"Enemy"の敵オブジェクトを取得

    //        foreach (GameObject enemy in enemies)
    //        {
    //            // プレイヤーと敵の距離を計算
    //            float distance = Vector3.Distance(player.position, enemy.transform.position);

    //            if (distance < maxDistance)
    //            {
    //                touchedEnemies.Add(enemy.transform);
    //            }
    //            Debug.Log($"Playerと{enemy.name}の距離: {distance}");
    //        }

    //        // 線を描画中かつボタンが押されている場合、線を描画
    //        // ボタンが押されているときだけ敵との距離をチェック
    //        if (touchedEnemies.Count > 0)
    //        {
    //            // 線の長さを設定
    //            lineRenderer.positionCount = touchedEnemies.Count + 1;
    //            lineRenderer.SetPosition(0, transform.position);

    //            // リストから破棄されたオブジェクトを削除しながら線を描画
    //            int currentIndex = 1;
    //            for (int i = 0; i < touchedEnemies.Count;)
    //            {
    //                Transform enemyTransform = touchedEnemies[i];

    //                // Transformオブジェクトが破棄されているかどうかをチェック
    //                if (enemyTransform != null)
    //                {
    //                    Vector3 enemyPosition = enemyTransform.position;
    //                    lineRenderer.SetPosition(currentIndex, enemyPosition);
    //                    currentIndex++;
    //                    i++; // 次の敵をチェック
    //                }
    //                else
    //                {
    //                    // 破棄されたオブジェクトをリストから削除
    //                    touchedEnemies.RemoveAt(i);
    //                }
    //            }

    //            // 破棄されたオブジェクトをリストから削除した後、余分な頂点をクリア
    //            for (int i = currentIndex; i < lineRenderer.positionCount; i++)
    //            {
    //                lineRenderer.SetPosition(i, Vector3.zero);
    //            }
    //        }
    //    }
    //}
   
}
