using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Energy : MonoBehaviour
{
    [SerializeField] protected Image _gaugeImage;
    [SerializeField] protected TextMeshProUGUI _text;

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
        /*電気ゲージは頻繁に変化するし常に更新する方針で良いか
          更新時に前フレームと変化あるか見る
        */

        float energy_now = _playerControl.CurrentEnergy;
        float energy_max = _playerControl.CurrentMaxEnergy;

        if(_lastEnergy ==  energy_now) 
        {
            _gaugeImage.fillAmount = energy_now / energy_max;


            _text.text = energy_now.ToString("F0");
        }

        _lastEnergy  = energy_now;
    }
}
