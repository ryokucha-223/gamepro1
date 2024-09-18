using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public string previousScene;
    private Fademane fademane;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).Name);
                    instance = singleton.AddComponent<GameManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }

    public bool clst = false; // �X�e�[�W�N���A�t���O

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        fademane = FindObjectOfType<Fademane>();//fademane�̎擾
        if (fademane == null)
        {
            Debug.LogError("Fademane instance not found in the scene.");
        }
    }

    public void OnFlag()
    {
        clst = true;
    }

    public void ResetFlags()
    {
        clst = false;
    }

    public void SavePreviousScene(string sceneName)
    {
        previousScene = sceneName;
    }

    public void LoadPreviousScene()
    {
        if (!string.IsNullOrEmpty(previousScene))
        {
            fademane.ChangeSceneWithFade(1f, 0.5f, previousScene); // �t�F�[�h���Ԃ͓K�X����
        }
    }
}
