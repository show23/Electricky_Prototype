using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_8th : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("真下にロボットがいますね…");
        Message.Add("いっそこの高さから落下しながら攻撃するのはどうですか？\n" +
            "空中でXボタンを押してください！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
