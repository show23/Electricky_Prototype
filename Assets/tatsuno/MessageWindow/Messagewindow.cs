﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Messagewindow : MonoBehaviour
{
    public GameObject MessageWindow;
    public TextMeshProUGUI MessageText;
    List<string> Message = new List<string>();
    public float WindowChangeSec = 1;
    bool HideMessageWindow = false;

    // Start is called before the first frame update
    void Start()
    {
        //MessageWindow.SetActive(true);
        //Message.Add("test");
        //Message.Add("test2");
        //Message.Add("test3");
        MessageText.text = Message[0];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMessage();
    }

    void UpdateMessage()
    {
        if (!HideMessageWindow)
        {
            //Message.RemoveAt(0);
            //MessageText.text = Message[0];
        }
        else
        {
            MessageWindow.SetActive(false);
            clearMessage();
        }

        //if (Message.Count > 1)
        //{
        //    Message.RemoveAt(0);
        //    MessageText.text = Message[0];
        //}
        //else
        //{
        //    MessageWindow.SetActive(false);
        //    clearMessage();
        //}
    }

    public void GetList(List<string> list)
    {
        Message = list;
    }

    public void GetTextAreaCollision(bool AreaCollision)
    {
        HideMessageWindow = AreaCollision;
    }

    void clearMessage()
    {
        Message = new List<string>();
        MessageText.text = "";
    }
}
