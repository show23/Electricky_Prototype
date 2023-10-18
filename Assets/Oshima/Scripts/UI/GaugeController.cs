using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GaugeController : MonoBehaviour
{
    [SerializeField] protected Image _gaugeImage;    //�Q�[�W�Ƃ��Ďg��Image

    //�Q�[�W�̌����ڂ�ݒ�
    public void UpdateGauge(float current, float max)
    {
        _gaugeImage.fillAmount = current / max; //fillAmount���X�V
    }
}
