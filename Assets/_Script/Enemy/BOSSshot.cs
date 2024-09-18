using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSShot : MonoBehaviour
{
    private Vector2 shotDirection;
    [SerializeField] GameObject efect;

    void Start()
    {
        // �e�̈ړ������ɉ����ăX�v���C�g�𔽓]������
        UpdateSpriteDirection();
    }

    private void UpdateSpriteDirection()
    {
        // shotDirection�����K������Ă���Ɖ��肵�āA�����Ɋ�Â��ăX�v���C�g�𔽓]
        if (shotDirection.x < 0)
        {
            // �������ɐi��ł���ꍇ�A�X�v���C�g�𔽓]
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // �E�����ɐi��ł���ꍇ�A�X�v���C�g�����̂܂܂ɂ���
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ground�I�u�W�F�N�g�ɐG�ꂽ�ꍇ�A�e�̐i�s�����ƐڐG�������r
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "shot")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "beam")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "slash")
        {
            CreateParticleEffect();
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "lassl")
        {
            CreateParticleEffect();
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Player")
        {
            Vector2 collisionPoint = collision.ClosestPoint(transform.position);

            // �G��Knockback�X�N���v�g���擾
            PlayyerMove playerMove = collision.gameObject.GetComponent<PlayyerMove>();
            if (playerMove != null)
            {
                // �m�b�N�o�b�N��K�p
                playerMove.plDamage(collisionPoint);
            }
            Destroy(gameObject);
        }
    }
    private void CreateParticleEffect()
    {
        if (efect != null)
        {
            // �p�[�e�B�N���G�t�F�N�g��e�Ȃ��Ő���
            GameObject particleEffect = Instantiate(efect, transform.position, Quaternion.identity);
            
        }
    }
}
