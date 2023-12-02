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
    [SerializeField,Tooltip("連射用の弾")]
    private GameObject RapidbulletPrefab; // 発射するPrefab
    [SerializeField, Tooltip("チャージ用の弾")]
    private GameObject ChargebulletPrefab; // 発射するPrefab

    public float RapidbulletSpeed = 5.0f; // 弾の速度
    public float ChargebulletSpeed = 5.0f; // 弾の速度

    [Tooltip("連射される最大の距離")]
    public float maxFireDistance = 5.0f;
    [Tooltip("連射時に移動モーションが発生する距離")]
    public float FireWalkDistance = 5.0f;
    
    public float BulletAnimationValue = 1.0f;

    private int ChargeTimer = 0;
    [SerializeField]
    private int ChargeTime = 200;


    private Vector3 oldPos;

    //public float bulletPointOffset = 0.5f;

    private bool canonStart = false;
    private bool canonFired = false;
    private bool canonEnd = false;

    [Tooltip("弾がスポーンする位置 空オブジェクトで指定"),SerializeField]
    private List<Transform> BulletSpawnPosition;
    [Tooltip("弾がスポーンする位置 空オブジェクトで指定"),SerializeField]
    private List<Transform> StandBulletSpawnPosition;
    [Tooltip("弾がスポーンする位置 空オブジェクトで指定"),SerializeField]
    private List<Transform> CanonSpawnPosition;


    private int UsePoint = 0;

    private Animator animator;
    public float MoveAnimationValue = 1.0f;

    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float findRadius = 5f;
    public float chaseRadius = 10.0f;
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
        animator = GetComponent<Animator>();
        oldPos = transform.position;
    }
    void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolUpdate();
                break;
            case EnemyState.ChasePlayer:
                ChaseUpdate();
                break;
            case EnemyState.RapidShot:
                RapidShotUpdate();
                break;
            case EnemyState.ChargeShot:
                ChargeShotUpdate();
                break;
        }

        if (currentState != EnemyState.ChargeShot)
            ChargeTimer++;

        Vector2 vector = new Vector2(oldPos.x - transform.position.x, oldPos.z - transform.position.z);
        animator.SetFloat("WalkSpeed", MoveAnimationValue * vector.magnitude);
        animator.SetFloat("Walk_withBullet_Speed", BulletAnimationValue * vector.magnitude);
        oldPos = transform.position;
    }

    void PatrolUpdate()
    {
        // プレイヤーまでの距離と方向を計算
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 障害物があるか確認
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, findRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // プレイヤーの方向を向く
                Quaternion targetRotation1 = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation1, Time.deltaTime * moveSpeed);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

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
    void ChaseUpdate()
    {
        // プレイヤーの方向を向く
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (distanceToPlayer > chaseRadius)
        {
            currentState = EnemyState.Patrol;
        }

        if (distanceToPlayer > maxFireDistance) 
        {
            // プレイヤーに向かって移動
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        
        if (distanceToPlayer < maxFireDistance)
        {
            currentState = EnemyState.RapidShot;
        }

    }
    void RapidShotUpdate()
    {
        // プレイヤーの方向を向く
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (distanceToPlayer > FireWalkDistance)
        {
            // プレイヤーに向かって移動
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        if (ChargeTime < ChargeTimer)
        {
            ChargeTimer = 0;
            UsePoint = 0;
            currentState = EnemyState.ChargeShot;
        }

        animator.SetBool("Bullet", true);
        if (distanceToPlayer > maxFireDistance)
        {
            animator.SetBool("Bullet", false);
            UsePoint = 0;
            currentState = EnemyState.ChasePlayer;
        }
    }
    void ChargeShotUpdate()
    {
        // プレイヤーの方向を向く
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        if (distanceToPlayer < maxFireDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        if (!canonStart)
        {
            animator.SetTrigger("Canon");
        }


        if (canonEnd && canonStart)
        {
            canonStart = false;
            currentState = EnemyState.ChasePlayer;
        }
    }

    public void RapidFire()
    {
        Vector3 shootPos = Vector3.Lerp(BulletSpawnPosition[UsePoint].position, StandBulletSpawnPosition[UsePoint].position, BulletAnimationValue);


        GameObject projectile = Instantiate(RapidbulletPrefab, shootPos, transform.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 directionToPlayer = (player.position - shootPos).normalized;

            // プレイヤーの方向に速度を与えつつ、少し上向きにも補正
            rb.velocity = directionToPlayer * RapidbulletSpeed;
        }

        UsePoint++;

        if (UsePoint > BulletSpawnPosition.Count - 1)
            UsePoint = 0;

        //これ弾の消滅にエフェクトあったほうがいいかも
        //bullet側のスクリプトにちゃんとエフェクト発生させる機構があるならこのままで
        Destroy(projectile, 3.0f);
    }

    public void StartCanon()
    {
        canonStart = true;
        canonFired = false;
        canonEnd = false;
        animator.SetBool("Bullet", false);
        animator.ResetTrigger("Canon");
    }
    public void FireCanon()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject projectile = Instantiate(RapidbulletPrefab, CanonSpawnPosition[i].position, transform.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // プレイヤーの方向に速度を与える
                Vector3 directionToPlayer = (player.position - CanonSpawnPosition[UsePoint].position).normalized;
                rb.velocity = directionToPlayer * ChargebulletSpeed;
            }
        }
        canonFired = true;
    }
    public void EndCanon()
    {
        canonEnd = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, findRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxFireDistance);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, FireWalkDistance);
    }
}
