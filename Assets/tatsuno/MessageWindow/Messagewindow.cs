using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Messagewindow : MonoBehaviour
{
    public GameObject MessageWindow;
    public TextMeshProUGUI MessageText;
    List<string> Message = new List<string>();
    public float WindowChangeSec = 2;
    bool HideMessageWindow = false;
    float seconds = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        MessageText.text = Message[0];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMessage();
    }

    void UpdateMessage()
    {
        seconds += Time.deltaTime;

        if(seconds >= WindowChangeSec)
        {
            if (Message.Count > 1)
            {
                Message.RemoveAt(0);
                MessageText.text = Message[0];
            }
            
            seconds = 0;
        }

        if (HideMessageWindow)
        {
            MessageWindow.SetActive(false);
            clearMessage();
        }

        //if (!HideMessageWindow)
        //{
        //    //Message.RemoveAt(0);
        //    //MessageText.text = Message[0];
        //}
        //else
        //{
        //    MessageWindow.SetActive(false);
        //    clearMessage();
        //}

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
