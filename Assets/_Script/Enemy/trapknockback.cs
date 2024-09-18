using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapknockback : MonoBehaviour
{
    [SerializeField] float forceMagnitude = 2f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // �v���C���[�I�u�W�F�N�g�ɐG�ꂽ���̏���
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("aaaa");
            // �v���C���[�̃��[�J���X�P�[�����擾
            Transform playerTransform = col.gameObject.transform;
            Vector3 playerScale = playerTransform.localScale;
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // ���[�J���X�P�[���Ɋ�Â��ė͂�������
                Vector2 forceDirection = Vector2.zero;

                if (playerScale.x > 0)
                {
                    // ���[�J���X�P�[�������Ȃ獶��
                    forceDirection = Vector2.left;
                }
                else if (playerScale.x < 0)
                {
                    // ���Ȃ�E��
                    forceDirection = Vector2.right;
                }

                // �͂�������
                rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
            }
        }
    }
}
