using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CammeraRotate : MonoBehaviour
{
    //　キャラクターのTransform
    [SerializeField]
    private Transform charaLookAtPosition;
    //　カメラの移動スピード
    [SerializeField]
    public float cameraMoveSpeed = 2f;
    //　カメラの回転スピード
    [SerializeField]
    public float cameraRotateSpeed = 90f;
    //　カメラのキャラクターからの相対値を指定
    [SerializeField]
    private Vector3 basePos = new Vector3(0f, 0f, 2f);
    // 障害物とするレイヤー
    [SerializeField]
    private LayerMask obstacleLayer;

    void FixedUpdate()
    {
        //　通常のカメラ位置を計算
        var cameraPos = charaLookAtPosition.position + (-charaLookAtPosition.forward * basePos.z) + (Vector3.up * basePos.y);
        //　カメラの位置をキャラクターの後ろ側に移動させる
        transform.position = Vector3.Lerp(transform.position, cameraPos, cameraMoveSpeed * Time.deltaTime);

        RaycastHit hit;
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        if (Physics.Linecast(charaLookAtPosition.position, transform.position, out hit, obstacleLayer))
        {
            transform.position = hit.point;
        }
        //　レイを視覚的に確認
        Debug.DrawLine(charaLookAtPosition.position, transform.position, Color.red, 0f, false);

        //　スピードを考慮しない場合はLookAtで出来る
        //transform.LookAt(charaTra.position);
        //　スピードを考慮する場合
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(charaLookAtPosition.position - transform.position), cameraRotateSpeed * Time.deltaTime);
    }
}
