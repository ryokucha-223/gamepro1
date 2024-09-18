using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharaMove : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float jumpPower = -10.0f;
    public float stepforce = 2.0f;//�X�e�b�v�̋���
    public Vector2 stepDirection = Vector2.right; // �X�e�b�v�̕����i�����F�E�j
    private Vector2 force;
    private CharacterController controller;
    private Rigidbody2D rb;
    // public GameObject BunretuPrefab;
    public system sys;

    //�ڒn����
    private bool isGround = false;
    private bool onPlayer = false;
    //�X���
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
            // �ړ����x�x�N�g�������ݒl����擾
            Vector2 velocity = rb.velocity;
            // X�����̑��x����͂��猈��
                velocity.x = Input.GetAxisRaw("Horizontal") * moveSpeed;
            
            // �v�Z�����ړ����x�x�N�g����Rigidbody2D�ɔ��f
            rb.velocity = velocity;
            //rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), 0).normalized * moveSpeed;//�g��Ȃ�.
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
        rb.MovePosition(rb.position + stepDirection * stepforce);//addforce�œ����Ȃ����p
    }

    //�����ɐG��Ă���Ƃ��ɌĂяo�����
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = true;
           /// Debug.Log(isGround);
        }
        if(collision.collider.CompareTag("Player"))//�A�̏�ɂ���Ƃ�
        {
            onPlayer = true;
        }
    }

    //�������痣�ꂽ�Ƃ��ɌĂяo�����
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = false;
          //  Debug.Log(isGround);
        }
    }

    //�ʂ̃I�u�W�F�N�g�ɐG�ꂽ���̏���
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag=="Damage"&&!isIce)
        {
          //  Debug.Log("damaged");//�g�Q�Ƃ��ɐG�ꂽ��
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
