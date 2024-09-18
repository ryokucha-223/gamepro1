using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapknockback : MonoBehaviour
{
    [SerializeField] float forceMagnitude = 2f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // プレイヤーオブジェクトに触れた時の処理
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("aaaa");
            // プレイヤーのローカルスケールを取得
            Transform playerTransform = col.gameObject.transform;
            Vector3 playerScale = playerTransform.localScale;
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // ローカルスケールに基づいて力を加える
                Vector2 forceDirection = Vector2.zero;

                if (playerScale.x > 0)
                {
                    // ローカルスケールが正なら左へ
                    forceDirection = Vector2.left;
                }
                else if (playerScale.x < 0)
                {
                    // 負なら右へ
                    forceDirection = Vector2.right;
                }

                // 力を加える
                rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
            }
        }
    }
}
