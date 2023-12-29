using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_1st : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("あー、あー、聞こえますか？");
        Message.Add("…うん、聞こえてるみたいですね。\n" +
            "突然ですが、あなたが今いるその街は悪いロボットたちに占拠されてしまったようです。");
        Message.Add("なんとかヘリポートまでたどり着いてくれたら、救助できるんですけど…");
        Message.Add("Lスティックを動かして、歩くことはできそうですか？\n" +
            "RBを押しながら移動すれば走ることもできそうですね。\n" +
            "あ、でもRスティックで周囲の状況を確認しながら慎重に行動してくださいね！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
