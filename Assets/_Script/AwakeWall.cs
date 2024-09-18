using UnityEngine;
using System.Collections;

public class AwakeWall : MonoBehaviour
{
    public GameObject visibleArea; // �g���K�[�����������Ƃ��Ɍ�����悤�ɂ���I�u�W�F�N�g

    BoxCollider2D box;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioClip BOSSBGM;
    [SerializeField] GameObject bgmobj;
    [SerializeField] Camera mainCamera, bossCamera;
    [SerializeField] Canvas UI, canvas;

    private Vector3 playerLastPosition; // �v���C���[�̍Ō�̈ʒu
    private bool hasTriggered = false; // ��x�����{�X����J�n���邽�߂̃t���O

    private void Start()
    {
        // ������Ԃł͌����Ȃ��悤�ɂ���
        SetVisible(false);
        box = GetComponent<BoxCollider2D>();

        // ���y�����O�ɓǂݍ���
        if (bgmAudioSource != null && BOSSBGM != null)
        {
            bgmAudioSource.clip = BOSSBGM;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerLastPosition = other.transform.position; // �v���C���[�̏����ʒu���L�^
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hasTriggered)
        {
            Vector3 playerCurrentPosition = other.transform.position;

            // �v���C���[���E�Ɉړ������ꍇ�̃`�F�b�N
            if (playerCurrentPosition.x > playerLastPosition.x)
            {
                hasTriggered = true; // �{�X����J�n����t���O�𗧂Ă�
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
        yield return new WaitForSeconds(0.1f); // �����ҋ@

        SetVisible(true);
        box.isTrigger = false;

        // bgmobj��AudioSource���~
        AudioSource bgmObjAudioSource = bgmobj.GetComponent<AudioSource>();
        if (bgmObjAudioSource != null)
        {
            bgmObjAudioSource.Stop();
        }

        // �V����BGM���Đ�
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
            mainCamera.gameObject.SetActive(false); // ���C���J�����𖳌��ɂ���
            bossCamera.gameObject.SetActive(true); // �{�X�J������L���ɂ���
            UI.worldCamera = bossCamera;
            canvas.worldCamera = bossCamera;
        }
    }
}
