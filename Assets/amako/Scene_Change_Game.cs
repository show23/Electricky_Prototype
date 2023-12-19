using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Change_Game : MonoBehaviour
{
    private Timer_decimal_TMP timeUI;

    // Start is called before the first frame update
    void Start()
    {
        timeUI = FindObjectOfType<Timer_decimal_TMP>().GetComponent<Timer_decimal_TMP>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            timeUI.StopTime();
            SceneManager.LoadScene("Result");
        }
    }
}
