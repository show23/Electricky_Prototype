using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowDlg : MonoBehaviour
{
    private List<string> messages = new List<string>();
    public TextMeshProUGUI Dlgtext;
    private const int MAX_LINE = 3;
    private List<string> dispMassage = new List<string>();

    public void setMessages(List<string> _messages)
    {
        messages.AddRange(_messages);
        while (messages.Count > 0)
        {
            addMessage();
        }
    }

    private void addMessage()
    {
        if (dispMassage.Count == MAX_LINE)
        {
            dispMassage.RemoveAt(0);
            dispMassage.Add(messages[0]);
        }
        else
        {
            dispMassage.Add(messages[0]);
        }
        Dlgtext.text = string.Join("\n", dispMassage.ToArray());
        messages.RemoveAt(0);
    }

    public void clearMessage()
    {
        messages = new List<string>();
        dispMassage = new List<string>();
        Dlgtext.text = "";
    }
}