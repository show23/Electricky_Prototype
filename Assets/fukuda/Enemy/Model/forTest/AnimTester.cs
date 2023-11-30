using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTester : MonoBehaviour
{

    private Animator _animator;

    [SerializeField]
    private int Animation;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _animator.SetInteger("Count", Animation);
    }
}
