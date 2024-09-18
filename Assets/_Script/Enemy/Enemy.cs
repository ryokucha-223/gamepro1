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
    public GameObject PlayerObject; // playerオブジェクトを受け取る器
    public Transform Player; // プレイヤーの座標情報などを受け取る器
    bool Isdamage;
    bool IsMoving = true; // 移動中かどうかを制御するフラグ
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
        rb = GetComponent<Rigidbody2D>(); // 修正: クラスレベルのrb変数に設定
        snd = gameObject.AddComponent<AudioSource>();
        HP = 4;
        Isdamage = false;
        if (PlayerObject == null)
        {
            PlayerObject = GameObject.FindWithTag("Player"); // "Player" タグを持つオブジェクトを検索
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
        else if (IsMoving) // IsMovingがtrueのときだけ移動する
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
        Vector2 e_pos = transform.position;  // 自分(敵キャラクタ)の座標
        Vector2 p_pos = Player.position;  // プレイヤーの座標
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
        // 方向ベクトルに速度を掛けて移動する
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
        if (col.gameObject.tag == "shot"|| col.gameObject.tag == "beam")
        {
            anim.SetBool("dead", true);
            Instantiate(hitefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
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
            IsMoving = false; // 攻撃が当たったら移動を停止する
        }
        if (col.gameObject.tag == "slash" || col.gameObject.tag == "lassl")
        {
            anim.SetBool("dead", true);

            Instantiate(slefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
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
            IsMoving = false; // 攻撃が当たったら移動を停止する
        }
    }
}

