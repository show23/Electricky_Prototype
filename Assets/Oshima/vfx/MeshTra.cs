using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTra : MonoBehaviour
{
    public float activeTime = 2.0f;
    [Header("Mesh Ralated")]
    public float meshRefreshRate = 0.1f;
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
        Debug.Log("Test" + isTrailActive);       
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
                gObj.AddComponent<MeshRenderer>();
                gObj.AddComponent<MeshFilter>();
            }
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }
}
