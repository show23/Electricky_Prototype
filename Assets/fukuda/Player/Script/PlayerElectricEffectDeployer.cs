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
    private PlayerControll_2 player_2;
    private void Start()
    {
        player = GetComponent<PlayerControll>();
        player_2 = GetComponent<PlayerControll_2>();
        timer = 0;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float developValue = 0;
        if (player)
            developValue = Mathf.Lerp(minDev, maxDev, player.CurrentEnergy / player.CurrentMaxEnergy);
        if (player_2)
            developValue = Mathf.Lerp(minDev, maxDev, player_2.CurrentEnergy / player_2.CurrentMaxEnergy);


        if (timer > 60 - (int)developValue)
        {
            if (Electric)
                Instantiate(Electric, transform.position, transform.rotation);
            timer = 0;
        }else
        timer++;
    }
}
