using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yosokuen : MonoBehaviour
{
    PlayerManager playerManager;
    // ターゲットとなるオブジェクトのTransformコンポーネントを取得
    private Transform targetTransform;
    private KeyCode hikiyosae;
    private KeyCode attack;
    // public float attackSize = 0.0f;
    private float range;
    //public bool attckOk;
    //public KeyCode attractionKey = KeyCode.E; // 引き寄せるキー
    // オブジェクトの初期サイズと新しいサイズを設定
    private Vector3 initialSize;
    //public Vector3 newSize = new Vector3(2.0f, 2.0f, 2.0f); // 例として2倍の大きさに変更
    public GameObject en;
    private PlayerAttack playerAttack;

    // サイズ変更の速度
    //public float sizeChangeSpeed = 1.0f;

    private bool isResizing = false;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        // ターゲットオブジェクトを名前で検索し、そのTransformを取得
        targetTransform = en.transform;
        playerAttack = GetComponent<PlayerAttack>();
        // ターゲットオブジェクトの初期サイズを保存
        initialSize = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void Update()
    {

        //otamesi = PlayerAttractor.instance.attractionKey;
        attack = playerManager.attackKey;
        range = playerManager.interactionRange * 0.08f;
        if(playerAttack.isAttack2 == true)
        {
            isResizing = true;
        }
        // リセットする場合（初期サイズに戻す）
        else
        {
            isResizing = false;
            targetTransform.localScale = initialSize;
        }

        // ボタンが押されている間にサイズを変更する
        if (isResizing)
        {
            // 新しいサイズに向かって徐々に変更
            targetTransform.localScale = new Vector3(range,range,range);
            //Debug.Log(targetTransform.localScale);

            //if (targetTransform.localScale == newSize)
            //{
            //    attckOk = true;
            //}
        }
    }


}
