using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip buttonSound;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadEndlessScene()
    {
        // Sounds
        audioSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene("EndlessScene", LoadSceneMode.Single);        
    }

    public void QuitGame()
    {
        // Sounds
        audioSource.PlayOneShot(buttonSound);
        Application.Quit();
    }

}
