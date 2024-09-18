using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Endingscene : MonoBehaviour
{
    [SerializeField] AudioClip Se_end;
    AudioSource snd;
    int state;
    bool isdis; // ����̔���
    bool canInput; // ���͂̉�

    private Fademane fademane;


    void Start()
    {
        // AudioSource�̎擾
        snd = GetComponent<AudioSource>();
        canInput = true;
        if (snd == null)
        {
            Debug.LogError("AudioSource component is missing from this GameObject.");
        }
        fademane = FindObjectOfType<Fademane>();//fademane�̎擾
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
        // ������

    }

    void Update()
    {
       
        if ((Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Space)) && canInput)
        {
            canInput = false;
            StartCoroutine(Str()); // �ҋ@��
            
        }
       if(Input.GetKeyDown(KeyCode.Tab))
        {
            Application.Quit();
        }
    }

    IEnumerator Str() // se����I���܂őҋ@
    {
        // ���y��炷
        if (snd != null && Se_end != null)
        {
            snd.PlayOneShot(Se_end);
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is null.");
        }

        // �I���܂őҋ@
        yield return new WaitWhile(() => snd.isPlaying);
        Debug.Log("se_end finished");
        fademane.ChangeSceneWithFade(1f, 0.5f, "TitleScenes"); // �t�F�[�h���Ԃ͓K�X����
    }


}
