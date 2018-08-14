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

            if (SceneManager.GetActiveScene().name == "MenuScene")                            
                endScore = 0;            
        }
    }

    public void LoadScoreScene(int score)
    {
        if (SceneManager.GetActiveScene().name == "EndlessScene")
        {
            endScore = score;
            SceneManager.LoadScene("ScoreScene", LoadSceneMode.Single);
        }
    }

    public void RestartGame()
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;
        endScore = 0;
        SceneManager.LoadScene("EndlessScene", LoadSceneMode.Single);              
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMenuScene()
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;
        endScore = 0;
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
}
