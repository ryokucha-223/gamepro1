using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoSSbumelan : MonoBehaviour
{
    [SerializeField] float speed = 5f; // 弾の移動速度
    [SerializeField] float maxDistance = 10f; // 弾が移動する最大距離
    public GameObject boss; // ボスのオブジェクト
    public Transform origin; // 弾の発射元（ボスの位置）
    [SerializeField] AudioClip SE_cou;
    AudioSource snd;

    private Vector2 startPosition;
    private bool returning = false; // 戻っているかどうか

    public bool IsReturning => returning; // プロパティで状態を公開
    bool counter;

    void Start()
    {
        snd = GetComponent<AudioSource>();
        startPosition = transform.position;

        // bossが設定されていない場合、親オブジェクトをbossとして設定
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
                // 弾が移動する
                transform.position += (Vector3)transform.right * speed * Time.deltaTime;

                // 指定した距離を超えたら戻る
                if (distance >= maxDistance)
                {
                    returning = true;
                }
            }
            else
            {
                // 弾が戻る
                Vector2 direction = (origin.position - transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;

                // 発射元に戻ったら削除
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

            // 敵のKnockbackスクリプトを取得
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // ノックバックを適用
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
