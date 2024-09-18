using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    [Header("�X�s�[�h")] public float speed = 3.0f;
    [Header("�ő�ړ�����")] public float maxDistance = 100.0f;
    private Rigidbody2D rb;
    private Vector3 defaultPos;
    private Vector2 shotDirection;
    public bool muki; // �v���C���[�̌������󂯎�邽�߂̕ϐ�

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;

        // muki�Ɋ�Â��Ēe�̐i�s������ݒ�
        shotDirection = muki ? Vector2.right : Vector2.left;
        rb.velocity = shotDirection * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float d = Vector3.Distance(transform.position, defaultPos);

        // �ő�ړ������𒴂��Ă���ꍇ�͒e������
        if (d > maxDistance)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // Debug.Log("Collision detected with: " + collision.name);

        // Ground�I�u�W�F�N�g�ɐG�ꂽ�ꍇ�A�e�̐i�s�����ƐڐG�������r
        if (collision.CompareTag("Ground"))
        {
           
                
                Destroy(this.gameObject);
            
        }
    }
}
