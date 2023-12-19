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
        float Life_now = _playerControl.CurrentHp;
        float Life_max = _playerControl.CurrentMaxHp;

        _slider.value = Life_now / Life_max;

        _text.text = Life_now.ToString("f0") + " / " + Life_max.ToString("f0");

        //_fadeImage.Range = 1.0f - (Life_now / Life_max);
    }
}
