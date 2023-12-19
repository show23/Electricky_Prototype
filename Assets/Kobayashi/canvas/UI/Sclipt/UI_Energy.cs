using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Energy : MonoBehaviour
{
    [SerializeField] protected Image _gaugeImage;
    [SerializeField] protected TextMeshProUGUI _text;

    private PlayerControll_2 _playerControl;

    private float _lastEnergy;

    // Start is called before the first frame update
    void Start()
    {
        _playerControl = FindObjectOfType<PlayerControll_2>().GetComponent<PlayerControll_2>();


    }

    // Update is called once per frame
    void Update()
    {
        float energy_now = _playerControl.CurrentEnergy;
        float energy_max = _playerControl.CurrentMaxEnergy;

        if(_lastEnergy !=  energy_now) 
        {
            _gaugeImage.fillAmount = energy_now / energy_max;

            _text.text = energy_now.ToString("F0");
        }

        _lastEnergy  = energy_now;
    }
}
