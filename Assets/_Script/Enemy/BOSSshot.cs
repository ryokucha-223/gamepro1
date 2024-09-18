using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSShot : MonoBehaviour
{
    private Vector2 shotDirection;
    [SerializeField] GameObject efect;

    void Start()
    {
        // 弾の移動方向に応じてスプライトを反転させる
        UpdateSpriteDirection();
    }

    private void UpdateSpriteDirection()
    {
        // shotDirectionが正規化されていると仮定して、向きに基づいてスプライトを反転
        if (shotDirection.x < 0)
        {
            // 左方向に進んでいる場合、スプライトを反転
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // 右方向に進んでいる場合、スプライトをそのままにする
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Groundオブジェクトに触れた場合、弾の進行方向と接触方向を比較
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "shot")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "beam")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "slash")
        {
            CreateParticleEffect();
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "lassl")
        {
            CreateParticleEffect();
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Player")
        {
            Vector2 collisionPoint = collision.ClosestPoint(transform.position);

            // 敵のKnockbackスクリプトを取得
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // ノックバックを適用
                playerMove.plDamage(collisionPoint);
            }
            Destroy(gameObject);
        }
    }
    private void CreateParticleEffect()
    {
        if (efect != null)
        {
            // パーティクルエフェクトを親なしで生成
            GameObject particleEffect = Instantiate(efect, transform.position, Quaternion.identity);
            
        }
    }
}
