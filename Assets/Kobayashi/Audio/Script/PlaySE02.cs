using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySE02 : MonoBehaviour
{
    [SerializeField] private AudioClip sounces01;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        float rnd = Random.Range(0.5f, 1.5f);
        audioSource.pitch = rnd;
        audioSource.PlayOneShot(sounces01);
    }
}
