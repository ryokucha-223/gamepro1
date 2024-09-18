using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bone : MonoBehaviour
{
    [SerializeField]
    AudioClip se_hit;
    [SerializeField] GameObject hitefect, slefect;
    AudioSource snd;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        snd = gameObject.AddComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag=="Ground")
        {
            // 反射させる
            rb.velocity = new Vector2(-rb.velocity.x, -rb.velocity.y);//yを反転させてバウンド
        }
        if (col.gameObject.tag == "shot"||col.gameObject.tag=="beam")
        {
            
            Instantiate(hitefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
            Debug.Log("hit");
            snd.PlayOneShot(se_hit);
            Destroy(gameObject);
        }
        if (col.gameObject.tag == "slash")
        {
            Instantiate(slefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
            Debug.Log("hit");
            snd.PlayOneShot(se_hit);
            Destroy(gameObject);
        }
        if (col.gameObject.tag == "lassl")
        {
            Instantiate(slefect, col.transform.position, Quaternion.identity); // ヒットエフェクト
            Debug.Log("hit");
            snd.PlayOneShot(se_hit);
            Destroy(gameObject);
        }
    }
}
