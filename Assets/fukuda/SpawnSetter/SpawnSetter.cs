using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSetter : MonoBehaviour
{
    private bool isUsed = false;


    private void OnTriggerEnter(Collider other)
    {
        if (!isUsed)
        {
            if (other.tag == "Player")
            {
                if (other.GetComponent<PlayerControll_2>())
                {
                    Transform childTransform = this.GetComponentInChildren<Transform>();


                    other.GetComponent<PlayerControll_2>().CurrentSpawnPoint = childTransform.position;
                    other.GetComponent<PlayerControll_2>().CurrentSpawnRotation = childTransform.rotation;
                    isUsed = true;
                }
            }
        }
    }
}
