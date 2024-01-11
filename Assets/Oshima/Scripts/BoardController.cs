using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] float ADJUST_SPEED;    // 看板の切り替わるスピードを調整できる定数("ADJUST_SPEED >= 1"とすること)

    private Material m_Material;
    float[] m_KeyOffset = new float[4] { 0.5f, 0.0f, 0.5f, 0.0f };    // 各看板が表示されるoffset.y値
   // float[] m_KeyOffset = new float[2] {  0.5f,0.0f };
    int m_Start = 0;        // 0:m_KeyOffset[0]->m_KeyOffset[1]へ切り替え, 1:[1]->[2]へ切り替え, 2:[2]->[3]へ切り替え, 3:[3]->[0]へ切り替え
    bool m_Move = false;    // T:看板を切り替えている最中, F:看板が停止している

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
            m_Move = true;  // offset.y変更開始
        }
        if (m_Move)
        {
            float y = m_Material.mainTextureOffset.y - Time.deltaTime / ADJUST_SPEED;   // 移動量算出
            if (y <= m_KeyOffset[m_Start + 1])
            {
                y = m_KeyOffset[m_Start + 1];
                m_Start++;
                if (m_Start >= m_KeyOffset.Length - 1)
                {
                    // 最後の看板が表示されたら、同じデザインである先頭の看板に切り替える
                    m_Start = 0;
                    y = m_KeyOffset[0];
                }
                m_Move = false;
            }
            m_Material.mainTextureOffset = new Vector2(0, y);   // offset値更新
        }
    }
}
