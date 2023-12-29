using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_7th : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("流石です！まるで忍者ですね！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
