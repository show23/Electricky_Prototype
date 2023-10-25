using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    
   
    [Header("▽Player")]
    [CustomLabel("歩くスピード")]
    public float moveSpeed = 30f;
    [CustomLabel("走るスピード")]
    public float dashSpeed = 50f;

    [CustomLabel("通常攻撃に必要なコスト(最大100)")]
    public float AAgauge = 10;
    [CustomLabel("通常攻撃レンジ")]
    public float beamRange = 10;
    [CustomLabel("通常攻撃速度")]
    public float beamSpeed = 10;
    [Header("▽UIゲージ")]
    [CustomLabel("移動によって溜まる量")]
    public float fillSpeed = 1f;



    [Header("▽引き寄せ攻撃")]
    [CustomLabel("引き寄せスキルに必要なコスト(最大100)")]
    public float currentHp = 80;

    
    [CustomLabel("円が広がるスピード")]
    public float increasedSpeed = 10.0f; // キーが押されている間に拡大する範囲
    [CustomLabel("最大の円の大きさ")]
    public float maxInteractionRange = 10.0f; // 最大許容距離
    [CustomLabel("引き寄せる範囲")]
    public float attractionRange = 10f;     // 引き寄せる範囲
    [CustomLabel("引き寄せる力")]
    public float attractionForce = 5f;      // 引き寄せる力
    [CustomLabel("引き寄せる最大速度")]
    public float maxSpeed = 10f;            // 最大速度
    [CustomLabel("停止距離")]
    public float stopDistance = 2f;         // 停止距離
   

    [Header("▽線引く攻撃")]
    [CustomLabel("Enemyのタグ名")]
    public string enemyTag = "Enemy"; // タグ名を指定
    [CustomLabel("ボタンを押せる時間")]
    public float buttonPressDuration = 5f; // ボタンを押せる最大時間（秒）
    [CustomLabel("PlayerとEnemyの判定距離")]
    public float maxDistance = 5f;
    
    [Header("触らない")]
    public Camera mainCamera; // メインカメラの参照
   
    public Transform player; // プレイヤーのTransformを格納するための変数
    public LineRenderer lineRenderer;

   
    public Transform firePoint;   // ビームの発射位置
    public float interactionRange = 0.0f; // PlayerとEnemyの間の許容距離


   
    private void OnDrawGizmos()
    {
        // ギズモの色を設定（例: グリーン）
        Gizmos.color = Color.green;

        // ギズモの範囲を表示
        Gizmos.DrawWireSphere(transform.position, attractionRange);

        // ギズモの停止距離を表示
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        // ギズモの色を設定
        Gizmos.color = Color.blue;

        // ギズモの範囲を表示
        Gizmos.DrawWireSphere(transform.position, maxDistance);
        // ギズモの色を設定
        Gizmos.color = Color.yellow;

        // ギズモの範囲を表示
        Gizmos.DrawWireSphere(transform.position, beamRange);

    }
}
