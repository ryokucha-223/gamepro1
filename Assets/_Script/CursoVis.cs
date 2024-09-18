using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursoVis : MonoBehaviour
{
    // カーソルのフラグ
    private bool cursorVisible = false;

    void Start()
    {
        // 初期状態でマウスカーソルを非表示にし、ロックする
        Cursor.visible = cursorVisible;
        Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // カーソルの状態を切り替える
            cursorVisible = !cursorVisible;
            Cursor.visible = cursorVisible;
            Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
