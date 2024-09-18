using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursoVis : MonoBehaviour
{
    // �J�[�\���̃t���O
    private bool cursorVisible = false;

    void Start()
    {
        // ������ԂŃ}�E�X�J�[�\�����\���ɂ��A���b�N����
        Cursor.visible = cursorVisible;
        Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // �J�[�\���̏�Ԃ�؂�ւ���
            cursorVisible = !cursorVisible;
            Cursor.visible = cursorVisible;
            Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
