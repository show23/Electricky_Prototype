using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    DebugButtonController debugButtonController;
    PlayerManager playerManager;
    PlayerAttractor playerAttractor;
    public bool isAttack = false;
    public bool isAttack2 = false;
    private bool oldIsAttack = false;

    public GameObject beamPrefab;

    [System.Obsolete]
    private void Start()
    {
        debugButtonController = FindObjectOfType<DebugButtonController>();
        playerManager = GetComponent<PlayerManager>();
        playerAttractor = GetComponent<PlayerAttractor>();
    }

    private void Update()
    {
        if (isAttack2 == true && debugButtonController.CurrentHp >= playerManager.currentHp)
        {
            playerAttractor.ApplyAttractionForce();
            playerManager.interactionRange += playerManager.increasedSpeed * Time.deltaTime;
            playerManager.interactionRange = Mathf.Min(playerManager.interactionRange, playerManager.maxInteractionRange);
        }

        if (isAttack2 == false && oldIsAttack == true && debugButtonController.CurrentHp >= playerManager.currentHp)
        {
            InteractWithEnemy();
            playerManager.interactionRange = 0.0f;
            debugButtonController.DamageSkillHikiyosePush();
        }

        if (isAttack == true && debugButtonController.CurrentHp >= playerManager.AAgauge)
        {
            GameObject nearestEnemy = FindNearestEnemyInCamera();

            if (nearestEnemy != null && !IsObstacleBetweenPlayerAndEnemy(nearestEnemy))
            {
                debugButtonController.DamageSkillAAPush();
                ShootBeam(nearestEnemy);
            }
        }

        isAttack = false;
        oldIsAttack = isAttack2;
        isAttack2 = false;
    }

    void InteractWithEnemy()
    {
        Vector3 playerPosition = transform.position;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(playerPosition, enemy.transform.position);

            if (distance <= playerManager.interactionRange)
            {
                Destroy(enemy);
            }
        }
    }

    GameObject FindNearestEnemyInCamera()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerManager.mainCamera);

        foreach (GameObject enemy in enemies)
        {
            if (GeometryUtility.TestPlanesAABB(planes, enemy.GetComponent<Collider>().bounds))
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestEnemy = enemy;
                    nearestDistance = distance;
                }
            }
        }

        return nearestEnemy;
    }

    void ShootBeam(GameObject target)
    {
        GameObject beamInstance = Instantiate(beamPrefab, playerManager.firePoint.position, playerManager.firePoint.rotation);
        Bullet beamScript = beamInstance.GetComponent<Bullet>();
        beamScript.SetTarget(target.transform);
        Destroy(beamInstance, 3f);
    }

    bool IsObstacleBetweenPlayerAndEnemy(GameObject enemy)
    {
        Vector3 playerPosition = transform.position;
        Vector3 targetPosition = enemy.transform.position;

        // Ray�̎n�_��������ɂ��炵�A�����O���ɂ��ړ�������
        Vector3 rayStart = playerPosition + Vector3.up * 2.1f + transform.forward * 2.1f;

        Vector3 rayDirection = (targetPosition - rayStart).normalized;

        Debug.Log("���C�̕���: " + rayDirection);

        // Ray�̎n�_����I�_�܂ł̃��C����ԐF�ŕ`�悵�܂�
        Debug.DrawLine(rayStart, targetPosition, Color.red, 2f);

        RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Ignore Raycast")); // Ignore Raycast���C���[�𖳎�����

        if (Physics.Raycast(rayStart, rayDirection, out hit, Vector3.Distance(rayStart, targetPosition), layerMask))
        {
            Debug.Log("���C�� " + hit.collider.gameObject.name + " �ɓ�����܂���");

            if (hit.collider.gameObject != enemy)
            {
                Debug.Log("��Q�������o����܂���");
                return true;
            }
        }

        return false;
    }




}
