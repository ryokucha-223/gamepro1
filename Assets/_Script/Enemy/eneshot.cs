using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eneshot : MonoBehaviour
{
    [Header("スピード")] public float speed = 3.0f;
    [Header("最大移動距離")] public float maxDistance = 100.0f;
    private Rigidbody2D rb;
    private Vector3 defaultPos;
    private Vector3 direction; // 追加

    public bool lr;
    GameObject robo;

    // Start is called before the first frame update
    void Start()
    {
        robo = transform.root.gameObject;
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;

        // 発射方向を設定
        if (robo.transform.localScale.x < 0)
        {
            direction = Vector3.right; // 左向き
        }
        else
        {
            direction = Vector3.left;  // 右向き
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float d = Vector3.Distance(transform.position, defaultPos);

        // 最大移動距離を超えている
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
            // 衝突点を取得
            Vector2 collisionPoint = other.ClosestPoint(transform.position);

            // プレイヤーのPlayyerMoveスクリプトを取得
            PlayyerMove playerMove = other.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // ノックバックを適用
                playerMove.plDamage(collisionPoint);
            }

            // 自分自身を削除
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
