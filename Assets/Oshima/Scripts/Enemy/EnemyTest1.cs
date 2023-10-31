using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest1 : MonoBehaviour
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

    public Transform player;
    public float fieldOfViewAngle = 60.0f;
    private int currentPatrolPoint = 0;
   
    private Vector3 targetPosition;//Playr�̉ߋ��̈ʒu��ۑ�
    public EnemyState currentState = EnemyState.Patrol;

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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation1, Time.deltaTime * rushSpeed);
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
        Debug.Log("DetectPlayer");

        // �v���C���[�̕���������
        Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rushSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // �v���C���[�̍ŏ��̈ʒu�Ɍ�����
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, rushSpeed * Time.deltaTime);

        // ���B��臒l��ݒ�
        float threshold = 1.0f;

        
        if (Vector3.Distance(transform.position, targetPosition) < threshold)
        {
            currentState = EnemyState.ChasePlayer;
        }

        // �v���C���[�� rushRadius ���������ꍇ�� Patrol ��ԂɑJ��
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseRadius)
        {
            currentState = EnemyState.Patrol;
        }
    }

    void ChasePlayerUpdate()
    {
        Debug.Log("ChasePlayer");

        // �v���C���[�̕���������
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // �v���C���[�Ɍ������Ĉړ�
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (distanceToPlayer > chaseRadius || angleToPlayer > fieldOfViewAngle * 0.5f)
        {

            currentState = EnemyState.Patrol;
        }
    }

    //�ːi����܂�
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
    }
}
