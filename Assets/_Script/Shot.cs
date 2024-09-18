using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    [Header("スピード")] public float speed = 3.0f;
    [Header("最大移動距離")] public float maxDistance = 100.0f;
    private Rigidbody2D rb;
    private Vector3 defaultPos;
    private Vector2 shotDirection;
    public bool muki; // プレイヤーの向きを受け取るための変数

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;

        // mukiに基づいて弾の進行方向を設定
        shotDirection = muki ? Vector2.right : Vector2.left;
        rb.velocity = shotDirection * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float d = Vector3.Distance(transform.position, defaultPos);

        // 最大移動距離を超えている場合は弾を消す
        if (d > maxDistance)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // Debug.Log("Collision detected with: " + collision.name);

        // Groundオブジェクトに触れた場合、弾の進行方向と接触方向を比較
        if (collision.CompareTag("Ground"))
        {
           
                
                Destroy(this.gameObject);
            
        }
    }
}
