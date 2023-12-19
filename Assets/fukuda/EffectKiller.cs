using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectKiller : MonoBehaviour
{
    [SerializeField,Tooltip("プレハブの生存時間")]
    private float LifeTime = 2.0f;

    private void Awake()
    {
        Destroy(this.gameObject, LifeTime);
    }
}
