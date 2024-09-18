using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StageSelect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stagename;
    [SerializeField] TextMeshProUGUI safetext;
    [SerializeField] public GameObject[] Stage = new GameObject[3];
    [Header("�w�i�摜�̔z��")] [SerializeField] GameObject[] haikei;
    private Rigidbody2D rb;
    // PS4�R���g���[���[�̍����o�[�̐�����
    public string verticalAxis = "Vertical";
    public float up = 0.5f; // ��
    public float down = -0.5f; // ��
    [SerializeField] float moveSpeed = 5f; // �ړ����x
    [SerializeField] float stopDistance = 0.1f; // ��~����
    int state = 0;
    private bool isMoving = false;
    private float lastInputTime = 0f; // �Ō�̓��͎���
    private float inputCooldown = 0.3f; // ���͂̊ԁ@�A�����͂̑j�~
    [SerializeField] AudioClip SE_sel, SE_dec;
    AudioSource snd;
    bool caninput = false;
    PlayyerMove pl;
    private bool canProceed = true; // �{�^���A�Ŗh�~�p�t���O
    public float buttonCooldownTime = 100.0f; // �{�^�����̓N�[���_�E������
    private Fademane fademane;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.ResetFlags(); // �t���O�����Z�b�g����
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found!");
        }

        snd = GetComponent<AudioSource>();
        if (snd == null)
        {
            snd = gameObject.AddComponent<AudioSource>();
        }
        isMoving = false;
        caninput = false;
        UpdateStageName();
        UpdateBackground(); // �����w�i��ݒ�
        fademane = FindObjectOfType<Fademane>();//fademane�̎擾
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
    }

    void Update()
    {
        if (isMoving) return; // �ړ����܂��̓V�[���J�ڒ��͓��͂𖳎�

        float x = Input.GetAxis(verticalAxis);

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene("TitleScenes");
        }

        if (Time.time - lastInputTime > inputCooldown)
        {
            if (x < down)
            {
                if (state > 0)
                {
                    state -= 1;
                    snd.PlayOneShot(SE_sel);
                    lastInputTime = Time.time; // �Ō�̓��͎��Ԃ��X�V
                    UpdateStageName();
                    Debug.Log(state);
                }
            }
            else if (x > up)
            {
                if (state < Stage.Length - 1)
                {
                    state += 1;
                    snd.PlayOneShot(SE_sel);
                    lastInputTime = Time.time; // �Ō�̓��͎��Ԃ��X�V
                    UpdateStageName();
                    Debug.Log(state);
                }
            }
        }

        if ((Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Space)) && canProceed && !isMoving && caninput)
        {
            StartCoroutine(StartTransition());
        }

        MoveTowardsTarget();
    }

    IEnumerator StartTransition() // SE����I���܂őҋ@
    {
        canProceed = false; // �{�^���A�ł�h�~
        isMoving = true; // �V�[���J�ڒ��͓��͂𖳎�
        caninput = false;
        snd.PlayOneShot(SE_dec);
        yield return new WaitWhile(() => snd.isPlaying);
        if(fademane!=null)
        {
            fademane.ChangeSceneWithFade(1f, 0.5f, "STAGE"+state); // �t�F�[�h���Ԃ͓K�X����
        }
        else
        {
            SceneManager.LoadScene("STAGE" + state);
        }
        StartCoroutine(ButtonCooldown());
    }

    IEnumerator ButtonCooldown()
    {
        yield return new WaitForSeconds(buttonCooldownTime);
        canProceed = true;
    }

    void MoveTowardsTarget()
    {
        Vector2 targetPos = Stage[state].transform.position;
        Vector2 currentPos = transform.position;
        Vector2 direction = targetPos - currentPos;

        // �ړI�n�ɋ߂Â������~
        if (direction.magnitude < stopDistance)
        {
            rb.velocity = Vector2.zero;
            transform.position = targetPos; // �ʒu��ݒ�
            isMoving = false;
            caninput = true;
            UpdateStageName();
            UpdateBackground(); // �w�i���X�V
        }
        else
        {
            caninput = false; ;
            rb.velocity = direction.normalized * moveSpeed;
        }
    }

    void UpdateStageName()
    {
        if (stagename != null && state >= 0 && state < Stage.Length)
        {
            if(state==2)
            {
                stagename.text = "BOSS STAGE";
                safetext.text = "Danger";
            }
            else
            {
                stagename.text = "Stage " + (state + 1).ToString();
                safetext.text = "Safety";
            }
        }
    }

    void UpdateBackground()
    {
        // �S�Ă̔w�i���\���ɂ���
        foreach (var bg in haikei)
        {
            bg.SetActive(false);
        }

        // ���݂̃X�e�[�W�ɑΉ�����w�i��\��
        if (haikei.Length > state)
        {
            haikei[state].SetActive(true);
        }
    }

    IEnumerator str() // SE����I���܂őҋ@
    {
        isMoving = true; // �V�[���J�ڒ��͓��͂𖳎�
        caninput = false;
        snd.PlayOneShot(SE_dec);
        yield return new WaitWhile(() => snd.isPlaying);
        SceneManager.LoadScene("STAGE" + state);
    }
}
