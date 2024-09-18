using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eneshot : MonoBehaviour
{
    [Header("�X�s�[�h")] public float speed = 3.0f;
    [Header("�ő�ړ�����")] public float maxDistance = 100.0f;
    private Rigidbody2D rb;
    private Vector3 defaultPos;
    private Vector3 direction; // �ǉ�

    public bool lr;
    GameObject robo;

    // Start is called before the first frame update
    void Start()
    {
        robo = transform.root.gameObject;
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;

        // ���˕�����ݒ�
        if (robo.transform.localScale.x < 0)
        {
            direction = Vector3.right; // ������
        }
        else
        {
            direction = Vector3.left;  // �E����
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float d = Vector3.Distance(transform.position, defaultPos);

        // �ő�ړ������𒴂��Ă���
        if (d > maxDistance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            rb.MovePosition(transform.position + direction * Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "shot" || other.gameObject.tag == "lassl" || other.gameObject.tag == "slash" || other.gameObject.tag == "beam")
        {
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Player")
        {
            // �Փ˓_���擾
            Vector2 collisionPoint = other.ClosestPoint(transform.position);

            // �v���C���[��PlayyerMove�X�N���v�g���擾
            PlayyerMove playerMove = other.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // �m�b�N�o�b�N��K�p
                playerMove.plDamage(collisionPoint);
            }

            // �������g���폜
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
