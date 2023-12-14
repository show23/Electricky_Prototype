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
            //OnPlay��Event Name�Ŏw�肵���C�ӂ̖���
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            effect.SendEvent("OnStop");
            //StopPlay��Event Name�Ŏw�肵���C�ӂ̖���
        }
    }
}
