using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButtonController : MonoBehaviour
{
    PlayerManager playerManager;
    private float _maxHp = 100;     //最大体力
    private float _currentHp = 0; //現在の体力

    [SerializeField] private GaugeController _gaugeController;  //使いたいゲージ操作クラス

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        _gaugeController.UpdateGauge(_currentHp, _maxHp);   //ゲージを初期値で更新
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            HealButtonPush();
        }
        
    }
    //ダメージ
    public void DamageSkillHikiyosePush()
    {
        _currentHp -= playerManager.currentHp;
        _gaugeController.UpdateGauge(_currentHp, _maxHp);   //体力が減った後のゲージの見た目を更新
    }
    public void DamageSkillAAPush()
    {
        _currentHp -= playerManager.AAgauge;
        _gaugeController.UpdateGauge(_currentHp, _maxHp);   //体力が減った後のゲージの見た目を更新
    }
    //回復
    public void HealButtonPush()
    {
        if (_currentHp < _maxHp)
        {
            _currentHp += playerManager.fillSpeed;
            if (_currentHp > _maxHp) // もし _currentHp が _maxHp を超えた場合、_currentHp を _maxHp に設定
            {
                _currentHp = _maxHp;
            }

            _gaugeController.UpdateGauge(_currentHp, _maxHp);   //体力が回復した後のゲージの見た目を更新
        }
    }

    public float CurrentHp
    {
        get { return _currentHp; }
        set { _currentHp = value; }
    }
}
