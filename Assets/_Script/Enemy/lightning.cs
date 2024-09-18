using UnityEngine;

public class lightning : MonoBehaviour
{
    public GameObject associatedLight; // 雷に関連付けられたライト

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.gameObject.CompareTag("Player"))
        {
            if (associatedLight != null)
            {
                associatedLight.SetActive(false); // エフェクトを非表示
            }
            Destroy(gameObject); // 雷オブジェクトを削除
        }
    }
}
