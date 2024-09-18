using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int HP;
    int rg = 1;
    public float pldis = 0.5f;
    public int moveSpeed = 10;
    public float atkInterval = 1.0f;
    private Rigidbody2D rb;
    private Animator anim;
    public GameObject PlayerObject; // player�I�u�W�F�N�g���󂯎���
    public Transform Player; // �v���C���[�̍��W���Ȃǂ��󂯎���
    bool Isdamage;
    bool IsMoving = true; // �ړ������ǂ����𐧌䂷��t���O
    private float wait = 0f;
    [SerializeField]
    LayerMask Pl;

    [SerializeField]
    AudioClip se_hit;
    [SerializeField] GameObject hitefect, slefect;
    AudioSource snd;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // �C��: �N���X���x����rb�ϐ��ɐݒ�
        snd = gameObject.AddComponent<AudioSource>();
        HP = 4;
        Isdamage = false;
        if (PlayerObject == null)
        {
            PlayerObject = GameObject.FindWithTag("Player"); // "Player" �^�O�����I�u�W�F�N�g������
            if (PlayerObject != null)
            {
                Player = PlayerObject.transform;
            }
            else
            {
                Debug.LogError("PlayerObject not found!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (wait > 0)
        {
            wait -= Time.deltaTime;
        }
        else if (IsMoving) // IsMoving��true�̂Ƃ������ړ�����
        {
            move();
        }
        if (HP <= 0)
        {
            anim.SetBool("dead", true);
            Isdamage = true;
        }
    }

    void move()
    {
        Vector2 e_pos = transform.position;  // ����(�G�L�����N�^)�̍��W
        Vector2 p_pos = Player.position;  // �v���C���[�̍��W
        Vector3 direction = new Vector3(p_pos.x - e_pos.x, 0f, 0f);
        float dir = Mathf.Abs(direction.x);
        if (direction.x > 0)
        {
            //num = 1;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
        else if (direction.x < 0)
        {
            //num = -1;
            transform.localScale = new Vector3(-0.3f, 0.3f, 0.3f);
        }
        // �����x�N�g���ɑ��x���|���Ĉړ�����
        if (Isdamage == false)
        {
            if (dir < 10)
            {
                transform.position += direction * moveSpeed * Time.deltaTime * rg;
            }
        }
    }

    void deadend()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "shot")
        {
            snd.PlayOneShot(se_hit);
            newsystem.score += 300;
            newsystem.killcount++;
        }
        if (collision.gameObject.tag == "beam")
        {
            snd.PlayOneShot(se_hit);
            newsystem.score += 300;
            newsystem.killcount++;
        }
        if (collision.gameObject.tag == "Player")
        {
            Vector2 collisionPoint = collision.contacts[0].point;

            // �G��Knockback�X�N���v�g���擾
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // �m�b�N�o�b�N��K�p
                playerMove.plDamage(collisionPoint);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "shot"|| col.gameObject.tag == "beam")
        {
            anim.SetBool("dead", true);
            Instantiate(hitefect, col.transform.position, Quaternion.identity); // �q�b�g�G�t�F�N�g
            Debug.Log("hit");
            snd.PlayOneShot(se_hit);
            Renderer renderer = GetComponent<Renderer>();
            renderer.enabled = false;
            newsystem.score += 300;
            newsystem.killcount++;
            Debug.Log(newsystem.killcount);
            GetComponent<Collider2D>().enabled = false;
            if (rb != null)
            {
                rb.gravityScale = 0;
            }
            else
            {
                Debug.LogWarning("Rigidbody2D component is not assigned!");
            }
            IsMoving = false; // �U��������������ړ����~����
        }
        if (col.gameObject.tag == "slash" || col.gameObject.tag == "lassl")
        {
            anim.SetBool("dead", true);

            Instantiate(slefect, col.transform.position, Quaternion.identity); // �q�b�g�G�t�F�N�g
            // Debug.Log("hit");
            snd.PlayOneShot(se_hit);
            Renderer renderer = GetComponent<Renderer>();
            renderer.enabled = false;
            newsystem.score += 300;
            newsystem.killcount++;
            Debug.Log(newsystem.killcount);
            GetComponent<Collider2D>().enabled = false;
            if (rb != null)
            {
                rb.gravityScale = 0;
            }
            else
            {
                Debug.LogWarning("Rigidbody2D component is not assigned!");
            }
            IsMoving = false; // �U��������������ړ����~����
        }
    }
}

