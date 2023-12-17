using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFrash : MonoBehaviour
{
    public GameObject startLight;
    Light lightInstance;
    // Start is called before the first frame update
    void Start()
    {
        lightInstance = startLight.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Frash(0f, 1f));
    }

    private void LightIntensity(float intenceAmount)
    {
        lightInstance.intensity = intenceAmount;
    }

    IEnumerator Frash(float intenceAmount, float intenceAmount2)
    {
        LightIntensity(intenceAmount);
        yield return new WaitForSeconds(0.8f);
        LightIntensity(intenceAmount2);
    }
}
