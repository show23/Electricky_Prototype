using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_10th : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("素晴らしい！！\n" +
            "目標地点まであと少しですよ！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
