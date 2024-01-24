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
        ChargeShot,
        Destroyed
    }


    //ゲーム内で変更される系のステータス
    [SerializeField, Tooltip("体力")]
    private float HP = 100;
    [SerializeField, Tooltip("最大体力")]
    private float maxHP = 100;

    [SerializeField]
    private bool isDestroyed = false;
    private bool oldDestroy = false;

    [SerializeField]
    private float DeleteTime = 6.0f;

    [System.Serializable]
    public struct SE_VFX_PrefabList
    {
        public GameObject Find;
        public GameObject LoseSight;
        public GameObject FootStep;
        public GameObject Destroyed;
        public GameObject Damaged;
        public GameObject ObjectDelete;
        [Space(10)]
        public GameObject RapidShot;
        public GameObject ChargeShoot;
    }

    [Tooltip("ここに 効果音とエフェクトがセットになった\nプレハブを入れてください"), SerializeField]
    private SE_VFX_PrefabList SE_VFX_Prefabs;

    public float CurrentHp
    {
        get { return HP; }
        set
        {
            if (value < HP)
            {
                Vector3 pos = transform.position;
                pos.y += 1.3f;
                pos += (transform.forward * 0.6f);

                if (SE_VFX_Prefabs.Damaged)
                    Instantiate(SE_VFX_Prefabs.Damaged, pos, transform.rotation);
            }

            HP = value;
            if (HP > maxHP)
                HP = maxHP;
            if (HP <= 0)
            {
                //エネミー死亡
                HP = 0;
                if (!isDestroyed)
                    UpdatePlayerDestroyCount();
                isDestroyed = true;
            }
        }
    }

    public float CurrentMaxHp
    {
        get { return maxHP; }
        set
        {
            float oldmaxhp = maxHP;
            maxHP = value;

            //最大体力が増えた場合は増えた分体力も増える
            if (oldmaxhp < value)
            {
                CurrentHp += value - oldmaxhp;
            }
        }
    }



    [SerializeField,Tooltip("連射用の弾")]
    private GameObject RapidbulletPrefab; // 発射するPrefab
    [SerializeField, Tooltip("チャージ用の弾")]
    private GameObject ChargebulletPrefab; // 発射するPrefab

    public float RapidbulletSpeed = 5.0f; // 弾の速度
    public float RapidbulletDamage = 5.0f;
    public float RapidbulletKnockBack = 5.0f;

    public float ChargebulletSpeed = 5.0f; // 弾の速度
    public float ChargebulletDamage = 40.0f;
    public float ChargebulletKnockBack = 15.0f;


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

    private Animator _animator;
    public float MoveAnimationValue = 1.0f;

    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float findRadius = 5f;
    public float chaseRadius = 10.0f;
    private Transform player;
    private Transform playerTarget;
    public float fieldOfViewAngle = 60.0f;
    private int currentPatrolPoint = 0;

    private Vector3 targetPosition;//Playrの過去の位置を保存
    public EnemyState currentState = EnemyState.Patrol;
    private Rigidbody rigidbody;






    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerControll_2>().transform;
        playerTarget = FindObjectOfType<PlayerTargetMarker>().transform;

        _animator = GetComponent<Animator>();
        oldPos = transform.position;

    }
    void FixedUpdate()
    {
        if (isDestroyed)
            currentState = EnemyState.Destroyed;

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
            case EnemyState.Destroyed:
                DestroyedUpdate();
                break;
        }



        Vector2 vector = new Vector2(oldPos.x - transform.position.x, oldPos.z - transform.position.z);
        _animator.SetFloat("WalkSpeed", MoveAnimationValue * vector.magnitude);
        _animator.SetFloat("Walk_withBullet_Speed", BulletAnimationValue * vector.magnitude);
        oldPos = transform.position;
    }
    private void OnDestroy()
    {
        if (SE_VFX_Prefabs.ObjectDelete)
            Instantiate(SE_VFX_Prefabs.ObjectDelete, transform.position, transform.rotation);
    }

    void DestroyedUpdate()
    {
        if (!oldDestroy)
        {
            _animator.SetBool("isAlive",false);
            _animator.SetInteger("DeathPattern", Random.Range(0, 2));
            Destroy(this.gameObject, DeleteTime);

            if (SE_VFX_Prefabs.Destroyed)
                Instantiate(SE_VFX_Prefabs.Destroyed, transform.position, transform.rotation);
        }
        oldDestroy = true;
    }



    private void UpdatePlayerDestroyCount()
    {
        PlayerControll_2 pControll = FindObjectOfType<PlayerControll_2>();
        pControll.CurrentDestroyEnemy += 1;
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


                if (SE_VFX_Prefabs.Find)
                    Instantiate(SE_VFX_Prefabs.Find, transform.position, transform.rotation);

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

            if (SE_VFX_Prefabs.LoseSight)
                Instantiate(SE_VFX_Prefabs.LoseSight, transform.position, transform.rotation);
        }

        if (distanceToPlayer > maxFireDistance) 
        {
            // プレイヤーに向かって移動
            rigidbody.velocity = (transform.forward * moveSpeed);
        }
        
        if (distanceToPlayer < maxFireDistance)
        {
            currentState = EnemyState.RapidShot;
        }

    }
    void RapidShotUpdate()
    {
        ChargeTimer++;

        // プレイヤーの方向を向く
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (distanceToPlayer > FireWalkDistance)
        {
            // プレイヤーに向かって移動
            rigidbody.velocity = (transform.forward * moveSpeed);
        }

        if (ChargeTime < ChargeTimer)
        {
            ChargeTimer = 0;
            UsePoint = 0;
            currentState = EnemyState.ChargeShot;
        }

        _animator.SetBool("Bullet", true);
        if (distanceToPlayer > maxFireDistance)
        {
            _animator.SetBool("Bullet", false);
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
            _animator.SetTrigger("Canon");
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
        
        Vector3 directionToPlayer = (playerTarget.position - shootPos).normalized;

        // プレイヤーの方向に速度を与えつつ、少し上向きにも補正
        rb.velocity = directionToPlayer * RapidbulletSpeed;


        if (SE_VFX_Prefabs.RapidShot)
            Instantiate(SE_VFX_Prefabs.RapidShot, shootPos, transform.rotation);


        EnemyBullet bullet = projectile.GetComponent<EnemyBullet>();
        if (bullet)
        {
            bullet.Damage = RapidbulletDamage;
            bullet.KnockBackPower = RapidbulletKnockBack;
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
        _animator.SetBool("Bullet", false);
        _animator.ResetTrigger("Canon");
    }
    public void FireCanon()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject projectile = Instantiate(ChargebulletPrefab, CanonSpawnPosition[i].position, transform.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            // プレイヤーの方向に速度を与える
            Vector3 directionToPlayer = (playerTarget.position - CanonSpawnPosition[UsePoint].position).normalized;
            rb.velocity = directionToPlayer * ChargebulletSpeed;

            EnemyBullet bullet = projectile.GetComponent<EnemyBullet>();
            
            bullet.Damage = ChargebulletDamage;
            bullet.KnockBackPower = ChargebulletKnockBack;


            if (SE_VFX_Prefabs.ChargeShoot)
                Instantiate(SE_VFX_Prefabs.ChargeShoot, CanonSpawnPosition[i].position, transform.rotation);
            Destroy(projectile, 3.0f);
        }
        canonFired = true;
    }
    public void EndCanon()
    {
        canonEnd = true;
    }


    public void FootStep()
    {

        if (SE_VFX_Prefabs.FootStep)
            Instantiate(SE_VFX_Prefabs.FootStep, transform.position, transform.rotation);
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
