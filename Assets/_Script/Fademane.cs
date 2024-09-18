using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fademane : MonoBehaviour
{
    private CanvasGroup fadeCanvas;

    private static float expectedfadetime;
    //�V�[���J�ڂɂ�����Ɨ\�z����鎞��
    public static float expectedFadetime
    {
        get { return expectedfadetime; }
    }

    // Use this for initialization
    void Awake()
    {
        var prefab = (GameObject)Resources.Load("FadeCanvas");

        if (prefab == null)
        {
            Debug.LogError("FadeCanvas��Resources�t�H���_���Ɍ�����܂���ł����B");
            return;
        }

        var instance = Instantiate(prefab);
        DontDestroyOnLoad(instance); // �v���n�u�S�̂�DontDestroyOnLoad�ŕێ�

        fadeCanvas = instance.GetComponent<CanvasGroup>();
        if (fadeCanvas == null)
        {
            Debug.LogError("CanvasGroup��FadeCanvas�Ɍ�����܂���ł����B");
            return;
        }

        // �t�F�[�h�摜�̃A���t�@�l��0�ɐݒ�i�ŏ��͓����ɂ���j
        fadeCanvas.alpha = 0f;


        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }
    private Sequence sequence;

    public void ChangeSceneWithFade(float fadeiotime, float fadewaittime, string nextscenename)
    {
        var operation = SceneManager.LoadSceneAsync(nextscenename); //���V�[���̓ǂݍ��݊J�n
        operation.allowSceneActivation = false;//�V�[���̕ύX���u���b�N

        expectedfadetime = fadeiotime + fadewaittime;

        sequence = DOTween.Sequence();

        sequence
                //.OnStart(InputManager.Instance.StopInput)      //���͒�~
                .Append(fadeCanvas.DOFade(1f, fadeiotime))     //�t�F�[�h�A�E�g
                .AppendCallback(() =>
                {
                    sequence.Pause();                          //�V�[�����ς��܂Œ�~
                    operation.allowSceneActivation = true;     //�V�[���̕ύX������
                })
                .AppendInterval(fadewaittime)                  //�Ó]���ɒ�~
                .Append(fadeCanvas.DOFade(0f, fadeiotime));     //�t�F�[�h�C��
                    //.OnComplete(InputManager.Instance.StartInput); //���͍ĊJ

            sequence.Play();
    }

    void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        sequence.Play();
    }
}
