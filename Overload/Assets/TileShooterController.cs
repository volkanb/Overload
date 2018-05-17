using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileShooterController : MonoBehaviour {

    public float secondsToStart = 3.0f;
    public float intervalSeconds = 3.0f;
    public Rigidbody2D Tile;
    //public float initialTileSpeed = 10.0f;

    void Start()
    {
        InvokeRepeating("CreateTile", secondsToStart, intervalSeconds);
    }


    void CreateTile()
    {
        Rigidbody2D tileClone = (Rigidbody2D)Instantiate(Tile, transform.position, transform.rotation);
        
        tileClone.GetComponent<TileController>().GetColor();
    }
}
