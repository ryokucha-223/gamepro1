using UnityEngine;
using System.Collections;

public class down : MonoBehaviour
{
    [SerializeField] float speed = 5f, wait = 1f;
    bool isDown = true;
    bool isWait = false;
    //bool firstFall = true;
    void Start()
    {
        
    }

    void Update()
    {
        if(!isWait)
        {
            float i = isDown ? -1 : 1;
            transform.Translate(Vector2.up * speed * i * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag=="Ground")
        {
           
                isWait = true;
                isDown = !isDown;
                StartCoroutine(waitTimer());
            
        }
    }
    IEnumerator waitTimer()
    {
        yield return new WaitForSeconds(wait);
        isWait = false;
    }
}
