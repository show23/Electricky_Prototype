using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameLight : MonoBehaviour
{
    private Light _light;
    public float multiplier = 1000.0f;
    public float fadeSpeed = 50.0f;
    
    // Start is called before the first frame update 
    private void Start()
    {
        _light = GetComponent<Light>();
    }

    // Update is called once per frame
    private void Update()
    {
        _light.intensity = (float)Random.Range(1, 5) * multiplier;
        multiplier -= fadeSpeed * Time.deltaTime;
    }
}
