using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class test_SaveSystem : MonoBehaviour
{
    [SerializeField] private SaveSystem _saveSystem;
    [SerializeField] private TextMeshProUGUI _textNow;

    [SerializeField] private TextMeshProUGUI[] _rankTime;
    [SerializeField] private TextMeshProUGUI[] _numName;

    //private rankData testDataTime;


    // Start is called before the first frame update
    void Start()
    {

        //_saveSystem.Save(1, 49f, 0);

        _saveSystem.Load();

        int minute = _saveSystem.ThisData.minute;
        float seconds = _saveSystem.ThisData.seconds;
        float timedecimal = seconds * 100 - (int)seconds * 100;

        _textNow.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)timedecimal).ToString("00");

        for (int i = 0; i < _rankTime.Length; i++)
        {
            minute = _saveSystem.BestData[i].minute;
            seconds = _saveSystem.BestData[i].seconds;
            timedecimal = seconds * 100 - (int)seconds * 100;

            _rankTime[i].text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)timedecimal).ToString("00");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
