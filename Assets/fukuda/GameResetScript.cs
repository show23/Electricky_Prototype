using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResetScript : MonoBehaviour
{
    [Header("LeftShift + Enter でリセット")]

    [Header("タイトルシーン名をここに設定してください")]
    
    [SerializeField]
    private string StartSceneName;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("LeftShift"))
        {
            if (Input.GetKeyDown("Enter"))
            {
                SceneManager.LoadScene(StartSceneName);
            }
        }
    }
}
