using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElePos : MonoBehaviour
{
    public VisualEffect vfxGraph;
    public Transform targetObject; // 特定の物体のTransformを指定

    private void Update()
    {
        if (vfxGraph != null && targetObject != null)
        {
            // 特定の物体の位置を取得
            Vector3 objectPosition = targetObject.position;

            // VFX Graphのパラメータに位置情報をセット
            vfxGraph.SetVector3("ObjectPosition", objectPosition);
        }
    }
}
