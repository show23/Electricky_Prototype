using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        DetectPlayer,
        ChasePlayer
    }
    public GameObject bulletPrefab; // ���˂���Prefab
    public float fireInterval = 2.0f; // ���˂̊Ԋu�i�b�j
    public float bulletSpeed = 5.0f; // �e�̑��x
    public float bulletPointOffset = 0.5f;
    private float lastFireTime = 0.0f;
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float rushRadius = 5f;
    //public float rushSpeed = 5f;
    public float chaseRadius = 10.0f;
    public float minDistanceToPlayer = 5.0f;
    public Transform player;
    public float fieldOfViewAngle = 60.0f;
    private int currentPatrolPoint = 0;

    private Vector3 targetPosition;//Playr�̉ߋ��̈ʒu��ۑ�
    public EnemyState currentState = EnemyState.Patrol;
    // Start is called before the first frame update
  
    // Update is called once per frame
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
    }

    void PatrolUpdate()
    {
        // �v���C���[�܂ł̋����ƕ������v�Z
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // ��Q�������邩�m�F
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, rushRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // �v���C���[�̕���������
                Quaternion targetRotation1 = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation1, Time.deltaTime * moveSpeed);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                // DetectPlayer �ɑJ��
                StartCoroutine(TransitionToDetectPlayer());
                return;
            }
        }

        //�v���C���[��������Ȃ������ꍇ�̏���
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
        // �v���C���[�̕���������
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (distanceToPlayer < minDistanceToPlayer)
        {
            // �v���C���[���߂Â��������ꍇ�A��ނ���
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        else
        {
            // �v���C���[�Ɍ������Ĉړ�
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        if (distanceToPlayer > chaseRadius)
        {
            currentState = EnemyState.Patrol;
        }
        if (Time.time - lastFireTime > fireInterval)
        {
            FireProjectile(); 
            lastFireTime = Time.time; 
        }
    }

    void ChasePlayerUpdate()
    {

    }
    void FireProjectile()
    {
        GameObject projectile = Instantiate(bulletPrefab, transform.position, transform.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // �v���C���[�̕����ɑ��x��^���A����������ɂ��␳
            rb.velocity = (directionToPlayer + Vector3.up * bulletPointOffset) * bulletSpeed;
        }

        Destroy(projectile, 3.0f);
    }


    IEnumerator TransitionToDetectPlayer()
    {

        yield return new WaitForSeconds(1.0f); // 1�b�҂�

        targetPosition = player.position;
        currentState = EnemyState.DetectPlayer;
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
