using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shoot : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        ChasePlayer,
        RapidShot,
        ChargeShot
    }
    public GameObject RapidbulletPrefab; // 発射するPrefab

    public float RapidfireInterval = 2.0f; // 発射の間隔（秒）
    public float RapidbulletSpeed = 5.0f; // 弾の速度
    
    public float bulletPointOffset = 0.5f;
    private float lastFireTime = 0.0f;


    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float rushRadius = 5f;
    public float chaseRadius = 10.0f;
    public float minDistanceToPlayer = 5.0f;
    private Transform player;
    public float fieldOfViewAngle = 60.0f;
    private int currentPatrolPoint = 0;

    private Vector3 targetPosition;//Playrの過去の位置を保存
    public EnemyState currentState = EnemyState.Patrol;
    // Start is called before the first frame update

    // Update is called once per frame

    private void Start()
    {
        player = FindObjectOfType<PlayerControll>().transform;
    }
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolUpdate();
                break;


            case EnemyState.ChasePlayer:
                ChasePlayerUpdate();
                break;
        }
    }

    void PatrolUpdate()
    {
        // プレイヤーまでの距離と方向を計算
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 障害物があるか確認
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, rushRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // プレイヤーの方向を向く
                Quaternion targetRotation1 = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation1, Time.deltaTime * moveSpeed);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                // DetectPlayer に遷移
                targetPosition = player.position;
                currentState = EnemyState.ChasePlayer;
                return;
            }
        }

        //プレイヤーが見つからなかった場合の処理
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPatrolPoint].position, moveSpeed * Time.deltaTime);
        Vector3 targetDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 1.1f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
        }
    }
    void ChasePlayerUpdate()
    {
        // プレイヤーの方向を向く
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (distanceToPlayer < minDistanceToPlayer)
        {
            // プレイヤーが近づきすぎた場合、後退する
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        else
        {
            // プレイヤーに向かって移動
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        if (distanceToPlayer > chaseRadius)
        {
            currentState = EnemyState.Patrol;
        }
        if (Time.time - lastFireTime > RapidfireInterval)
        {
            FireProjectile(); 
            lastFireTime = Time.time; 
        }
    }


    void FireProjectile()
    {
        GameObject projectile = Instantiate(RapidbulletPrefab, transform.position, transform.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // プレイヤーの方向に速度を与えつつ、少し上向きにも補正
            rb.velocity = (directionToPlayer + Vector3.up * bulletPointOffset) * RapidbulletSpeed;
        }

        Destroy(projectile, 3.0f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, rushRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);
    }
}
