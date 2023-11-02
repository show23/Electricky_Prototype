using UnityEngine;
using System.Collections;

public class EnemyTest : MonoBehaviour
{
    // 巡回ポイントの配列とプレイヤーの位置情報を保持する変数
    public Transform[] patrolPoints;
    public Transform player;

    // 巡回速度と追跡速度、視界の角度、追跡開始距離、追跡停止距離、敵同士の最小距離を設定する変数
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float chaseDistance = 50.0f;
    public float stopChaseDistance = 60.0f;
    public float fieldOfViewAngle = 60.0f;
    public float minDistanceBetweenEnemies = 5.0f;

    // 現在の巡回ポイントのインデックス、追跡中かどうかを管理するフラグ、速度の一時保存用の変数
    private int currentPatrolPoint = 0;
    private bool isChasing = false;
    private float oldPatrolSpeed;
    private float oldChaseSpeed;

    // 追跡開始の遅延時間を設定する変数
    public float chaseDelay = 3.0f;




    private bool firstTimeSeen = true;
    private Vector3 initialPlayerPosition;
    private void Start()
    {
        // 初期化時に速度の初期値を保存する
        oldChaseSpeed = chaseSpeed;
        oldPatrolSpeed = patrolSpeed;
    }

    void Update()
    {
        

        // プレイヤーまでの距離と方向を計算
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Vector3 targetPosition = player.position; // プレイヤーの最初の位置を取得
        // デバッグログに巡回速度を表示
        //Debug.Log(initialPlayerPosition);
        Debug.Log(firstTimeSeen);
        if (isChasing)
        {
            // プレイヤーを追跡中の処理
            //if (firstTimeSeen)
            //{
            //    firstTimeSeen = false;
            //    initialPlayerPosition = player.position; // プレイヤーの位置を保存
            //}
            // プレイヤーが視界内に入っており、追跡距離内にいる場合
            if (distanceToPlayer < chaseDistance && angleToPlayer < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
                {
                    if (hit.transform == player)
                    {
                        // プレイヤーの方向を向く
                        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                        //// プレイヤーに向かって移動
                        transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
                        // プレイヤーの最初の位置に向かう
                        
                       // transform.position = Vector3.MoveTowards(transform.position, initialPlayerPosition, chaseSpeed * Time.deltaTime);

                    }
                    else
                    {
                        // プレイヤーが見えなくなった場合は巡回に戻る
                        isChasing = false;
                        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                    }
                }
            }

            // 追跡停止距離を超えた場合は巡回に戻る
            if (distanceToPlayer > stopChaseDistance)
            {
                isChasing = false;
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            }

            // 敵同士の最小距離内にプレイヤーがいる場合、速度を減少させる
            if (distanceToPlayer < minDistanceBetweenEnemies)
            {
                float slowDownFactor = 0.8f;
                patrolSpeed *= slowDownFactor;
                chaseSpeed *= slowDownFactor;
            }
            else
            {
                // それ以外の場合は速度を元に戻す
                patrolSpeed = oldPatrolSpeed;
                chaseSpeed = oldChaseSpeed;
            }

        }
        else
        {
            
            //firstTimeSeen = true;
            // 次の巡回ポイントの方向を向く
            Vector3 targetDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * patrolSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            // 前進して巡回
            transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);

            // 巡回ポイントに近づいた場合は次のポイントに移る
            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 1.0f)
            {
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            }

            // 障害物がある場合は避ける（コメントアウトされている部分は障害物に当たった場合の処理）
            RaycastHit obstacleHit;
            if (Physics.Raycast(transform.position, targetDirection, out obstacleHit, 1.0f))
            {
                if (obstacleHit.collider.gameObject != patrolPoints[currentPatrolPoint].gameObject)
                {
                    //currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                }
            }

            // プレイヤーが視界内に入っており、追跡距離内にいる場合は追跡を開始
            if (distanceToPlayer < chaseDistance && angleToPlayer < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
                {
                    if (hit.transform == player)
                    {
                        isChasing = true;
                    }
                }
            }
        }
    }

    void CheckDistances()
    {
        // 他の敵キャラクターとの距離を確認して、最小距離内にいる場合は速度を減少させる（コメントアウトされている部分は他の敵キャラクターとの距離を確認する処理）
        //foreach (var enemy in FindObjectsOfType<EnemyTest>())
        //{
        //    if (enemy != this)
        //    {
        //        float distanceToOtherEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        //        if (distanceToOtherEnemy < minDistanceBetweenEnemies)
        //        {
        //            float slowDownFactor = 0.8f;
        //            patrolSpeed *= slowDownFactor;
        //            chaseSpeed *= slowDownFactor;
        //        }
        //    }
        //}
    }

    // ギズモを描画するための処理（範囲を表示）
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
