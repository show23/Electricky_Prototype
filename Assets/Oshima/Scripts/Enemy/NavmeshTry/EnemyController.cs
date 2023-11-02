using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public NavigationSystem navigationSystem; // ナビゲーションシステムを参照するための変数
    public Transform target; // 目的地を設定するためのTransform

    void Start()
    {
        // ナビゲーションシステムの参照を取得
        navigationSystem = FindObjectOfType<NavigationSystem>();
    }

    void Update()
    {
        // ナビゲーションシステムを使って移動する処理
        List<Vector3> path = navigationSystem.FindPath(transform.position, target.position);

        if (path != null && path.Count > 0)
        {
            Vector3 nextPosition = path[0];
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * 5);

            if (Vector3.Distance(transform.position, nextPosition) < 0.001f)
            {
                path.RemoveAt(0);
            }
        }
    }
}
