using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
    [SerializeField] AudioClip SE_dec;
    AudioSource snd;
    [SerializeField] TextMeshProUGUI textToBlink; // �_�ł�����TextMeshProUGUI�R���|�[�l���g
    public float blinkSpeed = 1.0f; // �_�ł̑��x

    private float timer;
    private bool isBlinking = true;
    private bool canProceed = true; // �{�^���A�Ŗh�~�p�t���O
    public float cooldownTime = 10.0f; // �N�[���_�E�����ԁi�b�j
    private Fademane fademane;

    // Start is called before the first frame update
    void Start()
    {
        // AudioSource�R���|�[�l���g���擾�A�Ȃ���Βǉ�����
        snd = GetComponent<AudioSource>();
        if (snd == null)
        {
            snd = gameObject.AddComponent<AudioSource>();
        }

        // AudioClip��null���ǂ������`�F�b�N����
        if (SE_dec == null)
        {
            Debug.LogError("AudioClip SE_dec is not assigned in the inspector.");
        }
        isBlinking = true;
        fademane = FindObjectOfType<Fademane>();//fademane�̎擾
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
            StartCoroutine(Cooldown()); // �N�[���_�E�����J�n
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

    IEnumerator StartTransition() // SE����I���܂őҋ@
    {
        if (SE_dec != null)
        {
            snd.PlayOneShot(SE_dec);
            // �I���܂őҋ@
            yield return new WaitWhile(() => snd.isPlaying);
            // ������I�������V�[���J��
            fademane.ChangeSceneWithFade(1f, 0.5f, "StageSelect"); // �t�F�[�h���Ԃ͓K�X����

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
