using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class MessageTest : MonoBehaviour
{
    public FrontObj frontObj;

    void Start()
    {
        List<Action> actList = new List<Action>();
        Action act = async () => {
            frontObj.inputWait(false);
            frontObj.setMessages("test");
            await Task.Delay(1000);
            frontObj.setMessages("test2");
            await Task.Delay(1000);
            frontObj.clearMessage();
        };
        actList.Add(act);
        frontObj.showDialog(actList);
    }
}