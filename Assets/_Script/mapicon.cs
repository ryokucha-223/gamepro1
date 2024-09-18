using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapicon : MonoBehaviour
{
    void LateUpdate()
    {
        // �v���C���[�i�e�I�u�W�F�N�g�j�̃X�P�[�����擾
        Vector3 parentScale = transform.localScale;

        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // �X�v���C�g�����]���Ȃ��悤�ɃX�P�[���𒲐�
                spriteRenderer.flipX = parentScale.x < 0;
                spriteRenderer.flipY = parentScale.y < 0;
            }
        }
    }
}