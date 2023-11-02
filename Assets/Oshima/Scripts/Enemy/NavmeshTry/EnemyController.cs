using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public NavigationSystem navigationSystem; // �i�r�Q�[�V�����V�X�e�����Q�Ƃ��邽�߂̕ϐ�
    public Transform target; // �ړI�n��ݒ肷�邽�߂�Transform

    void Start()
    {
        // �i�r�Q�[�V�����V�X�e���̎Q�Ƃ��擾
        navigationSystem = FindObjectOfType<NavigationSystem>();
    }

    void Update()
    {
        // �i�r�Q�[�V�����V�X�e�����g���Ĉړ����鏈��
        List<Vector3> path = navigationSystem.FindPath(transform.position, target.position);

        if (path != null && path.Count > 0)
        {
            Vector3 nextPosition = path[0];
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * 5);

            if (Vector3.Distance(transform.position, nextPosition) < 0.001f)
            {
                path.RemoveAt(0);
            }
        }
    }
}
