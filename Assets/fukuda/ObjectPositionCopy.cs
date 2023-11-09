using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class ObjectPositionCopy : MonoBehaviour
{
    [SerializeField]
    private Transform tracePosition;
    
    [SerializeField,Range(1,10)]
    private float RotationSpeedX = 1.0f;
    [SerializeField, Range(1, 10)]
    private float RotationSpeedY = 1.0f;

    private PlayerInput playerInput;

    private 
        InputAction cameraXY, cameraReset;



    private bool oldResetInput = false;
    

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        cameraXY = playerInput.actions["CameraXY"];
        cameraReset = playerInput.actions["CameraReset"];
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
