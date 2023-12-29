using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_4th : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("ちょうど良いところに敵を発見しましたね。\n" +
            "Xボタンでやっちゃってください！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
