using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTra : MonoBehaviour
{
    public float activeTime = 2.0f;
    [Header("Mesh Ralated")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Ralated")]
    public Material mat;
    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.I) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActiveteTrail(activeTime));
        }
             
    }
    IEnumerator ActiveteTrail(float timeActive)
    {
        while(timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
               MeshRenderer mr= gObj.AddComponent<MeshRenderer>();
               MeshFilter mf =  gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;
                mr.material = mat;

                Destroy(gObj, meshDestroyDelay);
            }
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }
}
