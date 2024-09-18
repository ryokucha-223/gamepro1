using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rider : MonoBehaviour
{
    public Transform player;
    [SerializeField] float HP=5;
    [SerializeField] float direction =8f;//索敵範囲
    [SerializeField] float dist = 4f;//保つ距離
    [SerializeField] float speed = 2f;
    [SerializeField] float shotInterval = 2f;
    bool IsMoving;
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] GameObject shotobj;
    [SerializeField]
    AudioClip se_hit;
    [SerializeField] GameObject hitefect, slefect;
    AudioSource snd;


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        snd = gameObject.AddComponent < AudioSource >();
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.Log("err");
        }
        IsMoving = true;
        StartCoroutine(shot());
    }

    void Update()
    {
        if (HP <= 0)
        {
            anim.SetBool("dead", true);
        }
        float distplayer = Vector2.Distance(transform.position, player.position);
        if (player != null)
        {
            Vector3 directionToPlayer = player.transform.position - transform.position;

            if (directionToPlayer.x > 0)
            {
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // プレイヤーが右にいる場合
            }
            else
            {
                transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f); // プレイヤーが左にいる場合
            }
        }
        if (distplayer<direction&&IsMoving)
        {
            if(distplayer>dist)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
        }
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
    void dead()
    {
        newsystem.score += 300;
        newsystem.killcount++;
        Debug.Log(newsystem.killcount);
        IsMoving = false; // 攻撃が当たったら移動を停止する
        Destroy(gameObject);
    }

    private IEnumerator shot()
    {
        while(true)
        {
            float distplayer = Vector2.Distance(player.position , transform.position);
            if(distplayer<direction)
            {
                anim.SetTrigger("shot");
                GameObject shot = Instantiate(shotobj,transform.position, Quaternion.identity);
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                shot.GetComponent<Rigidbody2D>().velocity = directionToPlayer * speed;
            }
            yield return new WaitForSeconds(shotInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "shot")
        {
            HP--;
            Instantiate(hitefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
            Debug.Log("hit");
            snd.PlayOneShot(se_hit);
        }
        if (col.gameObject.tag == "beam")
        {
            HP-=5;
            Instantiate(hitefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
            Debug.Log("hit");
            snd.PlayOneShot(se_hit);
        }
        if (col.gameObject.tag == "slash"  )
        {
            HP--;
            Instantiate(slefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
             Debug.Log("hit");
            snd.PlayOneShot(se_hit);
            
        }
        if(col.gameObject.tag == "lassl")
        {
            HP-=2;
            Instantiate(slefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
             Debug.Log("hit");
            snd.PlayOneShot(se_hit);
        }
    }

}
