using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapicon : MonoBehaviour
{
    void LateUpdate()
    {
        // プレイヤー（親オブジェクト）のスケールを取得
        Vector3 parentScale = transform.localScale;

        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // スプライトが反転しないようにスケールを調整
                spriteRenderer.flipX = parentScale.x < 0;
                spriteRenderer.flipY = parentScale.y < 0;
            }
        }
    }
}