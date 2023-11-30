using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove_ByFukuda_3 : MonoBehaviour
{
    //　キャラクターのTransform
    private Transform charaLookAtPosition;
    //　カメラの移動スピード
    [SerializeField]
    private float cameraMoveSpeed = 2f;
    //　カメラの回転追従スピード
    [SerializeField]
    private float cameraRotateSpeed = 90f;

    //　カメラの回転スピード
    [SerializeField,Tooltip("プレイ中の変更は不可能なので軌道前に変更してください")]
    private Vector2 RotateSpeed = new Vector2(0.2f, 0.1f);
    
    //　カメラのキャラクターからの相対値を指定
    [SerializeField]
    private Vector3 basePos = new Vector3(0f, 0f, 2f);
    [SerializeField]
    private Vector3 LookOffset = new Vector3(0f, 1.0f, 0f);
    // 障害物とするレイヤー
    [SerializeField]
    private LayerMask obstacleLayer;

    private void Start()
    {
        charaLookAtPosition = FindObjectOfType<ObjectPositionCopy>().transform;
        charaLookAtPosition.GetComponent<ObjectPositionCopy>().setRotSpeed = RotateSpeed;

        transform.position = charaLookAtPosition.position + (-charaLookAtPosition.forward * basePos.z) + (Vector3.up * basePos.y);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(charaLookAtPosition.position - transform.position), cameraRotateSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        //charaLookAtPosition.GetComponent<ObjectPositionCopy>().setRotSpeed = RotateSpeed;
        //　通常のカメラ位置を計算
        var cameraPos = charaLookAtPosition.position + (-charaLookAtPosition.forward * basePos.z) + (Vector3.up * basePos.y);

        //　カメラの位置をキャラクターの後ろ側に移動させる
        transform.position = Vector3.Lerp(transform.position, cameraPos, cameraMoveSpeed * Time.deltaTime);

        RaycastHit hit;
        Vector3 lookPos = charaLookAtPosition.position + LookOffset;
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        if (Physics.Linecast(lookPos, transform.position, out hit, obstacleLayer))
        {
            transform.position = hit.point;
        }
        //　レイを視覚的に確認
        Debug.DrawLine(lookPos, transform.position, Color.red, 0f, false);

        //　スピードを考慮しない場合はLookAtで出来る
        //transform.LookAt(charaTra.position);
        //　スピードを考慮する場合
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), cameraRotateSpeed * Time.deltaTime);
    }
}
