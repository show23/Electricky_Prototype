using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickingLight : MonoBehaviour
{
    [SerializeField]
    private Light lightSource;
    [SerializeField]
    private float min, max, intensityMultiplier, flidkerInterval;

    private void OnEnable()
    {
        StartCoroutine(flicker(flidkerInterval));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator flicker(float interval)
    {
        while(interval > 0)
        {
            yield return new WaitForSeconds(interval);
            lightSource.intensity = Random.Range(min, max) * intensityMultiplier;
        }
        while(interval <= 0)
        {
            yield return null;
            lightSource.intensity = Random.Range(min, max) * intensityMultiplier;
        }   
    }
}
