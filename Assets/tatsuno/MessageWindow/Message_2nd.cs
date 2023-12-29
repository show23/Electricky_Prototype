using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_2nd : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("道を塞ぐ障害物はAボタンで飛び越えてください。\n" +
            "それでも届かなかったら…2段ジャンプでもできればいいんですけど…");
        
        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
