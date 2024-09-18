using UnityEngine;

public class BoundEnemy : MonoBehaviour
{
    [Header("����")] [SerializeField] public float verticalAmplitude = 1f;
    [Header("�͂˂鑬��")] [SerializeField] public float verticalFrequency = 1f;
    [Header("���̈ړ��͈�")] [SerializeField] public float horizontalAmplitude = 3f;
    [Header("���̑��x")] [SerializeField] public float horizontalSpeed = 1f;
    [Header("���Ε����̑��x")] [SerializeField] public float reverseSpeed = 1f;
    [SerializeField] GameObject Player;
    [SerializeField]
    AudioClip se_hit;
    [SerializeField] GameObject hitefect, slefect;
    AudioSource snd;
    Animator anim;

    private Rigidbody2D rb;

    bool isdead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        snd = gameObject.AddComponent<AudioSource>();
        anim = GetComponent<Animator>();
        isdead = false;
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;  // �d�͂𖳌��ɂ���
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // ��]���Œ�
        }
    }

    void FixedUpdate()
    {
        Move();

        // �v���C���[�̕����ɉ����ă{�X�̌�����ύX
        if (Player != null)
        {
            Vector3 directionToPlayer = Player.transform.position - transform.position;

            if (directionToPlayer.x > 0)
            {
                transform.localScale = new Vector3(-0.4f, 0.4f, 0.4f); // �v���C���[���E�ɂ���ꍇ
            }
            else
            {
                transform.localScale = new Vector3(0.4f, 0.4f, 0.4f); // �v���C���[�����ɂ���ꍇ
            }
        }
    }
    private void Move()
    {
        if(!isdead)
        {
            // �����ړ��̌v�Z
            float y = Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;
            // �����ړ��̌v�Z
            float x = Mathf.Sin(Time.time * horizontalSpeed) * horizontalAmplitude;

            // Rigidbody2D �� velocity ��ݒ�
            rb.velocity = new Vector2(x, y);
        }
    }
    void enddead()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
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
        if (col.gameObject.tag == "shot")
        {
            isdead = true;
            anim.SetBool("dead", true);
            GetComponent<Collider2D>().enabled = false;
            Instantiate(hitefect, col.transform.position, Quaternion.identity);//�q�b�g�G�t�F�N�g            Debug.Log("hit");
             snd.PlayOneShot(se_hit);
            Renderer renderer = GetComponent<Renderer>();
            renderer.enabled = false;
            newsystem.score += 300;
            newsystem.killcount++;
            Debug.Log(newsystem.killcount);
        }
        if (col.gameObject.tag == "beam")
        {
            isdead = true;
            anim.SetBool("dead", true);
            GetComponent<Collider2D>().enabled = false;
            Instantiate(hitefect, col.transform.position, Quaternion.identity);//�q�b�g�G�t�F�N�g            Debug.Log("hit");
            snd.PlayOneShot(se_hit);
            Renderer renderer = GetComponent<Renderer>();
            renderer.enabled = false;
            newsystem.score += 300;
            newsystem.killcount++;
            Debug.Log(newsystem.killcount);
        }
        if (col.gameObject.tag == "slash"|| col.gameObject.tag == "lassl")
        {
            isdead = true;
            anim.SetBool("dead", true);
            GetComponent<Collider2D>().enabled = false;
            Instantiate(slefect, col.transform.position, Quaternion.identity);//�q�b�g�G�t�F�N�g
        //   Debug.Log("hit");
             snd.PlayOneShot(se_hit);
            Renderer renderer = GetComponent<Renderer>();
            renderer.enabled = false;
            newsystem.score += 300;
            newsystem.killcount++;
            Debug.Log(newsystem.killcount);

        }
    }
}
