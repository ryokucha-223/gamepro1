using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour
{
    public TextMeshProUGUI highScoreText; 
    private int highScore; // ハイスコア
    private string key = "HIGH SCORE"; // ハイスコアの保存先キー
    int score = newsystem.score; // 現在のスコア
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
        score = newsystem.score; // スコアを更新
        string currentScene = SceneManager.GetActiveScene().name;
        if(currentScene== "ClearScene" || currentScene == "Ending")
        {
            inStage = false;
        }
        else
        {
            inStage = true;
        }
        if (Input.GetKeyDown(KeyCode.Delete)&&!inStage)//deleteキーでハイスコアのリセット
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
            // ハイスコアを更新
            Debug.Log("updateed:" + highScore);
            Debug.Log("sc:" + score);
            PlayerPrefs.SetInt(key, highScore);
            // ハイスコアを保存
        }
        // ハイスコアより現在のスコアが高い場合
        ShowScoreText();
    }

    private void ShowScoreText()
    {
        if (highScoreText == null) return;

        Debug.Log("text;" + highScore);
        highScoreText.text = "ハイスコア： " + highScore;
        // ハイスコアを表示

    }
}
