using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlliceSiren : MonoBehaviour
{
    [SerializeField] private GameObject _RedLightObject, _BlueLightObject;
    public float waitTime = .2f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Siren());
    }

    IEnumerator Siren()
    {
        yield return new WaitForSeconds(waitTime);

        _RedLightObject.SetActive(false);
        _BlueLightObject.SetActive(true);

        yield return new WaitForSeconds(waitTime);

        _RedLightObject.SetActive(true);
        _BlueLightObject.SetActive(false);
        StartCoroutine(Siren());
    }
}
