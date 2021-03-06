﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSceneController : MonoBehaviour
{

    public GameObject recentScoreObj;
    public GameObject highScoresObj;

    public AudioSource audioSource;
    public AudioClip gameOverSound;


    public int playerScoreToDisplay = 0;
   

    // Use this for initialization
    void Start()
    {
        // Sounds
        audioSource.PlayOneShot(gameOverSound);
        
        playerScoreToDisplay = StaticDataHolder.endScore;
        StaticDataHolder.endScore = 0;


        // Write the recent score to 
        recentScoreObj.GetComponent<Text>().text = playerScoreToDisplay.ToString();

        // High scores handling
        int[] highScores = new int[5];

        // Set the high score slots, if not set before
        for (int i = 0; i < 5; i++)
        {
            if (!PlayerPrefs.HasKey("highScore" + (i+1).ToString()))
                PlayerPrefs.SetInt("highScore" + (i+1).ToString(), 0);
        }

        // Retrive high scores        
        highScores[0] = PlayerPrefs.GetInt("highScore1", 0);
        highScores[1] = PlayerPrefs.GetInt("highScore2", 0);
        highScores[2] = PlayerPrefs.GetInt("highScore3", 0);
        highScores[3] = PlayerPrefs.GetInt("highScore4", 0);
        highScores[4] = PlayerPrefs.GetInt("highScore5", 0);

       

        // Update high scores
        bool replace = false;
        int replacement = -1;
        for (int i = 0; i < 5; i++)
        {
            if (playerScoreToDisplay >= highScores[i])
            {
                if(!replace)
                {
                    replace = true;
                    replacement = highScores[i];
                    PlayerPrefs.SetInt("highScore" + (i+1).ToString(), playerScoreToDisplay);
                }
                else
                {                                        
                    PlayerPrefs.SetInt("highScore" + (i+1).ToString(), replacement);
                    replacement = highScores[i];
                }
            }
        }

        // Write the high scores 
        highScoresObj.transform.Find("1").GetComponent<Text>().text = ("1 - " + PlayerPrefs.GetInt("highScore1", 0).ToString());
        highScoresObj.transform.Find("2").GetComponent<Text>().text = ("2 - " + PlayerPrefs.GetInt("highScore2", 0).ToString());
        highScoresObj.transform.Find("3").GetComponent<Text>().text = ("3 - " + PlayerPrefs.GetInt("highScore3", 0).ToString());
        highScoresObj.transform.Find("4").GetComponent<Text>().text = ("4 - " + PlayerPrefs.GetInt("highScore4", 0).ToString());
        highScoresObj.transform.Find("5").GetComponent<Text>().text = ("5 - " + PlayerPrefs.GetInt("highScore5", 0).ToString());


    }

    // Update is called once per frame
    void Update()
    {

    }
}
