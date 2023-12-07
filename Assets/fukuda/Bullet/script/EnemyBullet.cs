using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float Damage = 0.0f;
    public float KnockBackPower = 0.0f;

    [Tooltip("エフェクト等をここに置くと良き"),CustomLabel("オブジェクト破壊時に出現するオブジェクト")]
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
            other.GetComponent<PlayerControll>().CurrentHp -= Damage;
            Vector3 vector = Vector3.Scale(other.transform.position - transform.position, new Vector3(1,0,1)).normalized;
            other.GetComponent<Rigidbody>().AddForce(KnockBackPower * vector, ForceMode.Impulse);

            Debug.Log("Player Damaged (damage " + Damage + " by Bullet )");
        }

        if (other.tag != "Bullet" && other.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
