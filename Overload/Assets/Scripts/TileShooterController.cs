using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileShooterController : MonoBehaviour {

    public float secondsToStart = 3.0f;
    public float intervalSeconds = 3.0f;
    public Rigidbody2D Tile;
    public GameController gController;

    void Start()
    {
        gController = GameObject.Find("GameController").GetComponent<GameController>();
        InvokeRepeating("CreateTile", secondsToStart, intervalSeconds);
    }


    void CreateTile()
    {
        Rigidbody2D tileClone = Instantiate(Tile, transform.position, transform.rotation);
        
        tileClone.GetComponent<TileController>().GetColor();

        gController.AddNewTile(transform.GetInstanceID(), tileClone.gameObject);

        // Unique combo tile processing
        if (GameController.totalPops > 2 && (GameController.totalPops % 3) == 0 && GameController.totalUniqueComboTiles < (int)(GameController.totalPops / 3))
        {
            TileController tc = tileClone.GetComponent<TileController>();
            tc.textComponent = tileClone.transform.GetChild(0).transform.GetComponentInChildren<Text>();

            tc.isComboTile = true;
            GameController.totalUniqueComboTiles++;

            // Combo counter calculation depending on level progress
            int progressIncrement = ( (int)(GameController.totalPops / 3) );         // Calculate level progress based increment
            tc.comboCounter = ((int)Random.Range(2.0f, 4.0f) + progressIncrement);      // Add random value
            tc.textComponent.text = tc.comboCounter.ToString();            
        }
        else
        {
            tileClone.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
