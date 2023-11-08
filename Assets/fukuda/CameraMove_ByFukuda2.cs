using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove_ByFukuda2 : MonoBehaviour
{
    [SerializeField]
    private Vector2 localAngle;

    //　キャラクターのTransform
    [SerializeField]
    private Transform TargetObj;

    [SerializeField]
    private Vector3 OffsetPos;
    
    private Vector3 CamLocalPos;

    //　カメラのキャラクターからの距離を指定
    [SerializeField]
    private float Length = 3.0f;
    
    // 障害物とするレイヤー
    [SerializeField]
    private LayerMask obstacleLayer;

    private InputAction cam, camReset;
    public Vector2 CameraInput;
    public bool isResetCamera;
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

        CamLocalPos = new Vector3(0, 0.4f, -1).normalized * Length;
        transform.position = TargetObj.position + OffsetPos + CamLocalPos;
        transform.rotation = Quaternion.LookRotation(TargetObj.position - transform.position);
    }

    void FixedUpdate()
    {
        //カメラ入力を更新
        CameraInput = cam.ReadValue<Vector2>();

        bool CameraReset = camReset.ReadValue<float>() > 0;

        Vector2 CamIn = new Vector2(CameraInput.x * CamAddValueX, CameraInput.y * CamAddValueY);

        localAngle += CamIn;

        Quaternion PosQ = Quaternion.LookRotation(CamLocalPos.normalized, Vector3.up);






        RaycastHit hit;
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        if (Physics.Linecast(TargetObj.position, transform.position, out hit, obstacleLayer))
        {
            transform.position = hit.point;
        }
        //　レイを視覚的に確認
        Debug.DrawLine(TargetObj.position, transform.position, Color.red, 0f, false);

        //　スピードを考慮しない場合はLookAtで出来る
        transform.LookAt(TargetObj.position);
    }

    void OnReset()
    {
        //　カメラの位置をキャラクターの後ろ側に移動させる
        transform.position = TargetObj.position + OffsetPos + (-TargetObj.forward * Length);
    }
}
