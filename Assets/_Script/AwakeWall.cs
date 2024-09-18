using UnityEngine;
using System.Collections;

public class AwakeWall : MonoBehaviour
{
    public GameObject visibleArea; // トリガーを解除したときに見えるようにするオブジェクト

    BoxCollider2D box;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioClip BOSSBGM;
    [SerializeField] GameObject bgmobj;
    [SerializeField] Camera mainCamera, bossCamera;
    [SerializeField] Canvas UI, canvas;

    private Vector3 playerLastPosition; // プレイヤーの最後の位置
    private bool hasTriggered = false; // 一度だけボス戦を開始するためのフラグ

    private void Start()
    {
        // 初期状態では見えないようにする
        SetVisible(false);
        box = GetComponent<BoxCollider2D>();

        // 音楽を事前に読み込む
        if (bgmAudioSource != null && BOSSBGM != null)
        {
            bgmAudioSource.clip = BOSSBGM;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerLastPosition = other.transform.position; // プレイヤーの初期位置を記録
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hasTriggered)
        {
            Vector3 playerCurrentPosition = other.transform.position;

            // プレイヤーが右に移動した場合のチェック
            if (playerCurrentPosition.x > playerLastPosition.x)
            {
                hasTriggered = true; // ボス戦を開始するフラグを立てる
                StartCoroutine(BossStart());
            }
        }
    }

    private void SetVisible(bool visible)
    {
        if (visibleArea != null)
        {
            visibleArea.SetActive(visible);
        }
    }

    private IEnumerator BossStart()
    {
        yield return new WaitForSeconds(0.1f); // 少し待機

        SetVisible(true);
        box.isTrigger = false;

        // bgmobjのAudioSourceを停止
        AudioSource bgmObjAudioSource = bgmobj.GetComponent<AudioSource>();
        if (bgmObjAudioSource != null)
        {
            bgmObjAudioSource.Stop();
        }

        // 新しいBGMを再生
        if (bgmAudioSource != null && BOSSBGM != null)
        {
            bgmAudioSource.Play();
        }
        SwitchCamera();
    }

    private void SwitchCamera()
    {
        if (mainCamera != null && bossCamera != null)
        {
            mainCamera.gameObject.SetActive(false); // メインカメラを無効にする
            bossCamera.gameObject.SetActive(true); // ボスカメラを有効にする
            UI.worldCamera = bossCamera;
            canvas.worldCamera = bossCamera;
        }
    }
}
