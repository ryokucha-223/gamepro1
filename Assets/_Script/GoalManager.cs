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
        // �V�[������Fademane�R���|�[�l���g���擾
        fademane = FindObjectOfType<Fademane>();
        if (fademane == null)
        {
            Debug.LogError("Fademane�X�N���v�g���V�[�����Ɍ�����܂���B");
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

        // �t�F�[�h���ʂ��g�p���ăV�[����ύX
        StartCoroutine(ChangeSceneWithFade(currentScene));
    }

    private IEnumerator ChangeSceneWithFade(string currentScene)
    {
        yield return new WaitForSeconds(waittime);

        // ���݂̃V�[���ɉ����Ď��̃V�[��������
        string nextScene;
        if (currentScene == "STAGE2")
        {
            nextScene = "Ending";
        }
        else
        {
            nextScene = "ClearScene";
        }

        // Fademane�X�N���v�g���g���ăt�F�[�h���ʂŃV�[����ύX
        if (fademane != null)
        {
            fademane.ChangeSceneWithFade(1f, 0.5f, nextScene); // �t�F�[�h���Ԃ͓K�X����
        }
        else
        {
            Debug.LogError("Fademane�X�N���v�g��������܂���B");
            // Fademane��������Ȃ��ꍇ�̃t�H�[���o�b�N
            SceneManager.LoadScene(nextScene);
        }

        Debug.Log("Clear");
    }
}
