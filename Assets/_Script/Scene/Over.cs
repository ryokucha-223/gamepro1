using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Over : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI yes;
    [SerializeField]
    TextMeshProUGUI no;
    [SerializeField]
    AudioClip se_sel;
    [SerializeField]
    AudioClip se_start;
    [SerializeField]
    AudioClip se_end;
    AudioSource snd;
    int state = 0;

    public float right = 0.5f;//右
    public float left = -0.5f;//左
    // PS4コントローラーの左レバーの水平軸
    public string horizontalAxis = "Horizontal";
    bool isdis;
    private Fademane fademane;

    void Start()
    {
        state = 1;
        yes.color = new Color(0.9528302f, 0.03056241f, 0.03056241f, 1f);
        no.color = new Color(255f, 255f, 255f, 1f);
        isdis = false;
        snd = gameObject.AddComponent<AudioSource>();

        fademane = FindObjectOfType<Fademane>();//fademaneの取得
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //レバー入力
        float horizontalInput = Input.GetAxis(horizontalAxis);

        if (horizontalInput < left && !isdis)
        {
            yes.color = new Color(0.9528302f, 0.03056241f, 0.03056241f, 1f);
            no.color = new Color(255f, 255f, 255f, 1f);
            if (state != 1)
            {
                snd.PlayOneShot(se_sel);
                state = 1;
            }
        }
        if (horizontalInput > right && !isdis)
        {
            no.color = new Color(0.9528302f, 0.03056241f, 0.03056241f, 1f);
            yes.color = new Color(255f, 255f, 255f, 1f);
            if (state != 2)
            {
                snd.PlayOneShot(se_sel);
                state = 2;
            }
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Space) && !isdis)
        {
            if (state == 1)
            {
                StartCoroutine(str());//待機へ
                isdis = true;
            }
            else if (state == 2)
            {
                StartCoroutine(end());//待機へ
                isdis = true;
            }
        }
       if (Input.GetKeyDown(KeyCode.Tab))
        {
            Application.Quit();
        }
    }

    IEnumerator str()//seが鳴り終わるまで待機
    {
        // 音楽を鳴らす
        snd.PlayOneShot(se_start);
        Debug.Log("Playing se_start");

        // 終了まで待機
        yield return new WaitWhile(() => snd.isPlaying);
        Debug.Log("se_start finished");
        // 前のシーンに戻る
        GameManager.Instance.LoadPreviousScene();
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
// }
//}

