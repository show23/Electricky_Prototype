//using UnityEngine;

//public class Node1 : MonoBehaviour
//{
//    public bool walkable = true; // ���̃m�[�h���ړ��\���ǂ���
//    public Transform[] neighbors; // ���̃m�[�h�ɗאڂ���m�[�h����
//    public int gridX; // �O���b�h���X���W
//    public int gridY; // �O���b�h���Y���W

//    public int gCost; // �X�^�[�g����̈ړ��R�X�g
//    public int hCost; // �S�[���܂ł̐���ړ��R�X�g
//    public Transform parent; // �o�H��̑O�̃m�[�h

//    public int fCost { get { return gCost + hCost; } } // gCost��hCost�̍��v
//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f); // Node�̈ʒu�ɉ��F�����C���[�t���[���̗����̂�`��
//        UnityEditor.Handles.Label(transform.position, $"({gridX}, {gridY})"); // Node�̈ʒu�Ƀ��x����\��
//    }
//    // ���̑��̃��\�b�h��v���p�e�B��ǉ����邱�Ƃ��ł��܂�
//}
