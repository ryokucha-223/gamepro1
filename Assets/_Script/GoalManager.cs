using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalManager : MonoBehaviour
{
    [SerializeField] float waittime = 1f;
    public newsystem newsystem;

    private Fademane fademane;

    private void Awake()
    {
        // シーン内のFademaneコンポーネントを取得
        fademane = FindObjectOfType<Fademane>();
        if (fademane == null)
        {
            Debug.LogError("Fademaneスクリプトがシーン内に見つかりません。");
        }
    }

    public void HandleGoal()
    {
        GameManager.Instance.clst = true;
        bool clstFlag = GameManager.Instance.clst;
        Debug.Log(GameManager.Instance.clst);
        Debug.Log(clstFlag);
        string currentScene = SceneManager.GetActiveScene().name;
        newsystem.InGoal();

        // フェード効果を使用してシーンを変更
        StartCoroutine(ChangeSceneWithFade(currentScene));
    }

    private IEnumerator ChangeSceneWithFade(string currentScene)
    {
        yield return new WaitForSeconds(waittime);

        // 現在のシーンに応じて次のシーンを決定
        string nextScene;
        if (currentScene == "STAGE2")
        {
            nextScene = "Ending";
        }
        else
        {
            nextScene = "ClearScene";
        }

        // Fademaneスクリプトを使ってフェード効果でシーンを変更
        if (fademane != null)
        {
            fademane.ChangeSceneWithFade(1f, 0.5f, nextScene); // フェード時間は適宜調整
        }
        else
        {
            Debug.LogError("Fademaneスクリプトが見つかりません。");
            // Fademaneが見つからない場合のフォールバック
            SceneManager.LoadScene(nextScene);
        }

        Debug.Log("Clear");
    }
}
