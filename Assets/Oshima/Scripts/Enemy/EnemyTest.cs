using UnityEngine;
using System.Collections;

public class EnemyTest : MonoBehaviour
{
    // ����|�C���g�̔z��ƃv���C���[�̈ʒu����ێ�����ϐ�
    public Transform[] patrolPoints;
    public Transform player;

    // ���񑬓x�ƒǐՑ��x�A���E�̊p�x�A�ǐՊJ�n�����A�ǐՒ�~�����A�G���m�̍ŏ�������ݒ肷��ϐ�
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float chaseDistance = 50.0f;
    public float stopChaseDistance = 60.0f;
    public float fieldOfViewAngle = 60.0f;
    public float minDistanceBetweenEnemies = 5.0f;

    // ���݂̏���|�C���g�̃C���f�b�N�X�A�ǐՒ����ǂ������Ǘ�����t���O�A���x�̈ꎞ�ۑ��p�̕ϐ�
    private int currentPatrolPoint = 0;
    private bool isChasing = false;
    private float oldPatrolSpeed;
    private float oldChaseSpeed;

    // �ǐՊJ�n�̒x�����Ԃ�ݒ肷��ϐ�
    public float chaseDelay = 3.0f;




    private bool firstTimeSeen = true;
    private Vector3 initialPlayerPosition;
    private void Start()
    {
        // ���������ɑ��x�̏����l��ۑ�����
        oldChaseSpeed = chaseSpeed;
        oldPatrolSpeed = patrolSpeed;
    }

    void Update()
    {
        

        // �v���C���[�܂ł̋����ƕ������v�Z
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Vector3 targetPosition = player.position; // �v���C���[�̍ŏ��̈ʒu���擾
        // �f�o�b�O���O�ɏ��񑬓x��\��
        //Debug.Log(initialPlayerPosition);
        Debug.Log(firstTimeSeen);
        if (isChasing)
        {
            // �v���C���[��ǐՒ��̏���
            //if (firstTimeSeen)
            //{
            //    firstTimeSeen = false;
            //    initialPlayerPosition = player.position; // �v���C���[�̈ʒu��ۑ�
            //}
            // �v���C���[�����E���ɓ����Ă���A�ǐՋ������ɂ���ꍇ
            if (distanceToPlayer < chaseDistance && angleToPlayer < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
                {
                    if (hit.transform == player)
                    {
                        // �v���C���[�̕���������
                        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                        //// �v���C���[�Ɍ������Ĉړ�
                        transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
                        // �v���C���[�̍ŏ��̈ʒu�Ɍ�����
                        
                       // transform.position = Vector3.MoveTowards(transform.position, initialPlayerPosition, chaseSpeed * Time.deltaTime);

                    }
                    else
                    {
                        // �v���C���[�������Ȃ��Ȃ����ꍇ�͏���ɖ߂�
                        isChasing = false;
                        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                    }
                }
            }

            // �ǐՒ�~�����𒴂����ꍇ�͏���ɖ߂�
            if (distanceToPlayer > stopChaseDistance)
            {
                isChasing = false;
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            }

            // �G���m�̍ŏ��������Ƀv���C���[������ꍇ�A���x������������
            if (distanceToPlayer < minDistanceBetweenEnemies)
            {
                float slowDownFactor = 0.8f;
                patrolSpeed *= slowDownFactor;
                chaseSpeed *= slowDownFactor;
            }
            else
            {
                // ����ȊO�̏ꍇ�͑��x�����ɖ߂�
                patrolSpeed = oldPatrolSpeed;
                chaseSpeed = oldChaseSpeed;
            }

        }
        else
        {
            
            //firstTimeSeen = true;
            // ���̏���|�C���g�̕���������
            Vector3 targetDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * patrolSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            // �O�i���ď���
            transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);

            // ����|�C���g�ɋ߂Â����ꍇ�͎��̃|�C���g�Ɉڂ�
            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 1.0f)
            {
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            }

            // ��Q��������ꍇ�͔�����i�R�����g�A�E�g����Ă��镔���͏�Q���ɓ��������ꍇ�̏����j
            RaycastHit obstacleHit;
            if (Physics.Raycast(transform.position, targetDirection, out obstacleHit, 1.0f))
            {
                if (obstacleHit.collider.gameObject != patrolPoints[currentPatrolPoint].gameObject)
                {
                    //currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                }
            }

            // �v���C���[�����E���ɓ����Ă���A�ǐՋ������ɂ���ꍇ�͒ǐՂ��J�n
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
        // ���̓G�L�����N�^�[�Ƃ̋������m�F���āA�ŏ��������ɂ���ꍇ�͑��x������������i�R�����g�A�E�g����Ă��镔���͑��̓G�L�����N�^�[�Ƃ̋������m�F���鏈���j
        //foreach (var enemy in FindObjectsOfType<EnemyTest>())
        //{
        //    if (enemy != this)
        //    {
        //        float distanceToOtherEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        //        if (distanceToOtherEnemy < minDistanceBetweenEnemies)
        //        {
        //            float slowDownFactor = 0.8f;
        //            patrolSpeed *= slowDownFactor;
        //            chaseSpeed *= slowDownFactor;
        //        }
        //    }
        //}
    }

    // �M�Y����`�悷�邽�߂̏����i�͈͂�\���j
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
