using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove_ByFukuda : MonoBehaviour
{
    //　キャラクターのTransform
    [SerializeField]
    private Transform TargetObj;
    //　カメラの移動スピード
    [SerializeField]
    private float cameraMoveSpeed = 2f;
    //　カメラの回転スピード
    [SerializeField]
    private float cameraRotateSpeed = 90f;
    //　カメラのキャラクターからの距離を指定
    [SerializeField]
    private float Length = 3.0f;
    // 障害物とするレイヤー
    [SerializeField]
    private LayerMask obstacleLayer;

    private InputAction cam, camReset;
    public Vector2 CameraInput;
    private PlayerInput playerInput;


    [SerializeField, Range(1, 5)]
    private float CamAddValueX;
    [SerializeField, Range(1, 5)]
    private float CamAddValueY;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        camReset = playerInput.actions["CameraReset"];
        cam = playerInput.actions["CameraXY"];
        transform.position = TargetObj.position + (-TargetObj.forward * Length);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetObj.position - transform.position), cameraRotateSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        //カメラ入力を更新
        CameraInput = cam.ReadValue<Vector2>();

        bool CameraReset = camReset.ReadValue<float>() > 0;
      
        CameraInput.x *= CamAddValueX;
        CameraInput.y *= CamAddValueY;


        Vector3 origin = TargetObj.position;
        transform.RotateAround(TargetObj.position, Vector3.up, CameraInput.x);
        Vector3 self = transform.position;

        Vector3 tVec = self - origin;
        tVec =tVec.normalized;

        transform.position = TargetObj.position + tVec * Length;

        RaycastHit hit;
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        if (Physics.Linecast(TargetObj.position, transform.position, out hit, obstacleLayer))
        {
            transform.position = hit.point;
        }
        //　レイを視覚的に確認
        Debug.DrawLine(TargetObj.position, transform.position, Color.red, 0f, false);

        //　スピードを考慮しない場合はLookAtで出来る
        //transform.LookAt(charaTra.position);
        //　スピードを考慮する場合
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetObj.position - transform.position), cameraRotateSpeed * Time.deltaTime);
    }

    void OnReset()
    {
        var cameraPos = TargetObj.position + (-TargetObj.forward * Length);
        //　カメラの位置をキャラクターの後ろ側に移動させる
        transform.position = Vector3.Lerp(transform.position, cameraPos, cameraMoveSpeed * Time.deltaTime);
    }
}
