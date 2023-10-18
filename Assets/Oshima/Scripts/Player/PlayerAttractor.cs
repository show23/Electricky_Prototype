using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttractor : MonoBehaviour
{
    PlayerManager playerManager;
    //public KeyCode attractionKey = KeyCode.Q; // 引き寄せるキー
    private KeyCode attack;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }
    private void FixedUpdate()
    {
        attack = playerManager.attackKey;
        if (Input.GetKey(attack))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, playerManager.attractionRange);

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    Rigidbody rb = col.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // プレイヤーから敵への方向ベクトルと距離を計算
                        Vector3 directionToPlayer = transform.position - col.transform.position;
                        float distanceToPlayer = directionToPlayer.magnitude;

                        // 引き寄せる力を適用
                        if (distanceToPlayer > playerManager.stopDistance)
                        {
                            Vector3 attractionDirection = directionToPlayer.normalized;
                            rb.AddForce(attractionDirection * playerManager.attractionForce, ForceMode.Force);

                            // 速度を制限して最大速度に収束させる
                            if (rb.velocity.magnitude > playerManager.maxSpeed)
                            {
                                rb.velocity = rb.velocity.normalized * playerManager.maxSpeed;
                            }
                        }
                        else
                        {
                            // 停止距離内にいる場合、力をゼロに設定して停止
                            rb.velocity = Vector3.zero;
                        }
                    }
                }
            }
        }
    }

    // ギズモを表示するためのメソッド
    
}
