using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBoxHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerControll_2>())
            {
                other.GetComponent<PlayerControll_2>().CurrentHp = 0;
            }
        }
    }
}
