using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Goalscene : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI yes, no,retry;
    [SerializeField]
    AudioClip se_sel;
    [SerializeField]
    AudioClip se_start, se_end,se_tag,se_score;//tagが項目、scoreが総額
    AudioSource snd;
    [SerializeField]
    TextMeshProUGUI scoretxt,highscoretxt;
    [SerializeField]
    TextMeshProUGUI coinct, diyact, killct, oxyct;
    [SerializeField] GameObject trg, targetyes, targetno;
    int state = 0;
    int score = newsystem.score;
    int coin = newsystem.coincount;
    int diya = newsystem.diyacount;
    int kill = newsystem.killcount;
    int oxy = newsystem.oxycount;

    [SerializeField] GameObject coinobj, diyaobj;

    public HighScore highScoreManager;


    bool isdis;//決定の判定
    bool canInput;//入力の可否

    public float right = 0.5f;//右
    public float left = -0.5f;//左
    // PS4コントローラーの左レバーの水平軸
    public string horizontalAxis = "LeftStickHorizontal";

    private Fademane fademane;

    void Start()
    {
        coinobj.SetActive(false);
        diyaobj.SetActive(false);
        killct.gameObject.SetActive(false);
        oxyct.gameObject.SetActive(false);
        scoretxt.gameObject.SetActive(false);
        retry.gameObject.SetActive(false);
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        trg.gameObject.SetActive(false);
        canInput = false;
        state = 1;
        yes.color = new Color(0.9528302f, 0.03056241f, 0.03056241f, 1f);
        no.color = new Color(255f, 255f, 255f, 1f);
        isdis = false;
        snd = gameObject.AddComponent<AudioSource>();
        StartCoroutine(DisplayTextWithDelay());

        fademane = FindObjectOfType<Fademane>();//fademaneの取得
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
        // デバッグログを追加してAudioClipの状態を確認
        /* Debug.Log("se_start: " + (se_start != null ? "Assigned" : "Not Assigned"));
         Debug.Log("se_end: " + (se_end != null ? "Assigned" : "Not Assigned"));
         Debug.Log("AudioSource: " + (snd != null ? "Exists" : "Not Exists"));*/
    }

    void Update()
    {
        // レバー入力
        float horizontalInput = Input.GetAxis(horizontalAxis);

        if (horizontalInput < left && !isdis && canInput)
        {
            yes.color = new Color(0.9528302f, 0.03056241f, 0.03056241f, 1f);
            no.color = new Color(255f, 255f, 255f, 1f);
            if (state != 1)
            {
                snd.PlayOneShot(se_sel);
                state = 1;
                trg.transform.position = targetyes.transform.position; // trgをtargetyesの位置に移動
            }
        }
        if (horizontalInput > right && !isdis && canInput)
        {
            no.color = new Color(0.9528302f, 0.03056241f, 0.03056241f, 1f);
            yes.color = new Color(255f, 255f, 255f, 1f);
            if (state != 2)
            {
                snd.PlayOneShot(se_sel);
                state = 2;
                trg.transform.position = targetno.transform.position; // trgをtargetnoの位置に移動
            }
        }
        if ((Input.GetKeyDown(KeyCode.JoystickButton1 )||Input.GetKeyDown(KeyCode.Space)) && !isdis && canInput)
        {
            if (state == 1)
            {
                isdis = true;
                StartCoroutine(str());//待機へ
            }
            else if (state == 2)
            {
                isdis = true;
                StartCoroutine(end());//待機へ
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Application.Quit();
        }
    }

    private IEnumerator DisplayTextWithDelay()
    {
        yield return new WaitForSeconds(1f);
        coinobj.SetActive(true);
        snd.PlayOneShot(se_tag);
        coinct.text = "　 ：" + coin + "×100";
        yield return new WaitForSeconds(1f); // 1秒待つ

        diyaobj.SetActive(true);
        snd.PlayOneShot(se_tag);
        diyact.text = "　 ：" + diya + "×1000";
        yield return new WaitForSeconds(1f);

        killct.gameObject.SetActive(true);
        killct.text = "倒した敵の数：" + kill + "×300";
        snd.PlayOneShot(se_tag);
        yield return new WaitForSeconds(1f);

        oxyct.gameObject.SetActive(true);
        oxyct.text = "ゲージボーナス！ ＋" + oxy;
        snd.PlayOneShot(se_tag);
        yield return new WaitForSeconds(1f);

        scoretxt.gameObject.SetActive(true);
        snd.PlayOneShot(se_score);
        scoretxt.text = "報酬金：＄" + score ;
        highScoreManager.InGoal();

        StartCoroutine(DisplayRetryAndOptions());
    }

    private IEnumerator DisplayRetryAndOptions()
    {
        yield return new WaitForSeconds(1f); // 1秒待つ

        retry.gameObject.SetActive(true);
        highscoretxt.gameObject.SetActive(true);
        trg.gameObject.SetActive(true);
        yes.gameObject.SetActive(true);
        no.gameObject.SetActive(true);
        canInput = true;
    }



    IEnumerator str()//seが鳴り終わるまで待機
    {
        // 音楽を鳴らす
        snd.PlayOneShot(se_start);
        Debug.Log("Playing se_start");

        // 終了まで待機
        yield return new WaitWhile(() => snd.isPlaying);
        Debug.Log("se_start finished");
        fademane.ChangeSceneWithFade(1f, 0.5f, "Stage2"); // フェード時間は適宜調整

    }

    IEnumerator end()//seが鳴り終わるまで待機
    {
        // 音楽を鳴らす
        snd.PlayOneShot(se_end);
        Debug.Log("Playing se_end");

        // 終了まで待機
        yield return new WaitWhile(() => snd.isPlaying);
        Debug.Log("se_end finished");
        fademane.ChangeSceneWithFade(1f, 0.5f, "TitleScenes"); // フェード時間は適宜調整

    }
}
