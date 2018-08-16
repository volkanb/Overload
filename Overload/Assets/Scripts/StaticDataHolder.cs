using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticDataHolder : MonoBehaviour {

    private static bool created = false;

    public AudioSource audioSource;
    public AudioClip buttonSound;

    public static int endScore = 0;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;                                 
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

        // Sounds
        audioSource = Camera.main.gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(buttonSound);

        endScore = 0;
        SceneManager.LoadScene("EndlessScene", LoadSceneMode.Single);              
    }

    public void QuitGame()
    {
        // Sounds
        audioSource = Camera.main.gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(buttonSound);

        Application.Quit();
    }

    public void LoadMenuScene()
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        // Sounds
        audioSource = Camera.main.gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(buttonSound);

        endScore = 0;
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
}
