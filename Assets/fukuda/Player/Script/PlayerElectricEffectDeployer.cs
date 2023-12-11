using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElectricEffectDeployer : MonoBehaviour
{
    [SerializeField]
    private GameObject Electric;
   
    [SerializeField, Tooltip("0-60の間で設定してください"), Range(0, 60)]
    private int minDev = 0;

    [SerializeField,Tooltip("0-60の間で設定してください"),Range(0,60)]
    private int maxDev = 20;


    private int timer = 0;

    private PlayerControll player;
    private void Start()
    {
        player = GetComponent<PlayerControll>();
        timer = 0;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float developValue = Mathf.Lerp(minDev, maxDev, player.CurrentEnergy / player.CurrentMaxEnergy);

        if (timer > 60 - (int)developValue)
        {
            if (Electric)
                Instantiate(Electric, transform.position, transform.rotation);
            timer = 0;
        }else
        timer++;
    }
}
