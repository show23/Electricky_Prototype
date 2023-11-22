using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Blow : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        DetectPlayer,
        ChasePlayer
    }

    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float rushRadius = 5f;
    public float rushSpeed = 5f;
    public float chaseRadius = 10.0f;

    private Transform player;
    public float fieldOfViewAngle = 60.0f;
    private int currentPatrolPoint = 0;
   
    private Vector3 targetPosition;//Playrの過去の位置を保存
    public EnemyState currentState = EnemyState.Patrol;

    [System.Obsolete]
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

            case EnemyState.DetectPlayer:
                DetectPlayerUpdate();
                break;

            case EnemyState.ChasePlayer:
                ChasePlayerUpdate();
                break;
        }
        //PatrolUpdate();
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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation1, Time.deltaTime * rushSpeed);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                // DetectPlayer に遷移
                StartCoroutine(TransitionToDetectPlayer());
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



    void DetectPlayerUpdate()
    {
        Debug.Log("DetectPlayer");

        // プレイヤーの方向を向く
        Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rushSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // プレイヤーの最初の位置に向かう
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, rushSpeed * Time.deltaTime);

        // 到達の閾値を設定
        float threshold = 1.0f;

        
        if (Vector3.Distance(transform.position, targetPosition) < threshold)
        {
            currentState = EnemyState.ChasePlayer;
        }

        //// プレイヤーが rushRadius よりも遠い場合に Patrol 状態に遷移
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //if (distanceToPlayer > chaseRadius)
        //{
        //    currentState = EnemyState.Patrol;
        //}
    }

    void ChasePlayerUpdate()
    {
        Debug.Log("ChasePlayer");

        // プレイヤーの方向を向く
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // プレイヤーに向かって移動
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (distanceToPlayer > chaseRadius)
        {

            currentState = EnemyState.Patrol;
        }
    }

    //突進するまで
    IEnumerator TransitionToDetectPlayer()
    {
       
        yield return new WaitForSeconds(1.0f); // 1秒待つ

        targetPosition = player.position;
        currentState = EnemyState.DetectPlayer;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, rushRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
