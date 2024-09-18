using UnityEngine;

public class AttackFollow : MonoBehaviour
{
    public Transform playerTransform; // プレイヤーのTransformを設定
    public Vector2 offset; // プレイヤーとの相対的なオフセット位置
    public bool shouldFollow = true; // 追従するかどうかを制御するフラグ

    void Update()
    {
        // shouldFollow が true の場合のみ、プレイヤーの位置に基づいて攻撃判定オブジェクトの位置を更新
        if (playerTransform != null)
        {
            transform.position = new Vector2(playerTransform.position.x + offset.x, playerTransform.position.y + offset.y);
        }
    }
}
