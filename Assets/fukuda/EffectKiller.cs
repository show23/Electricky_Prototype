using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectKiller : MonoBehaviour
{
    [SerializeField,Tooltip("プレハブの生存時間")]
    private float LifeTime = 2.0f;

    public void OnCall()
    {
        Destroy(this.gameObject, LifeTime);
    }
}
