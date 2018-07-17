using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSceneController : MonoBehaviour
{

    public int playerScoreToDisplay = 0;
    StaticDataHolder dh;

    // Use this for initialization
    void Start()
    {
        dh = GameObject.Find("StaticDataObject").GetComponent<StaticDataHolder>();

        playerScoreToDisplay = StaticDataHolder.endScore;
        StaticDataHolder.endScore = 0;       
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(200, 200, 100, 80), "Game Over. Your score is: " + playerScoreToDisplay);            // score display

        if (GUI.Button(new Rect(350, 200, 100, 50), "Restart"))
        {
            dh.RestartGame();
        }
    }
}
