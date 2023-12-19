using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_nextscene : MonoBehaviour
{
    bool nextscenecheck = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextScene()
    {
        if(!nextscenecheck)
        {
            SceneManager.LoadScene("Main_Scene");
            nextscenecheck = true;
        }
    }
}
