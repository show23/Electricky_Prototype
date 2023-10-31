using UnityEngine;
using System.Collections;

public class EnemyTest : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;

    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float chaseDistance = 50.0f;
    public float stopChaseDistance = 60.0f;
    public float fieldOfViewAngle = 60.0f;
    public float minDistanceBetweenEnemies = 5.0f;

    private int currentPatrolPoint = 0;
    private bool isChasing = false;
    private float oldPatrolSpeed;
    private float oldChaseSpeed;
    private void Start()
    {
        oldChaseSpeed = chaseSpeed;
        oldPatrolSpeed = patrolSpeed;
    }
    void Update()
    {
        Debug.Log(patrolSpeed);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (isChasing)
        {
            if (distanceToPlayer < chaseDistance && angleToPlayer < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
                {
                    if (hit.transform == player)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                        transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
                    }
                    else
                    {
                        isChasing = false;
                        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                    }
                }
            }

            if (distanceToPlayer > stopChaseDistance)
            {
                isChasing = false;
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            }
           
          
            if (distanceToPlayer < minDistanceBetweenEnemies)
            {
                float slowDownFactor = 0.8f;
                patrolSpeed *= slowDownFactor;
                chaseSpeed *= slowDownFactor;
            }
            else
            {
                patrolSpeed = oldPatrolSpeed;
                chaseSpeed = oldChaseSpeed;
                    
            }

        }
        else
        {
            Vector3 targetDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * patrolSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 1.0f)
            {
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            }
            RaycastHit obstacleHit;

            if (Physics.Raycast(transform.position, targetDirection, out obstacleHit, 1.0f))
            {
                if (obstacleHit.collider.gameObject != patrolPoints[currentPatrolPoint].gameObject)
                {

                    //currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                }

            }

            if (distanceToPlayer < chaseDistance && angleToPlayer < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
                {
                    if (hit.transform == player)
                    {
                        isChasing = true;
                    }
                }
            }
        }
    }

    void CheckDistances()
    {
        //foreach (var enemy in FindObjectsOfType<EnemyTest>())
        //{
        //    if (enemy != this)
        //    {
        //        float distanceToOtherEnemy = Vector3.Distance(transform.position, enemy.transform.position);

        //        if (distanceToOtherEnemy < minDistanceBetweenEnemies)
        //        {
        //            float slowDownFactor = 0.8f; // “KØ‚È’l‚É’²®‚µ‚Ä‚­‚¾‚³‚¢
        //            patrolSpeed *= slowDownFactor;
        //            chaseSpeed *= slowDownFactor;
        //        }
        //    }
        //}

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
