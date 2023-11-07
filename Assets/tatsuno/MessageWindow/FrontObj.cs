using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Threading.Tasks;
public class FrontObj : MonoBehaviour, IPointerClickHandler
{
    public Image img;
    public WindowDlg dlg;
    private List<Action> act;
    private Boolean waitFlg;
    void Start()
    {
        img = this.GetComponent<Image>();
        img.raycastTarget = false;
        //waitFlg = false;
    }

    void Update()
    {
    }

    public void setMessages(List<string> _messages)
    {
        dlg.setMessages(_messages);
    }

    public void setMessages(string _message)
    {
        List<string> _messages = new List<string>();
        _messages.Add(_message);
        dlg.setMessages(_messages);
    }

    public void clearMessage()
    {
        dlg.clearMessage();
        if (act.Count == 0)
        {
            Time.timeScale = 1f;
            img.raycastTarget = false;
            dlg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
    }

    public void inputWait(Boolean _waitFlg)
    {
        waitFlg = _waitFlg;
    }

    public async void showDialog(List<Action> _act)
    {
        Time.timeScale = 0f;
        img.raycastTarget = true;
        dlg.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);
        act = _act;
        act[0].Invoke();
        act.RemoveAt(0);
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (img.raycastTarget)
        {
            if (act.Count > 0 && waitFlg)
            {
                act[0].Invoke();
                act.RemoveAt(0);
            }
            if (act.Count == 0)
            {
                dlg.clearMessage();
            }
        }
    }
}