using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDamage : MonoBehaviour
{
    private DamageFlash _flash;

    // Start is called before the first frame update
    void Start()
    {
        _flash = GetComponent<DamageFlash>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _flash.CallDamageFlash();
        }
    }
}
