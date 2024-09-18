using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresuareBox : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject Item;
    [SerializeField] AudioClip SE_open;
    AudioSource snd;//音出すやつ
    [SerializeField] GameObject OpenEfect;//開けた時のエフェクト
    // Start is called before the first frame update
    void Start()
    {
        snd = gameObject.AddComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void trrigerOpen()
    {
        Vector3 effectPosition = transform.position;
        effectPosition.y += 0.2f;

        Vector3 position = transform.position;
        Instantiate(OpenEfect, effectPosition, Quaternion.identity);
        position.y += 0.2f;
        GameObject a = Instantiate(Item, position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            snd.PlayOneShot(SE_open);
            anim.SetBool("Open",true);
        }
    }
    void endanim()
    {
        Destroy(gameObject);

    }
}
