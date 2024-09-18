using UnityEngine;

public class BoundEnemy : MonoBehaviour
{
    [Header("高さ")] [SerializeField] public float verticalAmplitude = 1f;
    [Header("はねる速さ")] [SerializeField] public float verticalFrequency = 1f;
    [Header("横の移動範囲")] [SerializeField] public float horizontalAmplitude = 3f;
    [Header("横の速度")] [SerializeField] public float horizontalSpeed = 1f;
    [Header("反対方向の速度")] [SerializeField] public float reverseSpeed = 1f;
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
            rb.gravityScale = 0;  // 重力を無効にする
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // 回転を固定
        }
    }

    void FixedUpdate()
    {
        Move();

        // プレイヤーの方向に応じてボスの向きを変更
        if (Player != null)
        {
            Vector3 directionToPlayer = Player.transform.position - transform.position;

            if (directionToPlayer.x > 0)
            {
                transform.localScale = new Vector3(-0.4f, 0.4f, 0.4f); // プレイヤーが右にいる場合
            }
            else
            {
                transform.localScale = new Vector3(0.4f, 0.4f, 0.4f); // プレイヤーが左にいる場合
            }
        }
    }
    private void Move()
    {
        if(!isdead)
        {
            // 垂直移動の計算
            float y = Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;
            // 水平移動の計算
            float x = Mathf.Sin(Time.time * horizontalSpeed) * horizontalAmplitude;

            // Rigidbody2D の velocity を設定
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

            // 敵のKnockbackスクリプトを取得
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // ノックバックを適用
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
            Instantiate(hitefect, col.transform.position, Quaternion.identity);//ヒットエフェクト            Debug.Log("hit");
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
            Instantiate(hitefect, col.transform.position, Quaternion.identity);//ヒットエフェクト            Debug.Log("hit");
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
            Instantiate(slefect, col.transform.position, Quaternion.identity);//ヒットエフェクト
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
