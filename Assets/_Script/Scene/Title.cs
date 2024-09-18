using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
    [SerializeField] AudioClip SE_dec;
    AudioSource snd;
    [SerializeField] TextMeshProUGUI textToBlink; // 点滅させるTextMeshProUGUIコンポーネント
    public float blinkSpeed = 1.0f; // 点滅の速度

    private float timer;
    private bool isBlinking = true;
    private bool canProceed = true; // ボタン連打防止用フラグ
    public float cooldownTime = 10.0f; // クールダウン時間（秒）
    private Fademane fademane;

    // Start is called before the first frame update
    void Start()
    {
        // AudioSourceコンポーネントを取得、なければ追加する
        snd = GetComponent<AudioSource>();
        if (snd == null)
        {
            snd = gameObject.AddComponent<AudioSource>();
        }

        // AudioClipがnullかどうかをチェックする
        if (SE_dec == null)
        {
            Debug.LogError("AudioClip SE_dec is not assigned in the inspector.");
        }
        isBlinking = true;
        fademane = FindObjectOfType<Fademane>();//fademaneの取得
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Application.Quit();
        }

        if ((Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Space)) && canProceed)
        {
            StartCoroutine(StartTransition());
            StopBlinking();
            StartCoroutine(Cooldown()); // クールダウンを開始
        }

        if (isBlinking)
        {
            timer += Time.deltaTime * blinkSpeed;
            Color color = textToBlink.color;
            color.a = Mathf.PingPong(timer, 1.0f);
            textToBlink.color = color;
        }
    }

    public void StopBlinking()
    {
        isBlinking = false;
        Color color = textToBlink.color;
        color.a = 1.0f;
        textToBlink.color = color;
    }

    IEnumerator StartTransition() // SEが鳴り終わるまで待機
    {
        if (SE_dec != null)
        {
            snd.PlayOneShot(SE_dec);
            // 終了まで待機
            yield return new WaitWhile(() => snd.isPlaying);
            // 音が鳴り終わったらシーン遷移
            fademane.ChangeSceneWithFade(1f, 0.5f, "StageSelect"); // フェード時間は適宜調整

        }
        else
        {
            Debug.LogError("AudioClip SE_dec is not assigned.");
            SceneManager.LoadScene("StageSelect");
        }
    }

    IEnumerator Cooldown()
    {
        canProceed = false;
        yield return new WaitForSeconds(cooldownTime);
        canProceed = true;
    }
}
