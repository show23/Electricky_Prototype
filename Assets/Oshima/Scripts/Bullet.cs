using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static Bullet instance;
    private Transform target; // �r�[���̒ǔ��Ώ�
    public float beamSpeed = 100f; // �r�[���̑��x
    public float curveStrength = 1f; // �J�[�u�̋��x

    private float curveDirectionUp = 0; // �J�[�u�����𐧌䂷��ϐ� (0: ��, 1: ��, 2: �E, 3: ������J�[�u)
    private float curveDirectionRight = 0;
    private float curveDirectionLeft = 0;
    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    // �G�̒ǐՑΏۂ�ݒ�
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Start()
    {
        // �J�[�u�����������_���ɑI��
        curveDirectionUp = Random.Range(0.0f, 3.0f);
        curveDirectionRight = Random.Range(-3.0f, 3.0f);
        curveDirectionLeft = Random.Range(-3.0f, 3.0f);

    }

    void Update()
    {
        // �ǔ��Ώۂ����݂���ꍇ
        if (target != null)
        {
            // �r�[���̕�����ݒ�i�ǔ��j
            Vector3 direction = (target.position - transform.position).normalized;

            // �J�[�u��K�p
            Vector3 curve = Vector3.zero;
            curve = new Vector3(curveDirectionLeft, curveDirectionUp, curveDirectionRight) * curveStrength;
           

            GetComponent<Rigidbody>().velocity = (direction + curve) * beamSpeed;
        }
    }
}
