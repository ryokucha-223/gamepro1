using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour
{
    public TextMeshProUGUI highScoreText; 
    private int highScore; // �n�C�X�R�A
    private string key = "HIGH SCORE"; // �n�C�X�R�A�̕ۑ���L�[
    int score = newsystem.score; // ���݂̃X�R�A
    private bool inStage;

    // Start is called before the first frame update
    void Start()
    {
        highScore = PlayerPrefs.GetInt(key, 0);

        ShowScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        score = newsystem.score; // �X�R�A���X�V
        string currentScene = SceneManager.GetActiveScene().name;
        if(currentScene== "ClearScene" || currentScene == "Ending")
        {
            inStage = false;
        }
        else
        {
            inStage = true;
        }
        if (Input.GetKeyDown(KeyCode.Delete)&&!inStage)//delete�L�[�Ńn�C�X�R�A�̃��Z�b�g
        {
            highScore = 0;
            PlayerPrefs.SetInt(key, highScore);
            Debug.Log("resetHighscore=" + highScore);
        }

    }

    public void InGoal()
    {
        if (score >= highScore)
        {
            highScore = score;
            // �n�C�X�R�A���X�V
            Debug.Log("updateed:" + highScore);
            Debug.Log("sc:" + score);
            PlayerPrefs.SetInt(key, highScore);
            // �n�C�X�R�A��ۑ�
        }
        // �n�C�X�R�A��茻�݂̃X�R�A�������ꍇ
        ShowScoreText();
    }

    private void ShowScoreText()
    {
        if (highScoreText == null) return;

        Debug.Log("text;" + highScore);
        highScoreText.text = "�n�C�X�R�A�F " + highScore;
        // �n�C�X�R�A��\��

    }
}
