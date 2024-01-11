using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTra : MonoBehaviour
{
    private bool isUse = false;



    public bool Use
    {
        get { return isUse; }
        set { isUse = value; }
    }



    //public float activeTime = 2.0f;
    [Header("Mesh Ralated")]
    public float meshRefreshRate = 0.1f;

    private float meshTimer = 0.0f;

    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Ralated")]
    public Material mat;
    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    private void FixedUpdate()
    {
        if (isUse)
        {
            meshTimer += Time.deltaTime;

            if (meshTimer >= meshRefreshRate)
            {
                meshTimer -= meshRefreshRate;

                if (skinnedMeshRenderers == null)
                    skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    GameObject gObj = new GameObject();

                    gObj.transform.position = skinnedMeshRenderers[i].transform.position;
                    gObj.transform.rotation = skinnedMeshRenderers[i].transform.rotation;


                    MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                    MeshFilter mf = gObj.AddComponent<MeshFilter>();

                    Mesh mesh = new Mesh();
                    skinnedMeshRenderers[i].BakeMesh(mesh);
                    mf.mesh = mesh;

                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


                    Material[] mats = skinnedMeshRenderers[i].materials;
                    for (int t = 0; t < skinnedMeshRenderers[i].materials.Length; t++)
                    {
                        mats[t] = mat;
                    }
                    mr.materials = mats;

                    Destroy(gObj, meshDestroyDelay);
                }
            }
        }
    }
}
