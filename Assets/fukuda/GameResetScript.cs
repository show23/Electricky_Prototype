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

    [SerializeField]
    private bool useGameKill = true;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("LeftShift"))
        {
            if (Input.GetKeyDown("Enter"))
            {
                if (useGameKill)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
                }
                else
                {
                    SceneManager.LoadScene(StartSceneName);
                }
            }
        }
    }
}
