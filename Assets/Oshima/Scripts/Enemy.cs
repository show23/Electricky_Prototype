using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float detectionRange = 10f; // プレイヤーを検出する範囲
    public float chaseRange = 20f; // プレイヤーを追いかける範囲
    public float retreatRange = 30f; // １の行動に戻る範囲
    public float speed = 10f;
    
    public Transform player; // プレイヤーオブジェクトの参照
    public float stoppingDistance = 0.1f; // 停止距離
    public float detectionAngle = 45.0f; // 検知する角度

    private Vector3 initialPosition; // 最初の位置を保持するための変数
    public float rotationSpeed = 5.0f; // 回転速度（角度を変える速度）
    private float returnSpeed = 20f; // 最初の位置に戻る速度（調整可能）

    private bool playerLook;
    private void Start()
    {
        // 最初の位置を記録する
        initialPosition = transform.position;
    }
    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (player != null)
        {
            // プレイヤーへのベクトルを計算
            Vector3 playerDirection = player.position - transform.position;
            playerDirection.y = 0; // 高さの差を無視

            // 自身の前方ベクトルとプレイヤーへのベクトルの角度を計算
            float angle = Vector3.Angle(transform.forward, playerDirection);

            // 検知角度内にプレイヤーがいるかを確認
            if (angle <= detectionAngle && distanceToPlayer < retreatRange)
            {
                // プレイヤーを検出した場合
                ChasePlayer();
                NotifyOtherEnemies();
            }
            else if (distanceToPlayer > retreatRange)
            {
                // １の行動に戻る場合
                ReturnToBase();
            }
        }
        
        
    }


    void ChasePlayer()
    {
        playerLook = true;
        
        // プレイヤーが視界内に入れば全員追いかける
        Vector3 playerDirection = player.position - transform.position;
        playerDirection.y = 0; // 高さの差を無視

        // プレイヤーの方向を向く
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 敵の移動を制御する場合は、NavMeshを使用したり、以下のようにTransformを操作できます。
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // ここでアニメーションを制御するコードを追加する
       
        // プレイヤーとの距離が一定以下になったら攻撃などの行動を追加する
        if (Vector3.Distance(transform.position, player.position) < 2f)
        {
            // 攻撃などの行動を追加する
        }
    }

    void ReturnToBase()
    {
        // 目標位置への方向ベクトルを計算
        Vector3 direction = initialPosition - transform.position;

        // 目標位置に十分に近づいたら移動を停止
        if (direction.magnitude <= stoppingDistance)
        {
            // もし目標位置に到達したら、位置を正確に設定して微動を防ぐ
            transform.position = initialPosition;
        }
        else
        {
            // 方向ベクトルを正規化して速度をかける
            Vector3 velocity = direction.normalized * speed * Time.deltaTime;

            // 現在位置から目標位置に向かって少しずつ移動させる
            transform.position += velocity;
        }
    }

    void NotifyOtherEnemies()
    {
        // 他のエネミーにプレイヤーが検出されたことを通知
        foreach (Enemy otherEnemy in FindObjectsOfType<Enemy>())
        {
            if (otherEnemy != this)
            {
                otherEnemy.playerLook = true;
                otherEnemy.ChasePlayer();
            }
        }
    }
    private void OnDrawGizmos()
    {

       
        // ギズモの色を設定
        Gizmos.color = Color.black;

        // ギズモの範囲を表示
        Gizmos.DrawWireSphere(transform.position, retreatRange);
    }
}


