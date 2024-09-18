using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Houda : MonoBehaviour
{
    [SerializeField] int maxhp = 3;
    int hp;
    [Header("攻撃オブジェクト")] public GameObject attackObj;
    [Header("攻撃間隔")] public float interval;
    [Header("振り向きクールタイム")] public float turnCooldown = 2.0f; // 追加
    [SerializeField] GameObject item;
    private Animator anim;
    private float timer;
    private float turnTimer; // 追加
    GameObject bomb;
    [SerializeField] AudioClip se_hit;
    AudioSource snd;
    [SerializeField] GameObject hitefect, slefect;
    private bool isFacingLeft = true; // 追加

    // Start is called before the first frame update
    void Start()
    {
        hp = maxhp;
        anim = GetComponent<Animator>();
        bomb = (GameObject)Resources.Load("Bomb");
        snd = gameObject.AddComponent<AudioSource>();
        snd.volume = 1.0f;
        attackObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);

        // 通常の状態
        if (currentState.IsName("idle"))
        {
            // 振り向きクールタイムの処理
            if (turnTimer > 0.0f)
            {
                turnTimer -= Time.deltaTime;
            }
            else if (timer > interval)
            {
                anim.SetTrigger("attack");
                timer = 0.0f;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        if (hp <= 0)
        {
            dead();
        }
    }

    public void Attack()
    {
        GameObject g = Instantiate(attackObj);
        g.transform.SetParent(transform);
        g.transform.position = attackObj.transform.position;
        g.transform.rotation = attackObj.transform.rotation;

        // 発射方向を設定
        eneshot shotScript = g.GetComponent<eneshot>();
        if (shotScript != null)
        {
            if (transform.localScale.x < 0)
            {
                shotScript.lr = false; // 左向き
            }
            else
            {
                shotScript.lr = true; // 右向き
            }
        }

        g.SetActive(true);
    }

    private void LookAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = player.transform.position - transform.position;

            if (direction.x < 0 && !isFacingLeft)
            {
                isFacingLeft = true;
                transform.localScale = new Vector3(2, 2, 2);
                turnTimer = turnCooldown; // クールタイムをリセット
            }
            else if (direction.x >= 0 && isFacingLeft)
            {
                isFacingLeft = false;
                transform.localScale = new Vector3(-2, 2, 2);
                turnTimer = turnCooldown; // クールタイムをリセット
            }
        }
    }

    private void dead()
    {
        GetComponent<Collider2D>().enabled = false;
        anim.SetBool("dead", true);
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = false;

    }

    void enddead()
    {
        newsystem.score += 300;
        newsystem.killcount++;
        Debug.Log(newsystem.score);
        DropItem();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "shot")
        {
            newsystem.score += 300;
            newsystem.killcount++;
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Player")
        {
            Vector2 collisionPoint = collision.contacts[0].point;
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                playerMove.plDamage(collisionPoint);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "shot")
        {
            hp--;
            Instantiate(hitefect, col.transform.position, Quaternion.identity);
            snd.PlayOneShot(se_hit);
        }
        if (col.gameObject.tag == "beam")
        {
            hp-=3;
            Instantiate(hitefect, col.transform.position, Quaternion.identity);
            snd.PlayOneShot(se_hit);
        }
        if (col.gameObject.tag == "slash" || col.gameObject.tag == "lassl")
        {
            hp--;
            Instantiate(slefect, col.transform.position, Quaternion.identity);
            snd.PlayOneShot(se_hit);
        }
    }

    private void DropItem()
    {
        if (item != null)
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }
    }
}
