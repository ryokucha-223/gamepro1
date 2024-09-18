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
    [Header("背景画像の配列")] [SerializeField] GameObject[] haikei;
    private Rigidbody2D rb;
    // PS4コントローラーの左レバーの水平軸
    public string verticalAxis = "Vertical";
    public float up = 0.5f; // 上
    public float down = -0.5f; // 下
    [SerializeField] float moveSpeed = 5f; // 移動速度
    [SerializeField] float stopDistance = 0.1f; // 停止距離
    int state = 0;
    private bool isMoving = false;
    private float lastInputTime = 0f; // 最後の入力時間
    private float inputCooldown = 0.3f; // 入力の間　連続入力の阻止
    [SerializeField] AudioClip SE_sel, SE_dec;
    AudioSource snd;
    bool caninput = false;
    PlayyerMove pl;
    private bool canProceed = true; // ボタン連打防止用フラグ
    public float buttonCooldownTime = 100.0f; // ボタン入力クールダウン時間
    private Fademane fademane;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.ResetFlags(); // フラグをリセットする
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
        UpdateBackground(); // 初期背景を設定
        fademane = FindObjectOfType<Fademane>();//fademaneの取得
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
    }

    void Update()
    {
        if (isMoving) return; // 移動中またはシーン遷移中は入力を無視

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
                    lastInputTime = Time.time; // 最後の入力時間を更新
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
                    lastInputTime = Time.time; // 最後の入力時間を更新
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

    IEnumerator StartTransition() // SEが鳴り終わるまで待機
    {
        canProceed = false; // ボタン連打を防止
        isMoving = true; // シーン遷移中は入力を無視
        caninput = false;
        snd.PlayOneShot(SE_dec);
        yield return new WaitWhile(() => snd.isPlaying);
        if(fademane!=null)
        {
            fademane.ChangeSceneWithFade(1f, 0.5f, "STAGE"+state); // フェード時間は適宜調整
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

        // 目的地に近づいたら停止
        if (direction.magnitude < stopDistance)
        {
            rb.velocity = Vector2.zero;
            transform.position = targetPos; // 位置を設定
            isMoving = false;
            caninput = true;
            UpdateStageName();
            UpdateBackground(); // 背景を更新
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
        // 全ての背景を非表示にする
        foreach (var bg in haikei)
        {
            bg.SetActive(false);
        }

        // 現在のステージに対応する背景を表示
        if (haikei.Length > state)
        {
            haikei[state].SetActive(true);
        }
    }

    IEnumerator str() // SEが鳴り終わるまで待機
    {
        isMoving = true; // シーン遷移中は入力を無視
        caninput = false;
        snd.PlayOneShot(SE_dec);
        yield return new WaitWhile(() => snd.isPlaying);
        SceneManager.LoadScene("STAGE" + state);
    }
}
