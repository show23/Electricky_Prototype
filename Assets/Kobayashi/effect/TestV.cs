using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TestV : MonoBehaviour
{
    [SerializeField] VisualEffect effect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            effect.SendEvent("OnPlay");
            //OnPlayはEvent Nameで指定した任意の名称
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            effect.SendEvent("OnStop");
            //StopPlayはEvent Nameで指定した任意の名称
        }
    }
}
