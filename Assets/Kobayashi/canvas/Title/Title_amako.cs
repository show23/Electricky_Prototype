using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Title_amako : MonoBehaviour
{
    [SerializeField] VideoPlayer video;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(video.time >= 0.5f)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        
    }
}
