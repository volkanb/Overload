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
            tc.isComboTile = true;

            // Determine if UCT is going to be big
            if (GameController.totalUniqueComboTiles > 1 && (GameController.totalUniqueComboTiles % 3) == 0 )
            {
                tc.comboCounter = (GameController.totalUniqueComboTiles + Random.Range(5, 10));
            }
            else
            {
                tc.comboCounter = Random.Range(4, 6);
            }            
                        
            GameController.totalUniqueComboTiles++;
            
            // Tile text adjustment
            tc.textComponent = tileClone.transform.GetChild(0).transform.GetComponentInChildren<Text>();
            tc.textComponent.text = tc.comboCounter.ToString();

            // Sounds
            gController.audioSource.PlayOneShot(gController.shootSound);
        }
        else
        {
            tileClone.transform.GetChild(0).gameObject.SetActive(false);

            // Sounds
            gController.audioSource.PlayOneShot(gController.uctShootSound);
        }       
    }
}
