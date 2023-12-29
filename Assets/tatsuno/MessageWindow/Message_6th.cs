using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_6th : MonoBehaviour
{
    public GameObject MessageWindowObj;
    List<string> Message = new List<string>();
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Awake()
    {
        MessageWindowObj.SetActive(true);

        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();

        Message.Add("溜まった電力を使えば、電磁力で壁を走ることもできそうですね！\n" +
            "RBで走りながら壁に飛び込んでみてください！");

        MessageWindow.GetList(Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
