using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraCon : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject subCamera;
    void Start()
    {
        // �e�J�����I�u�W�F�N�g���擾
        mainCamera = GameObject.Find("Main Camera");
        subCamera = GameObject.Find("Sub Camera");

        // �T�u�J�����̓f�t�H���g�Ŗ����ɂ��Ă���
        subCamera.SetActive(false);
    }

    void Update()
    {
        // ����Space�L�[�������ꂽ�Ȃ�΁A
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �e�J�����I�u�W�F�N�g�̗L���t���O���t�](true��false,false��true)������
            mainCamera.SetActive(!mainCamera.activeSelf);
            subCamera.SetActive(!subCamera.activeSelf);
        }
    }
}
