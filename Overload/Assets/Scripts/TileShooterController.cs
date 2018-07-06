using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
