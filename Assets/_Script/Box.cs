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
            Destroy(audioSourceObj, se_break.length); // 音の再生が終わった後にオーディオソースを削除
            snd.PlayOneShot(se_break);
            Instantiate(Item, transform.position, Quaternion.identity);
            GameObject effect = Instantiate(Breakefect, transform.position, Quaternion.identity);
            Destroy(effect, GetParticleSystemDuration(effect));
            Destroy(gameObject); // Boxオブジェクトを同時に削除
        }
    }

    private float GetParticleSystemDuration(GameObject effect)
    {
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            return ps.main.duration + ps.main.startLifetime.constantMax; // 持続時間と開始寿命の最大値を返す
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
