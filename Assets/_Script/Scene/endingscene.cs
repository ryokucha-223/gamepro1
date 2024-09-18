using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Endingscene : MonoBehaviour
{
    [SerializeField] AudioClip Se_end;
    AudioSource snd;
    int state;
    bool isdis; // 決定の判定
    bool canInput; // 入力の可否

    private Fademane fademane;


    void Start()
    {
        // AudioSourceの取得
        snd = GetComponent<AudioSource>();
        canInput = true;
        if (snd == null)
        {
            Debug.LogError("AudioSource component is missing from this GameObject.");
        }
        fademane = FindObjectOfType<Fademane>();//fademaneの取得
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
        // 初期化

    }

    void Update()
    {
       
        if ((Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Space)) && canInput)
        {
            canInput = false;
            StartCoroutine(Str()); // 待機へ
            
        }
       if(Input.GetKeyDown(KeyCode.Tab))
        {
            Application.Quit();
        }
    }

    IEnumerator Str() // seが鳴り終わるまで待機
    {
        // 音楽を鳴らす
        if (snd != null && Se_end != null)
        {
            snd.PlayOneShot(Se_end);
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is null.");
        }

        // 終了まで待機
        yield return new WaitWhile(() => snd.isPlaying);
        Debug.Log("se_end finished");
        fademane.ChangeSceneWithFade(1f, 0.5f, "TitleScenes"); // フェード時間は適宜調整
    }


}
