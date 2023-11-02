//using UnityEngine;

//public class Node1 : MonoBehaviour
//{
//    public bool walkable = true; // このノードが移動可能かどうか
//    public Transform[] neighbors; // このノードに隣接するノードたち
//    public int gridX; // グリッド上のX座標
//    public int gridY; // グリッド上のY座標

//    public int gCost; // スタートからの移動コスト
//    public int hCost; // ゴールまでの推定移動コスト
//    public Transform parent; // 経路上の前のノード

//    public int fCost { get { return gCost + hCost; } } // gCostとhCostの合計
//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f); // Nodeの位置に黄色いワイヤーフレームの立方体を描画
//        UnityEditor.Handles.Label(transform.position, $"({gridX}, {gridY})"); // Nodeの位置にラベルを表示
//    }
//    // その他のメソッドやプロパティを追加することもできます
//}
