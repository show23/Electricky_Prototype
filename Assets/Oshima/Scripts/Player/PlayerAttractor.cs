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
        
        

    }
    public void ApplyAttractionForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, playerManager.attractionRange);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 directionToPlayer = transform.position - col.transform.position;
                    float distanceToPlayer = directionToPlayer.magnitude;

                    if (distanceToPlayer > playerManager.stopDistance)
                    {
                        Vector3 attractionDirection = directionToPlayer.normalized;
                        rb.AddForce(attractionDirection * playerManager.attractionForce, ForceMode.Force);

                        if (rb.velocity.magnitude > playerManager.maxSpeed)
                        {
                            rb.velocity = rb.velocity.normalized * playerManager.maxSpeed;
                        }
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;
                    }
                }
            }
        }
    }
    // ギズモを表示するためのメソッド

}
