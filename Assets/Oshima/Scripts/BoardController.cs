using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] float ADJUST_SPEED;    // �Ŕ̐؂�ւ��X�s�[�h�𒲐��ł���萔("ADJUST_SPEED >= 1"�Ƃ��邱��)

    private Material m_Material;
    float[] m_KeyOffset = new float[4] { 0.75f, 0.5f, 0.25f, 0.0f };    // �e�Ŕ��\�������offset.y�l
    int m_Start = 0;        // 0:m_KeyOffset[0]->m_KeyOffset[1]�֐؂�ւ�, 1:[1]->[2]�֐؂�ւ�, 2:[2]->[3]�֐؂�ւ�, 3:[3]->[0]�֐؂�ւ�
    bool m_Move = false;    // T:�Ŕ�؂�ւ��Ă���Œ�, F:�Ŕ���~���Ă���

    // Use this for initialization
    void Start()
    {
        m_Material = this.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !m_Move)
        {
            m_Move = true;  // offset.y�ύX�J�n
        }
        if (m_Move)
        {
            float y = m_Material.mainTextureOffset.y - Time.deltaTime / ADJUST_SPEED;   // �ړ��ʎZ�o
            if (y <= m_KeyOffset[m_Start + 1])
            {
                y = m_KeyOffset[m_Start + 1];
                m_Start++;
                if (m_Start >= m_KeyOffset.Length - 1)
                {
                    // �Ō�̊Ŕ��\�����ꂽ��A�����f�U�C���ł���擪�̊Ŕɐ؂�ւ���
                    m_Start = 0;
                    y = m_KeyOffset[0];
                }
                m_Move = false;
            }
            m_Material.mainTextureOffset = new Vector2(0, y);   // offset�l�X�V
        }
    }
}
