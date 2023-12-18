using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float Damage = 0.0f;
    public float KnockBackPower = 0.0f;

    [Tooltip("エフェクト等をここに置くと良き")]
    public GameObject onDestroySpawnObject;

    private void OnDestroy()
    {
        if (onDestroySpawnObject)
            Instantiate(onDestroySpawnObject, transform.position, transform.rotation);
    }

    private void Start()
    {
        this.gameObject.tag = "Bullet";
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            float HP = other.GetComponent<PlayerControll_2>().CurrentHp;
            other.GetComponent<PlayerControll_2>().CurrentHp -= Damage;

            //ヒットしてるかどうかリターンする という処理のことを忘れていたので
            //こっち側だけで解決することとした
            if (HP != other.GetComponent<PlayerControll_2>().CurrentHp)
            {
                Vector3 vector = Vector3.Scale(other.transform.position - transform.position, new Vector3(1, 0, 1)).normalized;
                other.GetComponent<Rigidbody>().AddForce(KnockBackPower * vector, ForceMode.Impulse);

                Debug.Log("Player Damaged (damage " + Damage + " by Bullet )");

                Destroy(gameObject);
            }
        }
        else
        if (other.tag != "Bullet" && other.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
