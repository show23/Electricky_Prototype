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
            //OnPlayÇÕEvent NameÇ≈éwíËÇµÇΩîCà”ÇÃñºèÃ
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            effect.SendEvent("OnStop");
            //StopPlayÇÕEvent NameÇ≈éwíËÇµÇΩîCà”ÇÃñºèÃ
        }
    }
}
