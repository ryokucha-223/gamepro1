using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoSSbumelan : MonoBehaviour
{
    [SerializeField] float speed = 5f; // �e�̈ړ����x
    [SerializeField] float maxDistance = 10f; // �e���ړ�����ő勗��
    public GameObject boss; // �{�X�̃I�u�W�F�N�g
    public Transform origin; // �e�̔��ˌ��i�{�X�̈ʒu�j
    [SerializeField] AudioClip SE_cou;
    AudioSource snd;

    private Vector2 startPosition;
    private bool returning = false; // �߂��Ă��邩�ǂ���

    public bool IsReturning => returning; // �v���p�e�B�ŏ�Ԃ����J
    bool counter;

    void Start()
    {
        snd = GetComponent<AudioSource>();
        startPosition = transform.position;

        // boss���ݒ肳��Ă��Ȃ��ꍇ�A�e�I�u�W�F�N�g��boss�Ƃ��Đݒ�
        if (boss == null && transform.parent != null)
        {
            boss = transform.parent.gameObject;
        }
        if (boss == null)
        {
            Debug.LogError("Boss object is not assigned and could not be found in parent.");
        }
    }

    void Update()
    {
        MoveBullet();
    }

    void MoveBullet()
    {
        float distance = Vector2.Distance(startPosition, transform.position);

        if (boss != null)
        {
            if (!returning)
            {
                // �e���ړ�����
                transform.position += (Vector3)transform.right * speed * Time.deltaTime;

                // �w�肵�������𒴂�����߂�
                if (distance >= maxDistance)
                {
                    returning = true;
                }
            }
            else
            {
                // �e���߂�
                Vector2 direction = (origin.position - transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;

                // ���ˌ��ɖ߂�����폜
                if (Vector2.Distance(transform.position, origin.position) <= 0.1f)
                {
                    NotifyBossOfReturn();
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Debug.LogError("Boss object is not assigned.");
        }
    }

    void NotifyBossOfReturn()
    {
        if (boss != null)
        {
            BOSSmove bossScript = boss.GetComponent<BOSSmove>();
            if (bossScript != null)
            {
                bossScript.BoomerangReturned();
            }
            else
            {
                Debug.LogError("BOSSmove script is not attached to the boss object.");
            }
        }
        else
        {
            Debug.LogError("Boss object is not assigned.");
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BOSS")
        {
            if(counter)
            {
                BOSSmove bossScript = boss.GetComponent<BOSSmove>();
                bossScript.counter();
                bossScript.TakeDamage(transform.position);
                counter = false;
            }
            else
            {
                NotifyBossOfReturn();
            }
            Destroy(gameObject);
        }
        if(collision.gameObject.tag=="Player")
        {
            Vector2 collisionPoint = collision.ClosestPoint(transform.position);

            // �G��Knockback�X�N���v�g���擾
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // �m�b�N�o�b�N��K�p
                playerMove.plDamage(collisionPoint);
            }
        }
        if(collision.gameObject.tag=="slash")
        {
            snd.PlayOneShot(SE_cou);
            returning = true;
            counter = true;
        }
    }
}
