using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class test_SaveSystem : MonoBehaviour
{
    [SerializeField] private SaveSystem _saveSystem;
    [SerializeField] private TextMeshProUGUI _textNow;
    [SerializeField] private TextMeshProUGUI _textRank;


    //private rankData testDataTime;


    // Start is called before the first frame update
    void Start()
    {

        _saveSystem.Save(1, 55f);

        Debug.Log(_saveSystem.BestData.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
