using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] GameObject Item, Breakefect;
    [SerializeField] AudioClip se_break;
    AudioSource snd;

    private void Start()
    {
        snd = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "shot" || collision.gameObject.tag == "slash" || collision.gameObject.tag == "lassl" || collision.gameObject.tag == "beam")
        {
            Debug.Log("Hit by: " + collision.gameObject.tag);
            GameObject audioSourceObj = new GameObject("TemporaryAudio");
            AudioSource temporaryAudioSource = audioSourceObj.AddComponent<AudioSource>();
            temporaryAudioSource.clip = se_break;
            temporaryAudioSource.Play();
            Destroy(audioSourceObj, se_break.length); // ���̍Đ����I�������ɃI�[�f�B�I�\�[�X���폜
            snd.PlayOneShot(se_break);
            Instantiate(Item, transform.position, Quaternion.identity);
            GameObject effect = Instantiate(Breakefect, transform.position, Quaternion.identity);
            Destroy(effect, GetParticleSystemDuration(effect));
            Destroy(gameObject); // Box�I�u�W�F�N�g�𓯎��ɍ폜
        }
    }

    private float GetParticleSystemDuration(GameObject effect)
    {
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            return ps.main.duration + ps.main.startLifetime.constantMax; // �������ԂƊJ�n�����̍ő�l��Ԃ�
        }
        else
        {
            ParticleSystem[] children = effect.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0f;
            foreach (ParticleSystem child in children)
            {
                float duration = child.main.duration + child.main.startLifetime.constantMax;
                if (duration > maxDuration)
                {
                    maxDuration = duration;
                }
            }
            return maxDuration;
        }
    }
}
