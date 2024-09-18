using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharaMove : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float jumpPower = -10.0f;
    public float stepforce = 2.0f;//ステップの距離
    public Vector2 stepDirection = Vector2.right; // ステップの方向（初期：右）
    private Vector2 force;
    private CharacterController controller;
    private Rigidbody2D rb;
    // public GameObject BunretuPrefab;
    public system sys;

    //接地判定
    private bool isGround = false;
    private bool onPlayer = false;
    //氷状態
    private bool isIce = false;

    // Start is called before the first frame update
    void Start()
    {
        force = new Vector2(0.0f, jumpPower);
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        if (moveX > 0)
        {
            stepDirection = Vector2.right;
        }
        else if (moveX < 0)
        {
            stepDirection = Vector2.left;
        }
         if (Input.GetKeyDown(KeyCode.JoystickButton4))
         {
            Step();
            sys.jetstep();
         }
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            // 移動速度ベクトルを現在値から取得
            Vector2 velocity = rb.velocity;
            // X方向の速度を入力から決定
                velocity.x = Input.GetAxisRaw("Horizontal") * moveSpeed;
            
            // 計算した移動速度ベクトルをRigidbody2Dに反映
            rb.velocity = velocity;
            //rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), 0).normalized * moveSpeed;//使わない.
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if(isGround||onPlayer)
            {
                // Debug.Log("Pushed x");
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
       
       
    }
    private void FixedUpdate()
    {

    }

    void Step()
    {
        rb.MovePosition(rb.position + stepDirection * stepforce);//addforceで動かない時用
    }

    //何かに触れているときに呼び出される
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = true;
           /// Debug.Log(isGround);
        }
        if(collision.collider.CompareTag("Player"))//泡の上にいるとき
        {
            onPlayer = true;
        }
    }

    //何かから離れたときに呼び出される
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = false;
          //  Debug.Log(isGround);
        }
    }

    //別のオブジェクトに触れた時の処理
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag=="Damage"&&!isIce)
        {
          //  Debug.Log("damaged");//トゲとかに触れた時
            Destroy(gameObject);
            system.Bubble--;
            system.DeadBubble++;
        }
        else if(col.gameObject.tag=="Ice")
        {
            isIce = true;
        }
    }
}
