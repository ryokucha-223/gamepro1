using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class system : MonoBehaviour
{
    [SerializeField]
    int MAXBubble = 40;

    [SerializeField]
    GameObject maincamera, subcamera;

    GameObject dethobj;
    public static int score = 0;
    public static int Bubble;
    public static int GoalBubble=0;
    public static int DeadBubble=0;
    [SerializeField]
    TextMeshProUGUI clear;
    [SerializeField]
    public GameObject[] jet = new GameObject[40];
    string vs;
    // Start is called before the first frame update
    void Start()
    {
        Bubble = MAXBubble;
        GoalBubble = 0;
        DeadBubble = 0;
        subcamera.SetActive(false);
        
        for(int i=0;i<MAXBubble;i++)
        {
            vs= "Player (" + i.ToString()+ ")";
            jet[i] = GameObject.Find(vs);
           // Debug.Log(jet[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DeadBubble == MAXBubble)
        {
            Debug.Log(DeadBubble);
            SceneManager.LoadScene("Gameover");
        }
        if (Bubble == 0 && GoalBubble >= 1)
        {
            clear.text = "CLEAR!";
            subcamera.SetActive(true); ;
            maincamera.SetActive(false);
            StartCoroutine(str());

        }
    }
    public void jetstep()
    {
        Debug.Log("aaa");
        /* if (Bubble >= 1)
         {
             for(int m=MAXBubble-1;m>0;m--)
             {
                 if (jet[m] != null)
                 {
                     Destroy(jet[m]);
                     Bubble--;
                     DeadBubble++;
                     break;
                 }
             }
         }*/
        int m = MAXBubble - 1;
       while(m>1)
        {
            if(jet[m]!=null)
            {
                Destroy(jet[m]);
                Bubble--;
                DeadBubble++;
                break;
            }
            m--;
        }
    }
    IEnumerator str()//seÇ™Ç»ÇËèIÇÌÇÈÇ‹Ç≈ë“ã@
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("ClearScene");
    }
}
