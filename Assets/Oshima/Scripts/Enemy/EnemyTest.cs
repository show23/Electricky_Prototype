using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;

    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float chaseDistance = 50.0f;
    public float stopChaseDistance = 360.0f * Mathf.Deg2Rad;
    public float fieldOfViewAngle = 60.0f;

    private int currentPatrolPoint = 0;
    private bool isChasing = false;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (isChasing)
        {
            if (distanceToPlayer < chaseDistance && angleToPlayer < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance) && hit.collider.CompareTag("Player"))
                {
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                    transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
                }
            }

            if (distanceToPlayer > stopChaseDistance)
            {
                isChasing = false;
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
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

            if (distanceToPlayer < chaseDistance && angleToPlayer < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance) && hit.collider.CompareTag("Player"))
                {
                    isChasing = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
