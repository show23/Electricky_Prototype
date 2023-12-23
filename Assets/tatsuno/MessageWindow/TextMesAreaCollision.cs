using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMesAreaCollision : MonoBehaviour
{
    bool AreaCollision = false;

    public GameObject Self;
    public GameObject MessageWindowObj;
    Messagewindow MessageWindow;

    // Start is called before the first frame update
    void Start()
    {
        MessageWindow = MessageWindowObj.GetComponent<Messagewindow>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AreaCollision = true;
            MessageWindow.GetTextAreaCollision(AreaCollision);
            Self.SetActive(false);
        }
    }
}
