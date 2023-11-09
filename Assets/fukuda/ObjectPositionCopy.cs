using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class ObjectPositionCopy : MonoBehaviour
{
    private Transform tracePosition;
    
    private float RotationSpeedX = 1.0f;
    private float RotationSpeedY = 1.0f;


    public Vector2 setRotSpeed
    {
        get { return new Vector2(RotationSpeedX, RotationSpeedY); }
        set
        {
            Vector2 a = value;
            RotationSpeedX = a.x;
            RotationSpeedY = a.y;
        }
    }


    private PlayerInput playerInput;

    private InputAction cameraXY, cameraReset;



    private bool oldResetInput = false;
    

    private void Start()
    {
        tracePosition = FindObjectOfType<PlayerControll>().transform;
        playerInput = GetComponent<PlayerInput>();
        cameraXY = playerInput.actions["CameraXY"];
        cameraReset = playerInput.actions["CameraReset"];


        transform.rotation = tracePosition.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 CameraInput = cameraXY.ReadValue<Vector2>();

        transform.rotation = Quaternion.LookRotation(transform.forward + transform.right * CameraInput.x * RotationSpeedX + transform.up * CameraInput.y * RotationSpeedY);

        bool ResetInput = cameraReset.ReadValue<float>() > 0;
        bool ResetTrigger = false;
        if (ResetInput && !oldResetInput)
        {
            ResetTrigger = true;
        }

        if (ResetTrigger)
        {
            transform.rotation = tracePosition.rotation;
        }


        transform.position = tracePosition.position;
        oldResetInput = ResetInput;
    }
}
