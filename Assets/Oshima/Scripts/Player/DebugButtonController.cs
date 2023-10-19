using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButtonController : MonoBehaviour
{
    PlayerManager playerManager;
    private Rigidbody s_rigidbody;
    private float _maxHp = 100;     //�ő�̗�
    private float _currentHp = 0; //���݂̗̑�
    private float playerSpeed;
    [SerializeField] private GaugeController _gaugeController;  //�g�������Q�[�W����N���X

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        s_rigidbody = GetComponent<Rigidbody>();
        _gaugeController.UpdateGauge(_currentHp, _maxHp);   //�Q�[�W�������l�ōX�V
    }

    private void Update()
    {
        playerSpeed = new Vector2(s_rigidbody.velocity.x, s_rigidbody.velocity.z).magnitude;

        Debug.Log(playerSpeed);

        HealButtonPush();
    }
    //�_���[�W
    public void DamageSkillHikiyosePush()
    {
        _currentHp -= playerManager.currentHp;
        _gaugeController.UpdateGauge(_currentHp, _maxHp);   //�̗͂���������̃Q�[�W�̌����ڂ��X�V
    }
    public void DamageSkillAAPush()
    {
        _currentHp -= playerManager.AAgauge;
        _gaugeController.UpdateGauge(_currentHp, _maxHp);   //�̗͂���������̃Q�[�W�̌����ڂ��X�V
    }
    //��
    public void HealButtonPush()
    {
        if (_currentHp < _maxHp)
        {
            _currentHp += playerSpeed * playerManager.fillSpeed;
            if (_currentHp > _maxHp) // ���� _currentHp �� _maxHp �𒴂����ꍇ�A_currentHp �� _maxHp �ɐݒ�
            {
                _currentHp = _maxHp;
            }

            _gaugeController.UpdateGauge(_currentHp, _maxHp);   //�̗͂��񕜂�����̃Q�[�W�̌����ڂ��X�V
        }
    }

    public float CurrentHp
    {
        get { return _currentHp; }
        set { _currentHp = value; }
    }
}
