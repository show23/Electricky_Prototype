using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_9th : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("この先に進むためには…建物をぐるぐる回りながら上に行きましょう。\n" +
            "壁を走りながらLスティックを左右に傾けたら上下に動けそうですね！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
