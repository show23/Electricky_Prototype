using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Life : MonoBehaviour
{
    [SerializeField] protected Slider _slider;
    [SerializeField] protected Image _gaugeImage;
    [SerializeField] protected TextMeshProUGUI _text;
    [SerializeField] protected FadeImage _fadeImage;

    private PlayerControll _playerControl;

    private float _lastEnergy;

    // Start is called before the first frame update
    void Start()
    {
        _playerControl = GameObject.Find("Player").GetComponent<PlayerControll>();
    }

    // Update is called once per frame
    void Update()
    {
        //float energy_now = _playerControl.CurrentEnergy;
        //float energy_max = _playerControl.CurrentMaxEnergy;

        //_fadeImage.Range = 1.0f - (energy_now / energy_max);
    }
}
