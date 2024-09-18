using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class newsystem : MonoBehaviour
{
    public static int score = 0;
    public static int coincount = 0;
    public static int diyacount = 0;
    public static int killcount = 0;
    public static int oxycount = 0;
    [SerializeField] static float oxyScore = 5000; // ステージごとに変更する酸素で増えるスコアの基準値


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        coincount = 0;
        diyacount = 0;
        killcount = 0;
        oxycount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene("TitleScenes");
        }
    }

    public  void InGoal()
    {
        float myFloat = PlayyerMove.oxyval * oxyScore; // PlayyerMove.oxyvalを使用
        int myInt = (int)Math.Round(myFloat);
        oxycount = myInt;
        score += myInt;
        Debug.Log("Updated Score: " + score);
    }
}
