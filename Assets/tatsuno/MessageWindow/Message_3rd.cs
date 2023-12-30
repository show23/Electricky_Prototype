using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_3rd : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("ってえぇ！？何で2段ジャンプできるんですか！？");
        Message.Add("…動くことで発電できる洋服のおかげ…？");
        Message.Add("すごいです！そんなすごいアイテムがあるならロボットだって倒せそうですね！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
