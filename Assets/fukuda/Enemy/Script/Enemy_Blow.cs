using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Blow : MonoBehaviour
{
    [System.Serializable]
    public enum EnemyState
    {
        Patrol,
        Chase,
        Punch,
        Bodyblow,
        Destroyed
    }


    [System.Serializable]
    public enum BodyBlowState
    {
        Start,
        Blow,
        End
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
        public GameObject Punch;
        public GameObject RushStart;

        [HideInInspector]
        public GameObject RushEffect;
    }

    [Tooltip("ここに 効果音とエフェクトがセットになった\nプレハブを入れてください"), CustomLabel("効果音 エフェクト類"), SerializeField]
    private SE_VFX_PrefabList SE_VFX_Prefabs;

    public float CurrentHp
    {
        get { return HP; }
        set
        {
            if(value < HP)
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



    [SerializeField]
    private Transform[] patrolPoints;
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float findRadius = 5f;
    [SerializeField]
    private float findSpeed = 5f;



    [SerializeField]
    private float punchDistance = 1.0f;
    [SerializeField]
    private float punchHitRadius = 1.0f;
    [SerializeField]
    private float punchDamage = 10.0f;
    [SerializeField]
    private float punchSpeed = 1.0f;


    private int bodyBlowTimer = 0;

    [SerializeField]
    private int bodyBlowTime = 90;

    [SerializeField]
    private float bodyBlowStartDistance = 1.0f;
    [SerializeField]
    private float bodyBlowBiginAngle = 5.0f;
    [SerializeField]
    private float bodyBlowHitRadius = 1.0f;
    [SerializeField]
    private float bodyBlowSpeed = 1.0f;
    [SerializeField]
    private float bodyBlowDamage = 20.0f;


    private float bodyBlowLength = 0.0f;
    private Vector3 bodyBlowStartpos;

    [SerializeField]
    private float hitKnockback = 7.5f;

    //当たり判定があるかどうか
    private bool bodyBlow_hasHit = false;
    //すでにプレイヤーが当たったかどうか
    private bool bodyBlow_isHit = false;
    private bool bodyBlow_Start = false;
    private bool bodyBlow_End = false;

    private bool punch_Using = false;

    [SerializeField]
    private BodyBlowState bodyBlowState;

    [SerializeField]
    private string PlayerTag = "Player";

    [SerializeField]
    private float chaseRadius = 10.0f;

    private Transform player;
    [SerializeField]
    private float fieldOfViewAngle = 60.0f;
    private int currentPatrolPoint = 0;

    private Vector3 targetPosition;//Playrの過去の位置を保存
    [SerializeField] EnemyState currentState = EnemyState.Patrol;
    private Animator _animator;

    [SerializeField]
    private float MoveAnimationValue = 55;

    private Vector3 oldPos;



    private Rigidbody rigidbody;

    [System.Obsolete]
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("isAlive", true);
        _animator.SetFloat("WalkSpeed", 0.0f);

        player = FindObjectOfType<PlayerControll>().transform;

        HP = maxHP;
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

            case EnemyState.Chase:
                ChasePlayerUpdate();
                break;

            case EnemyState.Punch:
                PunchUpdate();
                break;

            case EnemyState.Bodyblow:
                BodyBlowUpdate();
                break;
            case EnemyState.Destroyed:
                DestroyedUpdate();
                break;
        }



        Vector2 vector = new Vector2(oldPos.x - transform.position.x, oldPos.z - transform.position.z);
        _animator.SetFloat("WalkSpeed", MoveAnimationValue * vector.magnitude);
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
            _animator.SetBool("isAlive", false);
            _animator.SetInteger("DeathPattern", Random.Range(0, 2));
            Destroy(this.gameObject, DeleteTime);

            if (SE_VFX_Prefabs.Destroyed)
                Instantiate(SE_VFX_Prefabs.Destroyed, transform.position, transform.rotation);
        }
        oldDestroy = true;
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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation1, Time.deltaTime * findSpeed);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                targetPosition = player.position;
                currentState = EnemyState.Chase;

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


    void ChasePlayerUpdate()
    {
        bodyBlowTimer++;

        // プレイヤーの方向を向く
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // プレイヤーに向かって移動
        rigidbody.velocity = (transform.forward * moveSpeed);

        if (distanceToPlayer > chaseRadius)
        {
            currentState = EnemyState.Patrol;

            if (SE_VFX_Prefabs.LoseSight)
                Instantiate(SE_VFX_Prefabs.LoseSight, transform.position, transform.rotation);
        }

        if (distanceToPlayer < punchDistance &&
            !punch_Using)
            currentState = EnemyState.Punch;

        if (Mathf.Abs(angleToPlayer) < bodyBlowBiginAngle &&
            bodyBlowTime < bodyBlowTimer)
        {
            currentState = EnemyState.Bodyblow;
            bodyBlowTimer = 0;
        }
    }

    void BodyBlowUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));


        switch (bodyBlowState)
        {
            case BodyBlowState.Start:
                if (!bodyBlow_Start)
                {
                    _animator.SetTrigger("RushBegin");
                    bodyBlow_isHit = false;
                    bodyBlow_End = false;
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                break;
            case BodyBlowState.Blow:

                rigidbody.velocity = (transform.forward * bodyBlowSpeed);

                bool isWallDitect = false;
                //前方にEnemyではないオブジェクトがある場合 突進の中止
                float raycastDistance = 5.0f;
                RaycastHit Hit;
                if (Physics.Raycast(transform.position, transform.forward, out Hit, raycastDistance))
                {
                    if (Hit.collider.tag != "Ememy" && Hit.collider.tag != "Player" && Hit.collider.tag != "Bullet")
                        isWallDitect = true;
                }
                Vector3 vec1 = transform.position;
                Vector3 vec2 = player.position;

                vec1.y = 0;
                vec2.y = 0;

                if (bodyBlow_hasHit)
                {
                    if (bodyBlowLength < Vector3.Distance(vec1, vec2)
                        || isWallDitect || bodyBlow_isHit)
                    {
                        bodyBlow_hasHit = false;

                        if (punchDistance > distanceToPlayer)
                        {
                            bodyBlow_Start = false;
                            currentState = EnemyState.Punch;
                            bodyBlowState = BodyBlowState.Start;
                            bodyBlowTimer = 0;
                        }
                        else
                        {
                            Debug.Log("EnemyRushEnd");
                            _animator.SetTrigger("RushEnd");
                        }
                    }
                }

                //当たり判定

                if (!bodyBlow_isHit)
                {
                    if (bodyBlow_hasHit)
                        bodyBlow_isHit = AttackHitCheck(bodyBlowHitRadius, bodyBlowDamage, hitKnockback);
                }
                break;
            case BodyBlowState.End:
                if (bodyBlow_End)
                {
                    bodyBlow_Start = false;
                    //Debug.Log("End BodyBlow");
                    currentState = EnemyState.Chase;
                    bodyBlowState = BodyBlowState.Start;
                    _animator.ResetTrigger("RushBegin");
                    bodyBlowTimer = 0;
                }
                break;
        }

    }

    public void RushStart()
    {
        bodyBlow_Start = true;
    }

    public void RushHitStart()
    {
        bodyBlow_hasHit = true;
        bodyBlowStartpos = transform.position;
        Vector3 vec1 = transform.position;
        Vector3 vec2 = player.position;

        vec1.y = 0;
        vec2.y = 0;

        bodyBlowLength = Vector3.Distance(vec1, vec2) + 2.0f;
        bodyBlowState = BodyBlowState.Blow;

        if (SE_VFX_Prefabs.RushStart)
           SE_VFX_Prefabs.RushEffect = Instantiate(SE_VFX_Prefabs.RushStart, transform.position, transform.rotation);
    }


    public void RushHitEnd()
    {

        if (SE_VFX_Prefabs.RushEffect)
            DestroyImmediate(SE_VFX_Prefabs.RushEffect);
        SE_VFX_Prefabs.RushEffect = null;
        //Debug.Log("Animator Trigger BodyBlow Hit");
        bodyBlow_hasHit = false;
        bodyBlowState = BodyBlowState.End;
    }

    public void RushEnd()
    {
        //Debug.Log("Animator Trigger BodyBlow");
        bodyBlow_End = true;
    }



    void PunchUpdate()
    {
        if (!punch_Using)
        {
            _animator.SetTrigger("Punch");
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        rigidbody.velocity = (transform.forward * punchSpeed);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }


    public void PunchStart()
    {
        punch_Using = true;
    }
    public void PunchHit()
    {
        //当たり判定-------------
        AttackHitCheck(punchHitRadius, punchDamage, hitKnockback);


        if (SE_VFX_Prefabs.Punch)
            Instantiate(SE_VFX_Prefabs.Punch, transform.position, transform.rotation);
    }

    public void PunchEnd()
    {
        currentState = EnemyState.Chase;
        punch_Using = false;
    }
    

    public void FootStep()
    {

        if (SE_VFX_Prefabs.FootStep)
            Instantiate(SE_VFX_Prefabs.FootStep, transform.position, transform.rotation);
    }


    bool AttackHitCheck(float range, float damage, float knockback)
    {
        bool isHit = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(PlayerTag))
            {
                Vector3 direction = Vector3.Normalize(Vector3.Scale(col.transform.position - transform.position, new Vector3(1, 0, 1)));

                //ここで敵の体力を減らす処理をする
                //ダメージ量はdamageから使ってください
                GameObject player = col.gameObject;

                float HP = player.GetComponent<PlayerControll>().CurrentHp;

                player.GetComponent<PlayerControll>().CurrentHp -= damage;


                if (HP != player.GetComponent<PlayerControll>().CurrentHp)
                {
                    player.GetComponent<Rigidbody>().AddForce(direction * knockback, ForceMode.Impulse);

                    Debug.Log("Player Damaged (damage " + damage + " by BlowEnemy )");
                    isHit = true;
                }
                break; //プレイヤーは1人しかいないのでbreak
            }
        }

        return isHit;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, findRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
