using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticDataHolder : MonoBehaviour {

    private static bool created = false;

    public static int endScore = 0;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            //Debug.Log("Awake: " + this.gameObject);
        }
    }

    public void LoadScene(int score)
    {
        if (SceneManager.GetActiveScene().name == "EndlessScene")
        {
            endScore = score;
            SceneManager.LoadScene("ScoreScene", LoadSceneMode.Single);
        }
    }

    public void RestartGame()
    {
        if (SceneManager.GetActiveScene().name == "ScoreScene")
        {            
            SceneManager.LoadScene("EndlessScene", LoadSceneMode.Single);
        }
    }

}
